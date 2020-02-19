using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [Header("Scripts & Components : ")]
    [Space(10)]

    [SerializeField] Animator animatorVictoryDefeat;
    [SerializeField] Animator animatorMain;
    [SerializeField] GameObject feedBackTexts, ecransVictoireDefaite, countdownDebut;
    [SerializeField] TextMeshProUGUI timeLeftText;
    [SerializeField] TextMeshProUGUI activationTimerLeftText;
    [SerializeField] TextMeshProUGUI curCoinCountText;

    [Space(20)]

    public AudioClip victoryClip;
    public AudioClip defeatClip;



    public static ItemManager instance;

    [Space(20)]

    [SerializeField] float timeLeftPinch = 10f; //Le temps à partir duquel le texte deviendra rouge pour indiquer au joueur qu'il n'a plus beaucoup de temps
    [SerializeField] Color timeLeftNormalColor, timeLeftPinchColor;


    [Header("Items : ")]
    [Space(10)]

    [SerializeField] float timerBeforeDefeat = 180f;
    [SerializeField] int maxCoinCount = 100;

    int curCoinCount;
    float _timeLeftTimer;


    [Header("Epreuve : ")]
    [Space(10)]

    [SerializeField] Door finalDoor;
    [HideInInspector] public bool epreuveHasStarted, epreuveHasEnded;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        epreuveHasEnded = epreuveHasStarted = false;
        _timeLeftTimer = timerBeforeDefeat;
        feedBackTexts.SetActive(false);
        ecransVictoireDefaite.SetActive(false);
        countdownDebut.SetActive(true);

        curCoinCountText.text = activationTimerLeftText.text = timeLeftText.text = "";

    }

    // Update is called once per frame
    void Update()
    {
        if(epreuveHasStarted && !epreuveHasEnded)
        {
            if(_timeLeftTimer > 0f)
            {
                _timeLeftTimer -= Time.deltaTime;
                timeLeftText.text = ConvertToTime(_timeLeftTimer);
                timeLeftText.color = _timeLeftTimer > timeLeftPinch ? timeLeftNormalColor : timeLeftPinchColor;

            }
            else
            {
                timeLeftText.text = ConvertToTime(0f);
                epreuveHasEnded = true;
                OnEpreuveEnded(false);
            }
        }
    }

    public void AddCoin()
    {
        if (!epreuveHasEnded)
        {
            curCoinCount++;
            curCoinCountText.text = $"{curCoinCount} / {maxCoinCount}"; 

            if (curCoinCount == maxCoinCount)
            {
                finalDoor.OpenDoor();
            }
        }
        
    }

    public void AddTime(float timeVal)
    {
        if (!epreuveHasEnded)
        {
            _timeLeftTimer += timeVal;
        }
    }


    string ConvertToTime(float f)
    {
        //Utilisé pour convertir le temps en minutes et secondes avant de l'afficher sur l'UI
        int min = (int)Mathf.Floor(f / 60);
        int sec = (int)(f % 60);

        if (sec == 60)
        {
            sec = 0;
            min++;
        }

        string minutes = min.ToString("0");
        string seconds = sec.ToString("00");

        return string.Format("{0}:{1}", minutes, seconds);
    }

    public void OnEpreuveStarted()
    {
        epreuveHasStarted = true;
        _timeLeftTimer = timerBeforeDefeat;
        curCoinCountText.text = $"{curCoinCount} / {maxCoinCount}";
        feedBackTexts.SetActive(true);
        //countdownDebut.SetActive(false);
    }

    public void OnEpreuveEnded(bool victory)
    {
        PlayerPrefs.SetInt($"Epreuve {SceneFader.GetCurSceneIndex()} done", 1);

        if(victory && timerBeforeDefeat - _timeLeftTimer < PlayerPrefs.GetFloat($"New Record {SceneFader.GetCurSceneIndex()}", timerBeforeDefeat))
            PlayerPrefs.SetFloat($"New Record {SceneFader.GetCurSceneIndex()}", timerBeforeDefeat - _timeLeftTimer);


        feedBackTexts.SetActive(false);
        ecransVictoireDefaite.SetActive(true);
        epreuveHasEnded = true;
        animatorVictoryDefeat.Play(victory ? "victory" : "defeat");
        AudioManager.instance.Play(victory ? victoryClip : defeatClip);

        StartCoroutine(DisplayVictory(victory));
    }

    private IEnumerator DisplayVictory(bool victory)
    {
        yield return new WaitForSeconds(2f);


        if (victory)
        GameManager.instance.ReturnToMainMenu();
        else
        GameManager.instance.Restart();
    }

    public IEnumerator ShowDoorTimer(Door door)
    {
        //Activer anim timer door


        while (door.shouldCount)
        {
            activationTimerLeftText.text = Mathf.Clamp(door._timer, 0f, door.activationDelay).ToString();
            yield return null;
        }

        //Désactiver anim timer door

    }
}
