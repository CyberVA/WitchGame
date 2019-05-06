using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public bool paused = false;
    GameController gameController;
    public SpriteRenderer pauseFilter;
    public SpriteRenderer[] pauseButtons;

    private void Start()
    {
        gameController = GetComponent<GameController>();
        PauseGame(paused);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P) && !paused)
        {
            paused = true;
            PauseGame(paused);
        }
        else if (Input.GetKeyDown(KeyCode.P) && paused)
        {
            paused = false;
            PauseGame(paused);
        }
    }

    public void PauseGame(bool p)
    {
        if (p)
        {
            Time.timeScale = 0f;
            pauseFilter.enabled = true;
            paused = true;
            gameController.OnPaused(paused);
            for (int i = 0; i < pauseButtons.Length; i++) pauseButtons[i].enabled = true;
        }
        else
        {
            Time.timeScale = 1f;
            pauseFilter.enabled = false;
            paused = false;
            gameController.OnPaused(paused);
            for (int i = 0; i < pauseButtons.Length; i++) pauseButtons[i].enabled = false;
        }
    }
}
