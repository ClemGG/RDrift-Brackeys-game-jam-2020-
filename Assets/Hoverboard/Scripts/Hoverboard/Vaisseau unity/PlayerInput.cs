//This script handles reading inputs from the player and passing it on to the vehicle. We 
//separate the input code from the behaviour code so that we can easily swap controls 
//schemes or even implement and AI "controller". Works together with the VehicleMovement script

using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	[Tooltip("The name of the thruster axis.")]
	public string verticalAxisName = "Vertical";  

	[Tooltip("The name of the rudder axis.")]
	public string horizontalAxisName = "Horizontal";
	
	[Tooltip("The name of the brake button.")]
	public string brakingKey = "Brake";

	[Tooltip("The name of the drift button.")]
	public string driftingKey = "Drift";    
	
	[Tooltip("The name of the jump button.")]
	public string jumpKey = "Jump";             
	
	[Tooltip("The name of the boost button.")]
	public string boostKey = "Boost";

	[Tooltip("The name of the pause button.")]
	public string pauseKey = "Pause";

	//We hide these in the inspector because we want 
	//them public but we don't want people trying to change them
	[HideInInspector] public float thruster;			
	[HideInInspector] public float rudder;				
	[HideInInspector] public bool isBraking;			
	[HideInInspector] public bool isDrifting;			
	[HideInInspector] public bool isChargingJump;		
	[HideInInspector] public bool hasReleasedJump;		
	[HideInInspector] public bool hasActivatedBoost;		
	[HideInInspector] public bool isPausing;		

	void Update()
	{
		//If the player presses the Escape key and this is a build (not the editor), exit the game
		if (Input.GetButtonDown("Cancel") && !Application.isEditor)
			Application.Quit();

		//If a GameManager exists and the game is not active...
		if (GameManager.instance != null && !GameManager.instance.IsActiveGame() && (ItemManager.instance.epreuveHasStarted || !ItemManager.instance.epreuveHasEnded))
		{
			//...set all inputs to neutral values and exit this method
			thruster = rudder = 0f;
			isBraking = isChargingJump = isDrifting = hasActivatedBoost = false;
			return;
		}

		if (Input.GetButtonDown(pauseKey))
		{
			isPausing = !isPausing;
		}

		//Get the values of the thruster, rudder, and brake from the input class
		thruster = Input.GetAxis(verticalAxisName);
		rudder = Input.GetAxis(horizontalAxisName);
		//isBraking = Input.GetButton(brakingKey);
		isDrifting = Input.GetButton(driftingKey);
		isChargingJump = Input.GetButton(jumpKey);
		hasActivatedBoost = Input.GetButtonDown(boostKey);

		//La raison pour laquelle on n'utilise pas simplement GetButton() est parce que l'on veut que le boost désactive le frein quand on l'active.
		//Donc pour freiner à nouveau après un boost, le joueur doit à nouveau appuyer sur le frein.
		if (Input.GetButtonDown(brakingKey))
		{
			isBraking = true;
		}
		else if (Input.GetButtonUp(brakingKey))
		{
			isBraking = false;
		}


		if (Input.GetButtonUp(jumpKey))
		{
			hasReleasedJump = true;
		}

		

	}
}
