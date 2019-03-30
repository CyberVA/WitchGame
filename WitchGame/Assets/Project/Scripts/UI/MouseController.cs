using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseController : MonoBehaviour
{
    //VARIABLES

    //Misc
    public Camera mainCam;
    public Vector2 mousePos;

    //Arrays
    public Clickable[] clickables = new Clickable[3];

    //Booleans
    bool hoveredLastFrame;
    bool clickedLastFrame;


    //FUNCTIONS

    private void Start()
    {
        // SceneManager.sceneLoaded += OnNewScene
        clickables = FindObjectsOfType<Clickable>();
        mainCam = Camera.main;
    }
    /*
    * script checks array of colliders every frame, if mouse position hovers over collider in array,
    * script grabs information associated with collider
    */

    protected virtual void Update()
    {
        if (clickables != null)
        {
            for (int i = 0; i < clickables.Length; i++)
            {
                if (clickables[i].myBox.OverlapPoint(mainCam.ScreenToWorldPoint(Input.mousePosition)))
                {
                    clickables[i].OnHover();
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        clickables[i].OnClick();
                    }
                }
            }
        }

        else
        {
            Debug.LogWarning("Clickables un-defined");
        }

    }

    private void OnNewScene()
    {
        mainCam = Camera.main;
        clickables = FindObjectsOfType<Clickable>();
    }

    protected virtual void OnClick()
    {

    }
}
