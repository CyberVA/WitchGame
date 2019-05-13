using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_SliderNode : Clickable
{

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

    //SpriteRenderers
    SpriteRenderer fillBarSize;


    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        pos = gameObject.transform.position;
        fillBarSize = fillBar.GetComponent<SpriteRenderer>();
        fillBarSize.size = new Vector2(GameManager.gMan.volume * upperLimit, 1);
        gameObject.transform.position = new Vector2(fillBar.transform.position.x + fillBarSize.size.x, pos.y);
    }

    // Update is called once per frame
    void Update()
    {
        pos = gameObject.transform.position;
        mousePoint = mainCam.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetKeyDown(KeyCode.Mouse0)) isMouseDown = true;
        else if (Input.GetKeyUp(KeyCode.Mouse0)) isMouseDown = false;
        
        //if OnClickStay is activated, wasClicked becomes true
        if (isClicked) wasClicked = true;
        //If left click is being held down and OnClickStay was clicked, set sliders position to mouse x position
        if (wasClicked && isMouseDown)
        {
            if (fillBarSize.size.x > lowerLimit && fillBarSize.size.x < upperLimit)
            {
                gameObject.transform.position = new Vector2(mousePoint.x, pos.y);
                fillBarSize.size = new Vector2(mousePoint.x - fillBar.transform.position.x, 1);
            }
        }
        if (fillBarSize.size.x < lowerLimit)
        {
            Debug.Log("UnderBound");
            fillBarSize.size = new Vector2(lowerLimit + 0.0001f, 1);
            gameObject.transform.localPosition = new Vector2(-1.25f, 0);
        }
        else if (fillBarSize.size.x > upperLimit)
        {
            Debug.Log("OverBound");
            fillBarSize.size = new Vector2(upperLimit - 0.0001f, 1);
            gameObject.transform.localPosition = new Vector2(1.25f, 0);
        }

        output = fillBarSize.size.x / upperLimit;

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
