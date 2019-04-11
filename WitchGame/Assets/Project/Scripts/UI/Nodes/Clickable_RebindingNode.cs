using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_RebindingNode : Clickable
{

    public Color highlightColor;
    //public whatami- to dictate what keybind the button is currently assgined to change - just- Figure It Out


    private KeyCode newKey;

    private bool waitingForKey;


    public override void OnClick()
    {
        //dothatstuffdude
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
