using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_UnpauseNode : Clickable
{
    public Color highlightColor;

    public override void OnClick()
    {
        GameController.Main.pause.paused = false;
        GameController.Main.pause.PauseGame(GameController.Main.pause.paused);
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
