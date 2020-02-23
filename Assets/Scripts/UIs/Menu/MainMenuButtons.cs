using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] GameObject panelControls, panelLevelSelect;

    bool panelDone = false;

    private void Start()
    {
        OnLevelWasLoaded(0);
    }

    public void OnLevelWasLoaded(int level)
    {
        DisablePanel(panelControls);
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
