using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_ToggleNode : Clickable
{
    public Color highlightColor;
    public GameObject tog;
    bool toggle = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            OnClick();
        }
    }

    public override void OnClick()
    {
        if (toggle == true)
        {
            //Debug.Log("i got pressed as true and am now turning false");
            tog.SetActive(true);
            toggle = false;
        }
        else
        {
            //Debug.Log("i got pressed as false and am now turning true");
            tog.SetActive(false);
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
