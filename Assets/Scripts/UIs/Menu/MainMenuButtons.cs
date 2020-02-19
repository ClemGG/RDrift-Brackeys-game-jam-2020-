using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] GameObject panelBtns, panelControls, panelLevelSelect;

    bool panelDone = false;

    // Start is called before the first frame update
    void Start()
    {

        if (!panelDone)
        {
            panelBtns.SetActive(true);
            DisablePanel(panelControls, panelLevelSelect);

        }
    }


    public void OnLevelWasLoaded(int level)
    {
        DisablePanel(panelBtns, panelControls);
        panelLevelSelect.SetActive(true);
    }

    public void DisablePanel(params GameObject[] panels)
    {
        foreach (GameObject panel in panels)
        {
            if(panel.TryGetComponent(out Animator anim))
            {
                anim.Play("hide");
            }
            else
            {
                panel.SetActive(false);
            }
        }
    }

#if UNITY_EDITOR

    [ContextMenu("Delete All PlayerPrefs")]
    public void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

#endif
}
