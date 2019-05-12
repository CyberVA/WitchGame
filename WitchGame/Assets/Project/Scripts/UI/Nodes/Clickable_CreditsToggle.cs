using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_CreditsToggle : Clickable
{
    //I WAS going to do some animator shenanigans but then I realized this was so much easier- plus its only for Soup.
    //If we plan to continue this project one day, I'll make a non-hacky version of this. Right now I feel pressed for time.

    public Color highlightColor;
    public GameObject tog;


    public override void OnClick()
    {
        if (tog.activeSelf == true)
        {
            //Debug.Log("i got pressed as true and am now turning false");
            tog.SetActive(false);
        }
        else
        {
            //Debug.Log("i got pressed as false and am now turning true");
            tog.SetActive(true);
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
