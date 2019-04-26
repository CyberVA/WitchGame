using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{
    //Editor References
    public RoomController roomController;
    public SpriteRenderer brushDisplay;
    public SpriteRenderer highlight;
    public Transform editorCamera;

    //Editor Settings
    public string levelName;
    public int width, height;
    public float cameraSpeed;
    public Material specialObjects;

    //Auto References

    //Data
    Room loadedRoom;
    public bool drawSpecials;
    bool setup = false;
    bool roomLoaded = false;
    byte brush;
    byte spclBrush;

    //Update Logic
    GridPos mp, prevMousePos;

    //Properties
    GridTransform gridInfo => roomController.gridInfo;

    private void Start()
    {
        SetBrush(0);
    }
    
    private void Load()
    {
        loadedRoom = roomController.LoadRoom(levelName);
        roomController.UpdateTiles(loadedRoom, true);
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
                highlight.enabled = true;
                highlight.transform.position = gridInfo.GetGridVector(mp);

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
                highlight.enabled = false;
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            loadedRoom.SetValue(mp, Layer.Other, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            loadedRoom.SetValue(mp, Layer.Other, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            loadedRoom.SetValue(mp, Layer.Other, 50);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            loadedRoom.SetValue(mp, Layer.Other, 2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            loadedRoom.SetValue(mp, Layer.Other, 3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            loadedRoom.SetValue(mp, Layer.Other, 51);
        }

        //Room travel
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Load(loadedRoom.exitNorth);
            roomController.UpdateTiles(loadedRoom, true);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Load(loadedRoom.exitSouth);
            roomController.UpdateTiles(loadedRoom, true);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Load(loadedRoom.exitWest);
            roomController.UpdateTiles(loadedRoom, true);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Load(loadedRoom.exitEast);
            roomController.UpdateTiles(loadedRoom, true);
        }
        //Move Camera
        if (Input.GetKey(KeyCode.W))
        {
            editorCamera.Translate(0f, Time.deltaTime * cameraSpeed, 0f, Space.World);
        }
        if (Input.GetKey(KeyCode.A))
        {
            editorCamera.Translate(Time.deltaTime * -cameraSpeed, 0f, 0f, Space.World);
        }
        if (Input.GetKey(KeyCode.S))
        {
            editorCamera.Translate(0f, Time.deltaTime * -cameraSpeed, 0f, Space.World);
    }
        if (Input.GetKey(KeyCode.D))
        {
            editorCamera.Translate(Time.deltaTime * cameraSpeed, 0f, 0f, Space.World);
        }

        //End of update
        prevMousePos = mp;
    }

    void OnGUI()
    {
        const int pad = 22;
        int count = 0;
        if(setup)
        {
            GUI.Label(new Rect(10, count++ * pad, 100, 20), "Level Name");
            levelName = GUI.TextField(new Rect(10, count++ * 22, 100, 20), levelName, 25);
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
                    roomController.UpdateTiles(loadedRoom, true);
                }
            }
            if (GUI.Button(new Rect(10, count++ * pad, 100, 20), "Load"))
            {
                Load();
            }
        }
        else
        {
            GUI.Label(new Rect(10, count++ * pad, 100, 20), "Room Width");
            width = Convert.ToInt32(GUI.TextField(new Rect(10, count++ * pad, 100, 20), width.ToString(), 25));
            GUI.Label(new Rect(10, count++ * pad, 100, 20), "Room Height");
            height = Convert.ToInt32(GUI.TextField(new Rect(10, count++ * pad, 100, 20), height.ToString(), 25));
            if (GUI.Button(new Rect(10, count++ * pad, 100, 20), "Create Tiles"))
            {
                gridInfo.SetOffset(width, height);
                roomController.Setup(width, height);
                roomController.Position = Vector2.zero;
                setup = true;
            }
        }

    }

    private void OnRenderObject()
    {
        if(roomLoaded && drawSpecials)
        {
            specialObjects.SetPass(0);
            GL.Begin(GL.QUADS);
            GridPos p = new GridPos(0, 0);
            byte value;
            while (p.y < height)
            {
                while (p.x < width)
                {
                    value = loadedRoom.GetValue(p, Layer.Other);
                    if (value != 0)
                    {
                        Draw(value, gridInfo.GetRect(p));
                    }
                    p.x++;
                }
                p.y++;
                p.x = 0;
            }
            GL.End();
        }

        void Draw(byte v, Rect rect)
        {
            switch (v)
            {
                case 1: //fountain
                    SimpGL.DrawSprite(rect, 0.1f, 0f, 0.2f, 0.1f);
                    break;
                case 2: //door
                    SimpGL.DrawSprite(rect, 0.2f, 0f, 0.3f, 0.1f);
                    break;
                case 3: //key
                    SimpGL.DrawSprite(rect, 0.3f, 0f, 0.4f, 0.1f);
                    break;
                case 50: //armshroom
                    SimpGL.DrawSprite(rect, 0f, 0f, 0.1f, 0.1f);
                    break;
                case 51: //goblin
                    SimpGL.DrawSprite(rect, 0.4f, 0f, 0.5f, 0.1f);
                    break;
            }
        }
    }

    public void SetBrush(byte value)
    {
        brush = value;
        brushDisplay.sprite = roomController.tileSet[value];
    }

}
