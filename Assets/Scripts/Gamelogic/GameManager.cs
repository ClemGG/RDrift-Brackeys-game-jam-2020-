//This script manages the timing and flow of the game. It is also responsible for telling
//the UI when and how to update

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
	//The game manager holds a public static reference to itself. This is often referred to
	//as being a "singleton" and allows it to be access from all other objects in the scene.
	//This should be used carefully and is generally reserved for "manager" type objects
	public static GameManager instance;


	[Header("Race Settings")]
	public int countdownSeconds = 3;            //The number of seconds before the start of the race
	public int numberOfLaps = 3;            //The number of laps to complete
	public VehicleMovement vehicleMovement; //A reference to the ship's VehicleMovement script
	public AudioClip countdownClip, GoClip;

	[Header("UI References")]
	[SerializeField] Animator countdownAnimator;
	[SerializeField] TextMeshProUGUI countdownText;

	int currentLap = 0;                     //The current lap the player is on
	[HideInInspector] public bool isGameOver;                        //A flag to determine if the game is over
	bool raceHasBegun;                      //A flag to determine if the race has begun
	int _countdownTimer;

	void Awake()
	{
		//If the variable instance has not be initialized, set it equal to this
		//GameManager script...
		if (instance == null)
			instance = this;
		//...Otherwise, if there already is a GameManager and it isn't this, destroy this
		//(there can only be one GameManager)
		else if (instance != this)
			Destroy(gameObject);
	}

	void Start()
	{
		//When the GameManager is enabled, we start a coroutine to handle the setup of
		//the game. It is done this way to allow our intro cutscene to work. By slightly
		//delaying the start of the race, we give the cutscene time to take control and 
		//play out
		StartCoroutine(Init());
	}

	IEnumerator Init()
	{
		//yield return new WaitForEndOfFrame();

		countdownText.text = (countdownSeconds - _countdownTimer).ToString();
		countdownAnimator.Play("countdown_start");

		while (_countdownTimer < countdownSeconds)
		{
			AudioManager.instance.Play(countdownClip);
			yield return new WaitForSeconds(1f);

			_countdownTimer++;
			countdownText.text = (countdownSeconds - _countdownTimer).ToString();
		}

		countdownText.text = "GO!";
		countdownAnimator.Play("countdown_go");
		AudioManager.instance.Play(GoClip);
		ItemManager.instance.OnEpreuveStarted();


		//Initialize the lapTimes array and set that the race has begun
		raceHasBegun = true;
	}

	//Called by the FinishLine script
	public void PlayerCompletedLap()
	{
		//If the game is already over exit this method 
		if (isGameOver)
			return;

		//Incrememebt the current lap
		currentLap++;


		//If the player has completed the required amount of laps...
		if (currentLap >= numberOfLaps)
		{
			//...the game is now over...
			isGameOver = true;
			ItemManager.instance.OnEpreuveEnded(true);
		}
	}



	public bool IsActiveGame()
	{
		//If the race has begun and the game is not over, we have an active game
		return raceHasBegun && !isGameOver;
	}

	public void Restart()
	{
		//Restart the scene by loading the scene that is currently loaded
		SceneFader.instance.FadeToScene(SceneFader.GetCurSceneIndex());
	}
	public void ReturnToMainMenu()
	{
		//Restart the scene by loading the scene that is currently loaded
		SceneFader.instance.FadeToScene(0);
	}
}
