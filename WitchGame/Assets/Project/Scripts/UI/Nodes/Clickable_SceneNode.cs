using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Clickable_SceneNode : Clickable
{

    public string scenes;
    public Color highlightColor;

    public override void OnClick()
    {
        if (scenes != null)
        {
            SceneManager.LoadScene(scenes);
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
