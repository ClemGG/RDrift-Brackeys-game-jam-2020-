using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] PlayerInput input;

    [SerializeField] GameObject pauseCanvas;

    // Update is called once per frame

    private void Start()
    {
        input.isPausing = false;
        pauseCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        pauseCanvas.SetActive(input.isPausing);
        Time.timeScale = input.isPausing ? 0f : 1f;
    }

    public void ReturnToMainMenu()
    {
        input.isPausing = false;
        GameManager.instance.ReturnToMainMenu();
    }

    public void Resume()
    {
        input.isPausing = false;
    }
}
