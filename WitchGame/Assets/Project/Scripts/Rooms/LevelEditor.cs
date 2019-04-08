using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{
    //Editor References
    public RoomController roomController;
    public SpriteRenderer brushDisplay;

    //Editor Settings
    public string levelName;
    public int width, height;

    //Auto References
    SpriteRenderer spriteRenderer;
    GridTransform gridInfo;

    //Data
    Room loadedRoom;
    bool roomLoaded = false;
    byte brush;

    //Update Logic
    GridPos mp, prevMousePos;

    private void Awake()
    {
        //Grab References
        spriteRenderer = GetComponent<SpriteRenderer>();
        gridInfo = roomController.gridInfo;

        //Generate Tiles
        roomController.Setup(width, height);
    }

    private void Start()
    {
        SetBrush(0);
    }

    [ContextMenu("Load File")]
    private void Load()
    {
        loadedRoom = roomController.LoadRoom(levelName);
        roomController.UpdateTiles(loadedRoom);
        roomLoaded = true;
    }
    private void Load(string lvlName)
    {
        levelName = lvlName;
        Load();
    }

    [ContextMenu("Save File")]
    private void Save()
    {
        Save(levelName);
    }
    private void Save(string lvlName)
    {
        roomController.SaveAs(lvlName, loadedRoom);
    }

    void Update()
    {
        if (roomLoaded)
        {
            mp = gridInfo.GetGridPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (roomController.Inbounds(mp))
            {
                spriteRenderer.enabled = true;
                transform.position = gridInfo.GetGridVector(mp);

                //Commands executed on grid
                if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse0) && mp != prevMousePos)
                {
                    loadedRoom.SetValue(mp, Layer.Tex, brush);
                    roomController.SetTex(mp, brush);
                }
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    SetBrush(loadedRoom.GetValue(mp, Layer.Tex));
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    roomController.Rotate(mp, loadedRoom);
                }
                if (Input.GetKeyDown(KeyCode.Mouse2))
                {
                    if (loadedRoom.GetValue(mp, Layer.Collision) == 0)
                    {
                        loadedRoom.SetValue(mp, Layer.Collision, 1);
                        roomController.AddBox(mp);
                    }
                    else
                    {
                        loadedRoom.SetValue(mp, Layer.Collision, 0);
                        roomController.RemoveBox(mp);
                    }
                }
            }
            else
            {
                spriteRenderer.enabled = false;
            }
        }

        //Other Commands
        if (Input.mouseScrollDelta.y > 0)
        {
            if (brush < roomController.tileSet.TileCount - 1)
            {
                SetBrush((byte)(brush + 1));
            }
            else
            {
                SetBrush(0);
            }
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            if (brush != 0)
            {
                SetBrush((byte)(brush - 1));
            }
            else
            {
                SetBrush((byte)(roomController.tileSet.TileCount - 1));
            }
        }

        //Room travel
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Load(loadedRoom.exitNorth);
            roomController.UpdateTiles(loadedRoom);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Load(loadedRoom.exitSouth);
            roomController.UpdateTiles(loadedRoom);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Load(loadedRoom.exitWest);
            roomController.UpdateTiles(loadedRoom);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Load(loadedRoom.exitEast);
            roomController.UpdateTiles(loadedRoom);
        }

        //End of update
        prevMousePos = mp;
    }

    void OnGUI()
    {
        const int pad = 22;
        int count = 0;
        GUI.Label(new Rect(10, count++ * pad, 100, 20), "Level Name");
        levelName  = GUI.TextField(new Rect(10, count++ * 22, 100, 20), levelName, 25);
        if (roomLoaded)
        {
            GUI.Label(new Rect(10, count++ * pad, 100, 20), "North Exit");
            loadedRoom.exitNorth = GUI.TextField(new Rect(10, count++ * pad, 100, 20), loadedRoom.exitNorth, 25);
            GUI.Label(new Rect(10, count++ * pad, 100, 20), "South Exit");
            loadedRoom.exitSouth = GUI.TextField(new Rect(10, count++ * pad, 100, 20), loadedRoom.exitSouth, 25);
            GUI.Label(new Rect(10, count++ * pad, 100, 20), "East Exit");
            loadedRoom.exitEast = GUI.TextField(new Rect(10, count++ * pad, 100, 20), loadedRoom.exitEast, 25);
            GUI.Label(new Rect(10, count++ * pad, 100, 20), "West Exit");
            loadedRoom.exitWest = GUI.TextField(new Rect(10, count++ * pad, 100, 20), loadedRoom.exitWest, 25);

            if (GUI.Button(new Rect(10, count++ * pad, 100, 20), "Save"))
            {
                Save();
            }
            if (GUI.Button(new Rect(10, count++ * pad, 100, 20), "Reload"))
            {
                roomController.UpdateTiles(loadedRoom);
            }
        }
        if (GUI.Button(new Rect(10, count++ * pad, 100, 20), "Load"))
        {
            Load();
        }

    }

    public void SetBrush(byte value)
    {
        brush = value;
        brushDisplay.sprite = roomController.tileSet[value];
    }

}
