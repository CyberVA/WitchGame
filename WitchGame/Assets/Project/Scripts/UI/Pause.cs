using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public bool paused = false;
    GameController gameController;
    public SpriteRenderer pauseFilter;

    private void Start()
    {
        gameController = GetComponent<GameController>();
        pauseFilter.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P) && !paused)
        {
            Time.timeScale = 0f;
            pauseFilter.enabled = true;
            paused = true;
            gameController.OnPaused(paused);
        }
        else if (Input.GetKeyDown(KeyCode.P) && paused)
        {
            Time.timeScale = 1f;
            pauseFilter.enabled = false;
            paused = false;
            gameController.OnPaused(paused); 
        }
    }
}
