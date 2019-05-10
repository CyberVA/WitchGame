using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_MapNode : Clickable
{
    public GameObject youAreHere;
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
    public void UpdateMarker()
    {
        //getting the transform component on the map indicator
        Transform transform;
        transform = youAreHere.transform;

        //giant gross switch here- jesus 
        switch (GameController.Main.roomName)
        {
            case "start":
                transform.localPosition = new Vector3(-3.7f, 0.15f, 0);
                break;
            //Fungus Forest Coords - DONE
            case "FF2":
                transform.localPosition = new Vector3(-2.7f, 0.15f, 0);
                break;
            //case "FF3":
            //    transform.localPosition = new Vector3(-1.7f, 0.15f, 0);
            //    break;
            case "FF4":
                transform.localPosition = new Vector3(-1.7f, 0.15f, 0);
                break;
            case "FF5":
                transform.localPosition = new Vector3(-0.6f, 0.15f, 0);
                break;
            //case "FF6":
            //    transform.localPosition = new Vector3(1.4f, 0.15f, 0);
            //    break;
            //case "FF7":
            //    transform.localPosition = new Vector3(-3.7f, -1f, 0);
            //    break;
            //Fountain's Edge Coords - DONE
            case "FE1":
                transform.localPosition = new Vector3(0.4f, 0.15f, 0);
                break;
            case "FE2":
                transform.localPosition = new Vector3(1.4f, 0.15f, 0);
                break;
            case "FE3":
                transform.localPosition = new Vector3(2.6f, 0.15f, 0);
                break;
            //Bramble Maze Coords - DONE
            case "BM1":
                transform.localPosition = new Vector3(-1.7f, -0.85f, 0);
                break;
            case "BM2":
                transform.localPosition = new Vector3(-1.7f, -1.8f, 0);
                break;
            case "BM3":
                transform.localPosition = new Vector3(-0.6f, -1.8f, 0);
                break;
            case "BM4":
                transform.localPosition = new Vector3(0.4f, -1.8f, 0);
                break;
            case "BM5":
                transform.localPosition = new Vector3(1.4f, -1.8f, 0);
                break;
            case "BM6":
                transform.localPosition = new Vector3(2.6f, -1.8f, 0);
                break;
            case "BM7":
                transform.localPosition = new Vector3(1.4f, -0.85f, 0);
                break;
            case "BM8":
                transform.localPosition = new Vector3(2.6f, -0.85f, 0);
                break;
            case "BM9":
                transform.localPosition = new Vector3(0.4f, -0.85f, 0);
                break;
            case "BM10":
                transform.localPosition = new Vector3(-0.6f, -0.85f, 0);
                break;
            //Brightwood Corridor Coords - DONE
            case "BC1":
                transform.localPosition = new Vector3(-0.6f, 1.15f, 0);
                break;
            case "BC2":
                transform.localPosition = new Vector3(-1.6f, 1.15f, 0);
                break;
            //case "BC3":
            //    transform.localPosition = new Vector3(-1.9f, 1.15f, 0);
            //    break;
            case "BC4":
                transform.localPosition = new Vector3(-1.6f, 2f, 0);
                break;
            case "BC5":
                transform.localPosition = new Vector3(-2.6f, 1.15f, 0);
                break;
            case "BC6":
                transform.localPosition = new Vector3(-3.75f, 1.15f, 0);
                break;
            //Outside Bounds (Secret Rooms, Glitching into Midquarter Rooms)
            default:
                transform.localPosition = new Vector3(10f, 10f, 0); //Completely off screen
                break;
        }
    }

}
