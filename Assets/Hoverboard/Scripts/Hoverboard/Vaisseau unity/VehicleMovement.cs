//This script handles all of the physics behaviors for the player's ship. The primary functions
//are handling the hovering and thrust calculations. 

using System;
using UnityEngine;
using UnityEngine.Events;

public class VehicleMovement : MonoBehaviour
{

    #region Variables

    #region Components
    [Space(10)]
	[Header("Scripts & Components : ")]
	[Space(10)]


	[Tooltip("A reference to the ship's body, this is for cosmetics.")]
	public Transform shipBody;              

	Rigidbody rigidBody;                    //A reference to the ship's rigidBody
	Transform t;							//The transform of this gameObject
	PlayerInput input;                      //A reference to the player's input	


	[Space(10)]


	[Tooltip("Virtual hover engines defined by points.")]
	public Transform[] stabilizers;

	#endregion

	#region Drive Settings

	[Space(10)]
	[Header("Drive Settings")]
	[Space(10)]


	[Tooltip("The current forward speed of the ship.")]
	public float speed;                         

	[Tooltip("The force that the engine generates to move forward.")]
	public float driveForce = 17f;              

	[Tooltip("The force that the engine generates for a sharp turn.")]
	public float driftSpeed = 5f;               

	[Tooltip("The amount of drift during turns.")]
	public float turnDriftCoef = 2f;            

	[Tooltip("The closer the speed approaches this value, the weaker the rotatio smooth will be.")]
	public float maxSmoothSpeed = 40f;                               
	
	[Tooltip("The faster the board is, the more faithful is rotation will be according to the terrain.")]
	public Vector2 minMaxGravitySmooth = new Vector3(.05f, .85f);      


	[Space(10)]


	[Tooltip("The percentage of velocity the ship maintains when not thrusting (e.g., a value of .99 means the ship loses 1% velocity when not thrusting).")]
	public float slowingVelFactor = .99f;                           

	[Tooltip("The percentage of velocty the ship maintains when braking.")]
	public float brakingVelFactor = .95f;                           

	[Tooltip("The angle that the ship \"banks\" into a turn.")]
	public float angleOfRoll = 30f;                                 

	[Tooltip("The angle that the ship \"banks\" during a drift.")]
	public float angleOfRollDrift = 60f;

	#endregion


	#region Boost Settings

	[Space(10)]
	[Header("Boost Settings")]
	[Space(10)]

	[Tooltip("The impulsion added to the board to reach its maximal speed.")]
	[SerializeField] float boostForce = 30f;                

	[Tooltip("To avoid accelerating immediately after accomplishing a boost.")]
	[SerializeField] float delayBetweenBoosts = 2f;         



	// (calculated internally)
	float _boostForce;										//Force utilisée pour faire sauter la planche
	float _delayBetweenBoostsTimer;							//Pour s'assurer un intervalle entre chaque boost
	[HideInInspector] public bool allowedToBoost = false;   //Utilisée pour détecter que les conditions d'un boost sont réunies

	#endregion


	#region Jump Settings

	[Space(10)]
	[Header("Jump Settings")]
	[Space(10)]

	[Tooltip("The interval amount of force added to jump.")]
	public Vector2 minMaxJumpForce = new Vector3(10f, 60f);         

	[Tooltip("The maximal distance until which the board can detect the ground during a jump.")]
	[SerializeField] float maxJumpGroundDst = 30f;                  

	[Tooltip("The delay the player has to charge its jump to reach its full power.")]
	[SerializeField] float jumpDelayBeforeFullForce = 3f;           

	[Tooltip("To avoid jumping immediately after accomplishing a jump.")]
	[SerializeField] float delayBetweenJumps = 1f;                  

	[Tooltip("The jump duration allowed after loosing contact with the ground before gravity applies anew.")]
	[SerializeField] float delayBeforeGravityApplies = 3f;          


	// (calculated internally)
	float _jumpForce;               //Force ajoutée à la planche
	float _jumpTimer;				//Permet de calculer la durée de pression du bouton de saut
	float _isJumpingTimer;			//Après que la planche perde le contact avec le sol, permet de calculer la durée pendant laquelle 
									//le saut est maintenu avant que la gravité ne s'applique à nouveau sur la planche
	
	float _delayBetweenJumpsTimer;  //Pour s'assurer un intervalle entre chaque saut
	bool allowedToJump = false;     //Utilisée pour détecter que les conditions d'un saut sont réunies

	#endregion




	#region Hover Settings

	[Space(10)]
	[Header("Hover Settings")]
	[Space(10)]

	[Tooltip("The height the ship maintains when hovering.")]
	public float hoverHeight = 1.5f;        
	[Tooltip("The distance the ship can be above the ground before it is \"falling\".")]
	public float maxGroundDist = 5f;        
	[Tooltip("The force of the ship's hovering.")]
	public float hoverForce = 300f;         
	[Tooltip("A layer mask to determine what layer the ground is on.")]
	public LayerMask whatIsGround;          


	[Space(10)]


	[Tooltip("A PID controller to smooth the ship's hovering.")]
	public PIDController hoverPID;


	#endregion



	#region Physics Settings

	[Space(10)]
	[Header("Physics Settings")]
	[Space(10)]


	[Tooltip("The max speed the ship can go.")]
	public float terminalVelocity = 100f;        
	[Tooltip("The max speed the ship can go while boosting.")]
	public float boostTerminalVelocity = 120f;   
	[Tooltip("The gravity applied to the ship while it is on the ground.")]
	public float hoverGravity = 20f;             
	[Tooltip("The gravity applied to the ship while it is falling.")]
	public float fallGravity = 80f;              
	[Tooltip("The knockback force applied to the board when it hits a wall.")]
	public float collisionKnockbackForce = 10f;      
	[Tooltip("The angle from which the knockback force will have to stop the boost.")]
	public float collisionKnockbackStopBoostAngle = .5f;


	// (calculated internally)
	float drag;                             //The air resistance the ship recieves in the forward direction
	bool isOnGround;						//A flag determining if the ship is currently on the ground
	bool isJumping;                         //A flag determining if the ship is currently jumping on any kind of surface

    #endregion


    #endregion





    void Start()
	{
		//Get references to the Rigidbody and PlayerInput components
		t = transform;
		rigidBody = GetComponent<Rigidbody>();
		input = GetComponent<PlayerInput>();

		//Calculate the ship's drag value
		drag = driveForce / terminalVelocity;
	}

	void FixedUpdate()
	{
		//Calculate the current speed by using the dot product. This tells us
		//how much of the ship's velocity is in the "forward" direction 
		speed = Vector3.Dot(rigidBody.velocity, transform.forward);

		//Calculate the forces to be applied to the ship
		CalculatHover();
		CalculatePropulsion();
	}





    #region Movements & Collisions
    void CalculatHover()
	{

        #region Stabilisateurs

        //This variable will hold the "normal" of the ground. Think of it as a line
        //the points "up" from the surface of the ground

        Vector3 groundNormal = Vector3.zero;
		Vector3 groundPoint = Vector3.zero;


		//Nous permet de garder le nombre de stabilisateurs ayant un contact avec le sol
		int nbContacts = 0;
		float closetDst = Mathf.Infinity;

		// Update each engine in the array
		foreach (var engine in stabilizers)
		{
			// Generate a ray for each engine and perform a ccheck
			var engineRay = new Ray(engine.position, -engine.up);
			RaycastHit rayHit;

			// Check if a surface exists below this engine and update it, otherwise continue
			Physics.Raycast(engineRay, out rayHit, maxGroundDist, whatIsGround);


			//On récupère la moyenne de toutes les normales au sol et le point le plus proche du joueur pour coller à la plus proche surface
			groundNormal += rayHit.normal;

			float dst = (t.position - rayHit.point).sqrMagnitude;
			if(dst < closetDst)
			{
				closetDst = dst;
				groundPoint = rayHit.point;

			}

			if (!rayHit.collider) continue;

			nbContacts++;

		}

		//On affiche la moyenne des normales au sol
		if (nbContacts > 0)
		{
			groundNormal /= nbContacts;
			groundNormal = groundNormal.normalized;
		}






		#endregion




		#region Gravity


		//Calculate a ray that points straight down from the ship
		Ray ray = new Ray(t.position, -t.up);

		//Declare a variable that will hold the result of a raycast
		RaycastHit hitInfo;

		//Determine if the ship is on the ground by Raycasting down and seeing if it hits 
		//any collider on the whatIsGround layer
		isOnGround = Physics.Raycast(ray, out hitInfo, maxGroundDist, whatIsGround);



		//If the ship is on the ground...
		if (isOnGround)
		{
			
			//...determine how high off the ground it is...
			//float height = hitInfo.distance;
			float height = Vector3.Distance(t.position, groundPoint);

			//...save the normal of the ground...
			//Maintenant calculé avec les stabilisateurs
			//groundNormal = hitInfo.normal.normalized;

			//...use the PID controller to determine the amount of hover force needed...
			float forcePercent = hoverPID.Seek(hoverHeight, height);
			
			//...calulcate the total amount of hover force based on normal (or "up") of the ground...
			Vector3 force = groundNormal * hoverForce * forcePercent;
			//...calculate the force and direction of gravity to adhere the ship to the 
			//track (which is not always straight down in the world)...
			Vector3 gravity = -groundNormal * hoverGravity * height;

			//...and finally apply the hover and gravity forces
			rigidBody.AddForce(force, ForceMode.Acceleration);
			rigidBody.AddForce(gravity, ForceMode.Acceleration);
		}
		//...Otherwise...
		//Si on n'a pas de surface qu'on ne saute pas
		else
		{
			//...use Up to represent the "ground normal". This will cause our ship to
			//self-right itself in a case where it flips over
			groundNormal = isJumping ? t.up : Vector3.up;

			//Calculate and apply the stronger falling gravity straight down on the ship
			Vector3 gravity = -groundNormal * fallGravity;
			rigidBody.AddForce(gravity, ForceMode.Acceleration);
		}

		//On applique une gravité constante pour faire descendre le vaiseau dans les pentes.
		//Ca ajoute aussi une vitesse dans les montées et descentes.
		rigidBody.AddForce(Vector3.down * hoverGravity);

		#endregion



		#region Jump

		//Pour détecter si la planche est en plein saut
		if(Physics.Raycast(ray, out hitInfo, maxJumpGroundDst, whatIsGround))
		{
			isJumping = true;
			_isJumpingTimer = 0f;
		}
		else
		{
			if(_isJumpingTimer < delayBeforeGravityApplies)
			{
				_isJumpingTimer += Time.deltaTime;
			}
			else
			{
				isJumping = false;
			}
		}



		//Pour faire sauter la planche, la force de saut dépend de la durée de pression du bouton de saut
		if (input.isChargingJump)
		{
			_jumpTimer += Time.deltaTime;
		}
		if (input.hasReleasedJump)
		{
			if (isOnGround && allowedToJump)
			{
				//On applique la force de saut et on démarre le décompte d'intervalle entre les sauts
				_jumpForce = Mathf.Lerp(minMaxJumpForce.x, minMaxJumpForce.y, _jumpTimer / jumpDelayBeforeFullForce);
				rigidBody.AddForce(t.up * _jumpForce, ForceMode.Impulse);
				allowedToJump = false;
			}

			_jumpTimer = _jumpForce = 0f;
			input.hasReleasedJump = false;
		}

		//Tant que le délai entre les sauts n'est pas terminé, le joueur ne peut pas sauter
		if (!allowedToJump)
		{

			if (_delayBetweenJumpsTimer < delayBetweenJumps)
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

		#region Body Rotation


		//Calculate the amount of pitch and roll the ship needs to match its orientation
		//with that of the ground. This is done by creating a projection and then calculating
		//the rotation needed to face that projection
		Vector3 projection = Vector3.ProjectOnPlane(transform.forward, groundNormal);
		Quaternion rotation = Quaternion.LookRotation(projection, groundNormal);





		//Move the ship over time to match the desired rotation to match the ground. This is 
		//done smoothly (using Lerp) to make it feel more realistic
		float rotSmooth = Mathf.Lerp(minMaxGravitySmooth.x, minMaxGravitySmooth.y, speed / maxSmoothSpeed);
		rigidBody.MoveRotation(Quaternion.Lerp(rigidBody.rotation, rotation, rotSmooth));

		//Calculate the angle we want the ship's body to bank into a turn based on the current rudder.
		//It is worth noting that these next few steps are completetly optional and are cosmetic.
		//It just feels so darn cool
		float _rollAngle = input.isDrifting ? angleOfRollDrift : angleOfRoll;
		float angle = _rollAngle * -input.rudder;

		//Calculate the rotation needed for this new angle
		Quaternion bodyRotation = transform.rotation * Quaternion.Euler(0f, 0f, angle);
		//Finally, apply this angle to the ship's body
		shipBody.rotation = Quaternion.Lerp(shipBody.rotation, bodyRotation, Time.deltaTime * 10f);

        #endregion
    }

    void CalculatePropulsion()
	{
        #region Rotation & Friction


        float _rotSpd = input.isDrifting ? driftSpeed : 1f;

		//Calculate the yaw torque based on the rudder and current angular velocity
		//J'enlève l'angular velocity parce qu'elle ralentit trop la rotation
		float rotationTorque = input.rudder * _rotSpd;// - rigidBody.angularVelocity.y;

		//Apply the torque to the ship's Y axis
		//rigidBody.AddRelativeTorque(0f, rotationTorque, 0f, ForceMode.VelocityChange);
		Vector3 turnTorque = t.up * rotationTorque;
		rigidBody.AddTorque(turnTorque);

		//Calculate the current sideways speed by using the dot product. This tells us
		//how much of the ship's velocity is in the "right" or "left" direction
		float sidewaysSpeed = Vector3.Dot(rigidBody.velocity, transform.right);

		//Calculate the desired amount of friction to apply to the side of the vehicle. This
		//is what keeps the ship from drifting into the walls during turns. If you want to add
		//drifting to the game, divide Time.fixedDeltaTime by some amount
		Vector3 sideFriction = -transform.right * (sidewaysSpeed / Time.fixedDeltaTime / turnDriftCoef); 

		//Finally, apply the sideways friction
		rigidBody.AddForce(sideFriction, ForceMode.Acceleration);

        #endregion



        #region Boost countdown
        //Je place ça avant le if thruster pour que la durée du boost puisse être calculée même en l'air
        if (!allowedToBoost)
		{
			if (_delayBetweenBoostsTimer < delayBetweenBoosts)
			{
				_delayBetweenBoostsTimer += Time.deltaTime;

				//Pour faire accélérer la planche une fois le boost lancé jusqu'à ce qu'elle atteigne la boostTerminalVelocity
				if (!input.isBraking)
				{
					float _boostAcc = 0f;
					_boostAcc = boostForce > boostTerminalVelocity - speed ? Mathf.Clamp(_boostAcc, boostTerminalVelocity - speed, boostForce) : boostForce;

					float boostAcc = _boostAcc * Mathf.Clamp(speed, 0f, boostTerminalVelocity);
					rigidBody.AddForce(t.forward * boostAcc, ForceMode.Acceleration);
				}
			}
			else
			{
				_delayBetweenBoostsTimer = 0f;
				allowedToBoost = true;

				//On ramène le drag à sa valeur normale
				drag = driveForce / terminalVelocity;
			}
		}
		else
		{
			if (speed > terminalVelocity /*&& !input.isBraking*/)
			{
				//Pour faire ralentir la planche une fois le boost terminé pour la ramener à sa terminalVelocity si elle l'a dépassée
				float _boostDec = 0f;
				_boostDec = Mathf.Clamp(_boostDec, 0f, speed - terminalVelocity);

				float boostDec = _boostDec - drag * Mathf.Clamp(speed, terminalVelocity, boostTerminalVelocity);
				rigidBody.AddForce(t.forward * boostDec, ForceMode.Acceleration);
			}
		}


        #endregion


        //If not propelling the ship, slow the ships velocity
        if (input.thruster <= 0f)
			rigidBody.velocity *= slowingVelFactor;

		//Braking or driving requires being on the ground, so if the ship
		//isn't on the ground, exit this method
		if (!isOnGround)
			return;

		//If the ship is braking, apply the braking velocty reduction
		if (input.isBraking)
			rigidBody.velocity *= brakingVelFactor;



		//Le système de boost est à revoir, mais on s'en contentera pour le moment
		#region Boost & propulsion

		if (input.hasActivatedBoost)
		{
			if(isOnGround && allowedToBoost)
			{
				allowedToBoost = false;
				input.isBraking = false;

				_boostForce = boostForce > boostTerminalVelocity - speed ? Mathf.Clamp(_boostForce, 0f, boostTerminalVelocity - speed) : boostForce;
				rigidBody.AddForce(t.forward * _boostForce, ForceMode.Impulse);

				//On augmente le drag pour qu'il s'adapte à la vitesse du boost
				drag = boostForce / boostTerminalVelocity;
			}
		}



		//Calculate and apply the amount of propulsion force by multiplying the drive force
		//by the amount of applied thruster and subtracting the drag amount
		float propulsion = driveForce * input.thruster - drag * Mathf.Clamp(speed, 0f, !allowedToBoost ? boostTerminalVelocity : terminalVelocity);
		rigidBody.AddForce(t.forward * propulsion, ForceMode.Acceleration);

        #endregion
    }

    void OnCollisionStay(Collision c)
	{
		//If the ship has collided with an object on the Wall layer...
		if (c.gameObject.layer == LayerMask.NameToLayer("Terrain/Wall"))
		{
			//...calculate how much upward impulse is generated and then push the vehicle down by that amount 
			//to keep it stuck on the track (instead up popping up over the wall)
			Vector3 upwardForceFromCollision = Vector3.Dot(c.impulse, transform.up) * transform.up;
			rigidBody.AddForce(-upwardForceFromCollision, ForceMode.Impulse);

			
		}
	}

	private void OnCollisionEnter(Collision c)
	{
		//If the ship has collided with an object on the Wall layer...
		if (c.gameObject.layer == LayerMask.NameToLayer("Terrain/Wall"))
		{
			//On calcule la force à appliquer au joueur pour le repousser loin du mur après l'impact
			//La force de repoussement dépend de l'angle de la planche par rapport au mur : La force et maximale si la planche est perpendiculaire au mur

			//Ici, l'angle ira de 1 (perpendiculaire) à 0 (parallèle)
			/* On récupère l'angle (de 0 à 180), 
			 * on lui soustrait 90 pour le passer entre -90 et 90, 
			 * on prend sa valeur absolue (0 à 90), 
			 * on le divise par 90 (de 0 à 1),
			 * et on le soustrait à -1 pour avoir son inverse, afin que 1 corresponde à la perpendiculaire et 0 à la parallèle
			 */
			float angle = 1 - Mathf.Abs((Vector3.Angle(c.contacts[0].normal, t.right) - 90f ) / 90f);
			//print(angle);
			rigidBody.AddForce(c.contacts[0].normal * collisionKnockbackForce * angle, ForceMode.Impulse);

			//On arrête le boost si le facteur de recul est supérieur à 0,6
			if(angle > collisionKnockbackStopBoostAngle)
			{
				allowedToBoost = true;
				_boostForce = 0f;
				_delayBetweenBoostsTimer = 0f;
			}
		}
	}


    #endregion


    public float GetSpeedPercentage()
	{
		//Returns the total percentage of speed the ship is traveling
		return rigidBody.velocity.magnitude / terminalVelocity;
	}

	public float GetJumpChargePercentage()
	{
		return Mathf.Clamp01(_jumpTimer / jumpDelayBeforeFullForce);
	}
}
