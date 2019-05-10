using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Windows.Input;

public class Pause : MonoBehaviour
{
    public bool paused;
    GameController gameController;
    public GameObject pauseFilter;
    public GameObject[] pauseButtons;
    GameManager gMan;

    private void Start()
    {
        gameController = GetComponent<GameController>();
        paused = false;
        PauseGame(paused);
    }

    // Update is called once per frame
    void Update()
    {
        if((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)))
        {
            paused = !paused;
            PauseGame(paused);
        }
    }

    public void PauseGame(bool p)
    {
        if (p)
        {
            Time.timeScale = 0f;
            pauseFilter.SetActive(true);
            paused = true;
            gameController.OnPaused(paused);
            for (int i = 0; i < pauseButtons.Length; i++) pauseButtons[i].SetActive(true);
        }
        
        else
        {
            Time.timeScale = 1f;
            pauseFilter.SetActive(false);
            paused = false;
            gameController.OnPaused(paused);
            for (int i = 0; i < pauseButtons.Length; i++) pauseButtons[i].SetActive(false);
        }
    }
}
