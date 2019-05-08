using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Clickable_SceneNode : Clickable
{

    GameManager gMan;

    double transparent;

    bool sceneChange;

    public string scenes;
    public Color highlightColor;

    private void Update()
    {
        if (sceneChange)
        {
            transparent +=  0.2 * Time.deltaTime;
            GameManager.gMan.fadeInOut.color = new Color(0f, 0f, 0f, (float)transparent);
            if (transparent >= 1)
            {
                sceneChange = false;
                transparent = 0;
                SceneManager.LoadScene(scenes);
                /*if (gMan.currentScene.name == "Game")
                {
                    GameManager.gMan.fadeInOut.color = new Color(0f, 0f, 0f, (float)transparent);
                }*/
            }
        }
    }

    public override void OnClick()
    {
        if (scenes != null)
        {
            sceneChange = true;
        }
        else
        {
            Debug.LogWarning("Scene undefined on " + gameObject.name);
        }
    }
    public override void OnHover()
    {
        gameObject.GetComponent<SpriteRenderer>().color = highlightColor;
    }
    public override void OnEmpty()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
