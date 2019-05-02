using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_RootSnare : Clickable
{

    Timer timer;

    int speed = 10;
    public bool rootSnareActive;


    public override void OnClick()
    {
        if(rootSnareActive)
        {

            speed = 1;
        }
    }
    public override void OnHover()
    {
        if(rootSnareActive)
        {

        }
    }
}
