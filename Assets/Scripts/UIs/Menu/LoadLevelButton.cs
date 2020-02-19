using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadLevelButton : MonoBehaviour
{
    Button btn;
    GameObject black, lockIcon, doneIcon, recordText;

    private void Awake()
    {
        btn = GetComponent<Button>();
        black = transform.GetChild(1).gameObject;
        lockIcon = transform.GetChild(2).gameObject;
        doneIcon = transform.GetChild(3).gameObject;
        recordText = transform.GetChild(4).gameObject;
    }

    public void MarkAsLocked(bool locked)
    {
        btn.interactable = !locked;
        doneIcon.SetActive(false);
        recordText.SetActive(false);
        if (locked)
        {
            black.SetActive(true);
            lockIcon.SetActive(true);
        }
        else
        {
            if (black) Destroy(black);
            if (lockIcon) Destroy(lockIcon);
        }
    }

    public void DisplayNewRecord(int levelIndex)
    {
        recordText.GetComponent<TextMeshProUGUI>().text = "<size=20>Record:</size>\n" + ConvertToTime(PlayerPrefs.GetFloat($"New Record {levelIndex}", 0));
    }

    public void MarkAsDone()
    {
        btn.interactable = true;
        doneIcon.SetActive(true);
        recordText.SetActive(true);


        if (black) Destroy(black);
        if (lockIcon) Destroy(lockIcon);
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

        string minutes = min.ToString(min >= 10 ? "00" : "0");
        string seconds = sec.ToString("00");

        return string.Format("{0}:{1}", minutes, seconds);
    }

}
