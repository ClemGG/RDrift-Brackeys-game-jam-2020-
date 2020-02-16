using System;
using System.Collections.Generic;
using UnityEngine;

public class HoverArray : MonoBehaviour {

    #region Variables

    [Space(10)]
	[Header("Scripts & Components :")]
	[Space(10)]


	// Virtual hover engines defined by points.
	[SerializeField] Transform[] stabilizers;


	Rigidbody rb;
	Transform t;


	[Space(10)]
	[Header("Inputs :")]
	[Space(10)]

    float hInput, vInput;
	bool leftClickMaintainedInput, leftClickUpInput;


	[Space(10)]
	[Header("Movement :")]
	[Space(10)]




	[SerializeField] float moveSpeed = 1000f;
	[SerializeField] float thrustForcePerEngine = 250f;	//La foce de chaque stabilisateur
	[SerializeField] float rotSpeed = 180f;

	[SerializeField] float maxTurnRotAngle = 45f; 	//La rotation maximale sur l'axe Z lors d'un virage
	[SerializeField] float maxDiveRotAngle = 5f; 	//La rotation maximale sur l'axe X lors de l'accélération

	[Space(10)]

	[SerializeField, Range(0f, 1f)] float dragInputDeadZone = 1f; 	//hInput devra être au del) de cette valeur pour changer le drag
	[SerializeField, Range(0f, 1f)] float rotSpeedSmooth = .2f;   //Plus le smooth sera élevé et plus la planche tanguera dans les virages
	[SerializeField, Range(0f, 1f)] float targetRotSpeedSmooth = .3f;   //Le smooth de l'orientation de la planche par rapport au terrain

	[Space(10)]

	[SerializeField] Vector2 minMaxRotX = new Vector2(-30f, 30f);	//Pour clamper la rotation en X et Z
	[SerializeField] Vector2 minMaxRotZ = new Vector2(-30f, 30f);
	[SerializeField] Vector2 minMaxDrag = new Vector2(.25f, 3f); //Le drag en ligne droite et dans les virages



	[Space(10)]
	[Header("Jump :")]
	[Space(10)]


	[SerializeField] Vector2 minMaxJumpForce = new Vector2(10f, 60f);

	[Space(10)]

	[SerializeField] float airborneGravityMultiplier = 100f;    //Pour faire retomber la planche plus rapidement au sol si elle n'a pas de contacts avec le sol
	[SerializeField] float jumpDelayBeforeFullForce = 3f; // Le temps que doit mettre le joueur à charger son saut pour atteindre sa pleine puissance
	[SerializeField] float delayBetweenJumps = 1f; // Pour éviter de sauter immédiatement après avoir relâché le bouton de saut
	[SerializeField] float jumpRaycastDst = 2f;

	[Space(10)]

	[SerializeField] bool isHovering = false; //Permet de savoir si la planche est suffisamment proche du sol
	[SerializeField] bool allowedToJump = false;





	[Space(10)]
	[Header("Raycast :")]
	[Space(10)]



	[SerializeField] float hoverHeight = 1.33f;
	[SerializeField] float raycastDst = 7.5f;	//La distance de détection du sol pour garder la planche en survol sans qu'elle ne s'envole
	[SerializeField] LayerMask raycastLayer;




	float _perEngineForce;		// Force used by each engine (calculated internally)
	float _rotTurnVelocity;      // Force used for the rotation while turning (calculated internally)
	float _rotDiveVelocity;      // Force used for the rotation while accelerating (calculated internally)
	float _gravityMul; //Force utilisée quand la planche n'a pas de contact avec le sol (calculated internally)
	float _jumpForce; //Force utilisée pour faire sauter la planche (calculated internally)
	float _jumpTimer;
	float _delayBetweenJumpsTimer;	//Pour s'assurer un intervalle entre chaque saut

	//Les vecteurs utilisés pour stocker la moyenne des normales au sol
	Vector3 averageNormal;
	Vector3 averagePoint;
	Quaternion targetRot; //La rotation que la planche doit adopter pour rester collée au terrain





	[Space(10)]
	[Header("Gizmos :")]
	[Space(10)]

	[SerializeField] bool showEngines;
	[SerializeField] bool showAverageNormal;
	[SerializeField] bool showJump;

	[Space(10)]

	[SerializeField] float hoverEngineSphereSize = .05f;

	[Space(10)]

	[SerializeField] Color hoverEngineColor = Color.red;
	[SerializeField] Color rayHasTouchedColor = Color.green;
	[SerializeField] Color rayHasNotTouchedColor = Color.red;
	[SerializeField] Color averageNormalColor = Color.blue;
	[SerializeField] Color allowedToJumpColor = Color.cyan;
	[SerializeField] Color notAllowedToJumpColor = Color.yellow;
	[SerializeField] Color isGroundedColor = Color.blue;



	#endregion


	#region Init
	private void Start()
	{
		Initialize();
	}


	//Initialization
	public void Initialize() {

		t = transform;
		rb = GetComponent<Rigidbody>();
		_perEngineForce = moveSpeed / stabilizers.Length;
	}

    #endregion


    #region Update

    private void Update()
	{

		GetInput();
		_perEngineForce = thrustForcePerEngine;

	}

	private void GetInput()
	{
		hInput = Input.GetAxis("Horizontal");
		vInput = Input.GetAxis("Vertical");
		leftClickMaintainedInput = Input.GetMouseButton(0);

		if (Input.GetMouseButtonUp(0))
		{
			leftClickUpInput = true;
		}


	}


	#endregion


	// Update the hover array
	public void FixedUpdate() {


        #region Calcul des stabilisateurs

        //On réinitialise les moyennes pour pouvoir les réassigner au moindre changement
        averageNormal = averagePoint = Vector3.zero;

		//Nous permet de garder le nombre de stabilisateurs ayant un contact avec le sol
		int nbContacts = 0;


		// Update each engine in the array
		foreach (var engine in stabilizers) {
			// Generate a ray for each engine and perform a ccheck
			var engineRay = new Ray(engine.position, -engine.up);
			RaycastHit rayHit;

			// Check if a surface exists below this engine and update it, otherwise continue
			Physics.Raycast(engineRay, out rayHit, raycastDst, raycastLayer);


			//On récupère la moyenne de toutes les normales au sol
			averageNormal += rayHit.normal;
			averagePoint += rayHit.point;

			if (!rayHit.collider) continue;

			nbContacts++;


			// Calculate the force this engine needs to produce
			// TODO: Improve the proportional calculation by replacing it with a proper PID controller implementation
			rb.AddForceAtPosition(t.up * _perEngineForce * (1f - Vector3.Distance(engine.position, rayHit.point) / hoverHeight), engine.position);
		}

		//On affiche la moyenne des normales au sol
		if (nbContacts > 0)
		{
			averageNormal /= nbContacts;
			averagePoint /= nbContacts;
		}
		#endregion



		#region Deplacements


		//Dans les virages, on augmente le drag pour permettre à la planche de faire des virages serrés
		rb.drag = (hInput > dragInputDeadZone || hInput < -dragInputDeadZone && isHovering && allowedToJump) ? minMaxDrag.y : minMaxDrag.x;




		/* 
		 
		  Coller la planche au terrain pour lui permettre de grimper les pentes les plus raides.
		  Si la planche n'a aucun contact avec le sol, on l'oriente vers le bas.
		  Le problème est que dès que hoverHeight est trop importante, la rotation ne sera plus réaliste.
		  C'est parce que hoverHeight n'est utile que si le mesh est plus grand, vu qu'on utilise un pourcentage pour calculer la force à appliquer,
		  ce pourcentage reste le même, ce qui est logique parce qu'une planche suffisamment puissante 
		  pour rester plus haut devient proportionnellement plus sensible aux différentes hauteurs du terrain

		 */
		float smoothMul;
		if (nbContacts == 0)
		{
			targetRot = Quaternion.FromToRotation(t.up, Vector3.up) * t.rotation;
			smoothMul = 2f;
		}
		else 
		{
			targetRot = Quaternion.FromToRotation(t.up, averageNormal.normalized) * t.rotation;
			smoothMul = 1f;
		}
		//On rajoute smoothMul pour adoucir la rotation quand la planche est en l'air, pour un effet moins violent
		t.rotation = Quaternion.Slerp(t.rotation, targetRot, targetRotSpeedSmooth / smoothMul);




		//Pour faire avancer la planche
		Vector3 forwardForce = t.forward * moveSpeed * vInput;
		forwardForce *= rb.mass * Time.deltaTime;
		rb.AddForce(forwardForce);


		//On fait pivoter la planche sur les axes X et Z pour donner un faux effet d'inertie en utilisant un smooth.
		//L'effet est plus sympa mais ça rend la planche moins maniable.
		//On laisse Vector3.up au lieu de t.up pour garder l'effet de basculement ; ça ne bug pas avec l'effet de plongeon en dessous mais c'est pas utilisable dans les pentes
		//Vector3 turnTorque = Vector3.up * rotSpeed * hInput;
		Vector3 turnTorque = t.up * rotSpeed * hInput;
		turnTorque *= rb.mass * Time.deltaTime;
		rb.AddTorque(turnTorque);



		////On met t.right à la place de Vector3.forward pour éviter les bugs de rotation
		//Vector3 diveTorque = (isHovering ? -t.right : t.right) * rotSpeed * vInput;
		//diveTorque *= rb.mass * Time.deltaTime;
		//rb.AddTorque(diveTorque);




		//Vector3 newRot = t.eulerAngles;
		//newRot.x = Mathf.SmoothDampAngle(newRot.x, vInput * -maxDiveRotAngle, ref _rotDiveVelocity, rotSpeedSmooth);
		//newRot.z = Mathf.SmoothDampAngle(newRot.z, hInput * -maxTurnRotAngle, ref _rotTurnVelocity, rotSpeedSmooth);
		//newRot.x = ClampAngle(newRot.x, minMaxRotX.x, minMaxRotX.y) + targetRot.x;
		//newRot.z = ClampAngle(newRot.z, minMaxRotZ.x, minMaxRotZ.y) + targetRot.z;
		//t.eulerAngles = newRot;





		#endregion


		#region Jump

		//Pour la faire tourner sur elle-même
		//rb.AddRelativeTorque(0f, Input.GetAxis("Horizontal") * rotSpeed, 0f);


		//Avant de faire sauter la planche, on s'assure que celle-ci est suffisamment proche du sol
		var jumpRay = new Ray(t.position, -t.up);
		isHovering = Physics.Raycast(jumpRay, jumpRaycastDst, raycastLayer);


		//Pour faire sauter la planche, la force de saut dépend de la durée de pression du bouton de saut
		if (leftClickMaintainedInput)
		{
			_jumpTimer += Time.deltaTime;
		}
		if (leftClickUpInput)
		{
			if (isHovering && allowedToJump)
			{
				//On applique la force de saut et on démarre le décompte d'intervalle entre les sauts
				_jumpForce = Mathf.Lerp(minMaxJumpForce.x, minMaxJumpForce.y, _jumpTimer / jumpDelayBeforeFullForce);
				rb.AddForce(t.up * _jumpForce * rb.mass, ForceMode.Impulse);
				allowedToJump = false;
			}

			_jumpTimer = _jumpForce = 0f;
			leftClickUpInput = false;
		}

		//Tant que le délai entre les sauts n'est pas terminé, le joueur ne peut pas sauter
		if (!allowedToJump)
		{

			if(_delayBetweenJumpsTimer < delayBetweenJumps)
			{
				_delayBetweenJumpsTimer += Time.deltaTime;

			}
			else
			{
				_delayBetweenJumpsTimer = 0f;
				allowedToJump = true;
			}
		}

		#endregion


		#region Gravité

		

		//On applique la gravité sur l'axe Y de la planche
		_gravityMul = isHovering ? 1f : airborneGravityMultiplier;
        rb.AddForce(Physics.gravity.y * t.up * _gravityMul);

        #endregion

    }


    public static float ClampAngle(float angle, float min, float max)
	{
		angle = Mathf.Repeat(angle, 360);
		min = Mathf.Repeat(min, 360);
		max = Mathf.Repeat(max, 360);
		bool inverse = false;
		var tmin = min;
		var tangle = angle;
		if (min > 180)
		{
			inverse = !inverse;
			tmin -= 180;
		}
		if (angle > 180)
		{
			inverse = !inverse;
			tangle -= 180;
		}
		var result = !inverse ? tangle > tmin : tangle < tmin;
		if (!result)
			angle = min;

		inverse = false;
		tangle = angle;
		var tmax = max;
		if (angle > 180)
		{
			inverse = !inverse;
			tangle -= 180;
		}
		if (max > 180)
		{
			inverse = !inverse;
			tmax -= 180;
		}

		result = !inverse ? tangle < tmax : tangle > tmax;
		if (!result)
			angle = max;
		return angle;
	}



#if UNITY_EDITOR


	private void OnValidate()
	{
		if(raycastDst < hoverHeight * 2f)
		{
			raycastDst = hoverHeight * 2f;
			thrustForcePerEngine = hoverHeight * moveSpeed / stabilizers.Length;
		}
	}



	private void OnDrawGizmos()
	{

		if (!t)
			t = transform;


		//On réinitialise les moyennes pour pouvoir les réassigner au moindre changement
		averageNormal = averagePoint = Vector3.zero;

		//Nous permet de garder le nombre de stabilisateurs ayant un contact avec le sol
		int nbContacts = 0;


		for (int i = 0; i < stabilizers.Length; i++)
		{

			// Check if a surface exists below this engine and update it, otherwise continue
			var engineRay = new Ray(stabilizers[i].position, -stabilizers[i].up);
			RaycastHit rayHit;
			Physics.Raycast(engineRay, out rayHit, raycastDst, raycastLayer);



			//On récupère la moyenne de toutes les normales au sol
			averageNormal += rayHit.normal;
			averagePoint += rayHit.point;




			if (stabilizers[i] != null)
			{

				if (showEngines)
				{
					Gizmos.color = rayHit.collider != null ? rayHasTouchedColor : rayHasNotTouchedColor;

					//On affiche les positions des stabilisateurs à l'écran
					Gizmos.DrawSphere(stabilizers[i].position, hoverEngineSphereSize);

					//On affiche les raycasts partant des stabilisateurs. Si le stabilisateur touche le sol, le rayon devient vert.
					Vector3 hitPos = rayHit.collider != null ? rayHit.point : (-stabilizers[i].up * raycastDst) + stabilizers[i].position;
					Gizmos.DrawLine(stabilizers[i].position, hitPos);
					
					// On affiche les points de contact à l'écran si le Raycast touche le sol
					if (rayHit.collider)
					{
						Gizmos.DrawSphere(hitPos, hoverEngineSphereSize);
					}
				}

				
				// On affiche les points de contact à l'écran si le Raycast touche le sol
				if (rayHit.collider)
				{
					nbContacts++;
				}
			}

			
		}


		if (showJump)
		{

			// Check if a surface exists below this engine and update it, otherwise continue

			Gizmos.color = allowedToJump ? (isHovering ? isGroundedColor : allowedToJumpColor) : notAllowedToJumpColor;
			Gizmos.DrawLine(t.position, t.position - t.up * jumpRaycastDst);

		}


		if (showAverageNormal)
		{

			Gizmos.color = averageNormalColor;

			//On affiche la moyenne des normales au sol
			if (nbContacts > 1)
			{
				averageNormal /= nbContacts;
				averagePoint /= nbContacts;
			}

			Gizmos.DrawLine(averagePoint, averagePoint + averageNormal.normalized);
			Gizmos.DrawSphere(averagePoint, hoverEngineSphereSize);
		}

	}


#endif
}
