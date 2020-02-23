using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] LoadLevelButton[] btns;

    private void Awake()
    {
        PlayerPrefs.SetInt($"Epreuve {0} done", 1);
    }



    private void Start()
    {


        for (int i = 0; i < btns.Length; i++)
        {
            if (btns[i])
            {
                bool locked = PlayerPrefs.GetInt($"Epreuve {i} done", 0) == 0;
                bool done = PlayerPrefs.GetInt($"Epreuve {i + 1} done", 0) == 1;
                btns[i].MarkAsLocked(locked && i != 0); //Pour que le premier bouton soit toujours activé

                if (done)
                {
                    btns[i].MarkAsDone();
                    btns[i].DisplayNewRecord(i + 1);
                }
            }
        }

    }

    public void LoadLevel(int index)
    {
        SceneFader.instance.FadeToScene(index);
    }
}
