using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public bool paused = false;
    GameController gameController;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P) && !paused)
        {
            Time.timeScale = 0f;
            paused = true;
            gameController.OnPaused(paused);
        }
        else if (Input.GetKeyDown(KeyCode.P) && paused)
        {
            Time.timeScale = 1f;
            paused = false;
            gameController.OnPaused(paused);
        }
    }
}
