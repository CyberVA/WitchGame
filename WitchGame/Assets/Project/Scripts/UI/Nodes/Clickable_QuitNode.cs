using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_QuitNode : Clickable
{
    public override void OnClick()
    {
        Application.Quit();
    }
    public override void OnHover()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
    }
    public override void OnEmpty()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
