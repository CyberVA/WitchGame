using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_ToggleNode : Clickable
{
    public Color highlightColor;
    public GameObject tog;
    bool toggle = true;

    public override void OnClick()
    {
        SpriteRenderer sr;
        sr = tog.GetComponent<SpriteRenderer>();
        if (toggle == true)
        {
            //Debug.Log("i got pressed as true and am now turning false");
            sr.enabled = true;
            toggle = false;
        }
        else
        {
            //Debug.Log("i got pressed as false and am now turning true");
            sr.enabled = false;
            toggle = true;
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
