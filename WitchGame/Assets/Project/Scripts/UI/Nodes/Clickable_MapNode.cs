using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_MapNode : Clickable
{
    public GameObject youAreHere;
    bool toggle = true;

    public override void OnClick()
    {
        //getting the transform component on the map indicator
        Transform transform;
        transform = youAreHere.GetComponent<Transform>();

        //giant gross switch here- jesus 
        switch (GameController.Main.roomName)
        {
            case "start":
                transform.localPosition = new Vector3(-7.95f, -2.85f, 0);
                break;
                //Fungus Forest Coords - DONE
            case "FF2":
                transform.localPosition = new Vector3(-6.9f, -2.85f, 0);
                break;
            case "FF3":
                transform.localPosition = new Vector3(-5.9f, -2.85f, 0);
                break;
            case "FF4":
                transform.localPosition = new Vector3(-4.8f, -2.85f, 0);
                break;
            case "FF5":
                transform.localPosition = new Vector3(-3.7f, -2.85f, 0);
                break;
            case "FF6":
                transform.localPosition = new Vector3(-2.7f, -2.85f, 0);
                break;
            case "FF7":
                transform.localPosition = new Vector3(-7.95f, -3.85f, 0);
                break;
                //Fountain's Edge Coords - DONE
            case "FE1":
                transform.localPosition = new Vector3(-1.75f, -2.85f, 0);
                break;
            case "FE2":
                transform.localPosition = new Vector3(-0.9f, -2.85f, 0);
                break;
            case "FE3":
                transform.localPosition = new Vector3(0.04f, -2.85f, 0);
                break;
                //Bramble Maze Coords - DONE
            case "BM1":
                transform.localPosition = new Vector3(-5.9f, -3.85f, 0);
                break;
            case "BM2":
                transform.localPosition = new Vector3(-5.9f, -4.9f, 0);
                break;
            case "BM3":
                transform.localPosition = new Vector3(-4.8f, -4.9f, 0);
                break;
            case "BM4":
                transform.localPosition = new Vector3(-3.7f, -4.9f, 0);
                break;
            case "BM5":
                transform.localPosition = new Vector3(-2.7f, -4.9f, 0);
                break;
            case "BM6":
                transform.localPosition = new Vector3(-1.75f, -4.9f, 0);
                break;
            case "BM7":
                transform.localPosition = new Vector3(-2.7f, -3.85f, 0);
                break;
            case "BM8":
                transform.localPosition = new Vector3(-1.75f, -3.85f, 0);
                break;
            case "BM9":
                transform.localPosition = new Vector3(-3.7f, -3.85f, 0);
                break;
            case "BM10":
                transform.localPosition = new Vector3(-4.8f, -3.85f, 0);
                break;
            //Brightwood Corridor Coords - DONE
            case "BC1":
                transform.localPosition = new Vector3(-3.7f, -1.95f, 0);
                break;
            case "BC2":
                transform.localPosition = new Vector3(-4.8f, -1.95f, 0);
                break;
            case "BC3":
                transform.localPosition = new Vector3(-5.9f, -1.95f, 0);
                break;
            case "BC4":
                transform.localPosition = new Vector3(-5.9f, -1f, 0);
                break;
            case "BC5":
                transform.localPosition = new Vector3(-6.9f, -1.95f, 0);
                break;
            case "BC6":
                transform.localPosition = new Vector3(-7.95f, -1.95f, 0);
                break;
                //Outside Bounds (Secret Rooms, Glitching into Midquarter Rooms)
            default:
                transform.localPosition = new Vector3(6f, -2.85f, 0); //Completely off screen
                break;
        }

        //copied over from toggle basically with the same function
        if (toggle == true)
        {
            youAreHere.SetActive(true);
            toggle = false;
        }
        else
        {
            youAreHere.SetActive(false);
            toggle = true;
        }
    }

}
