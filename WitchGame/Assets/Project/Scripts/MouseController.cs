using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MouseController : MonoBehaviour
{
    //VARIABLES

    //Misc
    public Camera mainCam;
    public Vector2 mousePos;
    //public ButtonAction action;

    //Arrays
    public BoxCollider2D[] buttons = new BoxCollider2D[3];

    //Booleans
    bool hoveredLastFrame;
    bool clickedLastFrame;


    //FUNCTIONS

    private void Start()
    {
        for(int i = 0; i <= buttons.Length; i ++)
        {
            if (!buttons[i]) buttons[i] = GetComponent<BoxCollider2D>();
        }
    }
    /*
    * script checks array of colliders every frame, if mouse position hovers over collider in array,
    * script grabs information associated with collider
    */

    protected virtual void Update()
    {
        for(int i = 0; i <= buttons.Length; i++)
        {
            if (buttons[i].OverlapPoint(mainCam.ScreenToWorldPoint(Input.mousePosition)))
            {
                hoveredLastFrame = true;
                OnHover();
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    OnClick();
                }
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    clickedLastFrame = true;
                    OnClickHeld();
                }
                else if (clickedLastFrame)
                {
                    clickedLastFrame = false;
                    OnClickExit();
                }
            }
            else if (hoveredLastFrame)
            {
                hoveredLastFrame = false;
                OnHoverExit();
                clickedLastFrame = false;
                OnClickExit();
            }
        }
    }

    protected virtual void OnClick()
    {
    //    action.Exuecute();
    }

    protected abstract void OnHover();
    protected abstract void OnClickHeld();
    protected abstract void OnClickExit();
    protected abstract void OnHoverExit();
}
