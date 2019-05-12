using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// script checks array of colliders every frame, if mouse position hovers over collider in array,
/// script grabs information associated with collider
/// </summary>
public class MouseController : MonoBehaviour
{
    //VARIABLES

    //Misc
    public Camera mainCam;
    public Vector2 mousePos;

    //Arrays
    public Clickable[] clickables;

    //Booleans
    bool hoveredLastFrame;
    bool clickedLastFrame;


    //FUNCTIONS

    private void Start()
    {
        //reassignes clickables in scene to the clickables array
        clickables = FindObjectsOfType<Clickable>();
        
        //assigns the main camera in the scene to the mainCame variable
        mainCam = Camera.main;
    }

    protected virtual void Update()
    {

        //if the scene is any less than 2 scenes (the GameManager scene and the current loaded scene) then MouseController will...
        // ... Check for the camera and clickable objects again.
        // This is done so we have the latest main camera and clickable objects from each scene
        if (SceneManager.sceneCount < 2)
        {
            mainCam = Camera.main;
            clickables = FindObjectsOfType<Clickable>();
        }

        //Checks if there are clickables (more accurately, does something if clickables exist)
        if (clickables != null)
        {
            //For each clickable, do this...)
            for (int i = 0; i < clickables.Length; i++)
            {
                //if clickable I has a collider attached to it, then do this...
                if (clickables[i].myBox != null)
                {

                    //if the mouse position is hovering over the clickables collider, then do this...)
                    if (clickables[i].myBox.OverlapPoint(mainCam.ScreenToWorldPoint(Input.mousePosition)))
                    {
                        //calls OnHover
                        clickables[i].OnHover();
                        //Debug.Log("Hovering");
                        //this one too. sorry vince.
                        
                        //If player left clicks the mouse
                        if (Input.GetKeyDown(KeyCode.Mouse0)) clickables[i].OnClick();
                        //If the player is holding down the left click
                        if (Input.GetKey(KeyCode.Mouse0)) clickables[i].OnClickStay();
                        else if (Input.GetKeyUp(KeyCode.Mouse0)) clickables[i].OnClickStay();
                    }
                    else
                    {
                        clickables[i].OnEmpty();
                    }
                }
                //if clickable does not have a collider
                else if (clickables[i].myBox == null)
                {
                    //outputs warning that there are not any clickable box colliders.
                    Debug.LogWarning("No Clickable Box Colliders");
                }
            }
        }

        //if there are no existing clickables
        else
        {
            //outputs that there are no clickables, or at least defined clickables in the scene
            Debug.LogWarning("Clickables un-defined");
        }

    }

    protected virtual void OnClick()
    {

    }
}
