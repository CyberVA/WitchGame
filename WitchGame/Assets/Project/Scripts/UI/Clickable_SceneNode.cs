using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Clickable_SceneNode : Clickable
{

    public string scenes;

    public override void OnClick()
    {
        SceneManager.LoadScene(scenes);
    }
}
