using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_RebindingNode : Clickable
{

    public string keyToChange;
    public Color highlightColor;

    private KeyCode newKey;

    private bool waitingForKey = false;


    public override void OnClick()
    {
        //TODO:
        //Enter a state where it starts looking for the next key press
        //when that (the key press) happens, assign the last key pressed to newKey
        //change the newKey to be the keycode used for the specified direction
        //exit the state of waiting for a keypress

        //the problem: how to get each button to recognize what it needs to change w/out cheesing it
        //InputManager.IM.(keyToChange) = newKey; doesn't work
        //heres the cheese: a string, and a switch on that sting. depending on the string the button has been assigned it changes the whatever blah blah AWFUL i'm doing it
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
