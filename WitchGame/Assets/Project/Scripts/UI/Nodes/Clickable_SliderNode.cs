using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_SliderNode : Clickable
{

    //References
    GameManager gMan;

    //Cameras
    public Camera mainCam;

    //Vectors
    public Vector3 pos;
    Vector2 mousePoint;

    //Booleans
    public bool isClicked = false;
    public bool isHover;
    bool wasClicked;
    bool isMouseDown;

    //Floats
    public float lowerLimit = 0;
    public float upperLimit = 2.5f;
    public float output;

    //GameObjects
    public GameObject fillBar;


    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        pos = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        mousePoint = mainCam.ScreenToWorldPoint(Input.mousePosition);
        //fillBarSize = fillBar.GetComponent<SpriteRenderer>().size;
        if (Input.GetKeyDown(KeyCode.Mouse0)) isMouseDown = true;
        else if (Input.GetKeyUp(KeyCode.Mouse0)) isMouseDown = false;
        
        //if OnClickStay is activated, wasClicked becomes true
        if (isClicked) wasClicked = true;
        //If left click is being held down and OnClickStay was clicked, set sliders position to mouse x position
        if (wasClicked && isMouseDown)
        {
            if (fillBar.GetComponent<SpriteRenderer>().size.x > lowerLimit && fillBar.GetComponent<SpriteRenderer>().size.x < upperLimit)
            {
                gameObject.transform.position = new Vector2(mousePoint.x, pos.y);
                fillBar.GetComponent<SpriteRenderer>().size = new Vector2(mousePoint.x - fillBar.transform.position.x, 1);
            }
        }
        if (fillBar.GetComponent<SpriteRenderer>().size.x < lowerLimit)
        {
            Debug.Log("UnderBound");
            fillBar.GetComponent<SpriteRenderer>().size = new Vector2(lowerLimit + 0.0001f, 1);
            gameObject.transform.localPosition = new Vector2(-1.25f, 0);
        }
        else if (fillBar.GetComponent<SpriteRenderer>().size.x > upperLimit)
        {
            Debug.Log("OverBound");
            fillBar.GetComponent<SpriteRenderer>().size = new Vector2(upperLimit - 0.0001f, 1);
            gameObject.transform.localPosition = new Vector2(1.25f, 0);
        }

        output = fillBar.GetComponent<SpriteRenderer>().size.normalized.x;

        //If left click is not being held down, wasClicked is nolonger true; 
        if (!isMouseDown) wasClicked = false;

    }

    public override void OnClickStay()
    {
        isClicked = true;
        Debug.Log("isClicked: " + isClicked);
    }
    public override void OnHover()
    {
        isClicked = false;
    }
    public override void OnEmpty()
    {
        isClicked = false;
    }
}
