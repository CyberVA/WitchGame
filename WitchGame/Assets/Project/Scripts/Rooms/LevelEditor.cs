using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static SpclObj;

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

    //Update Logic
    GridPos mp, prevMousePos;

    //Properties
    GridTransform gridInfo => roomController.gridInfo;

    private void Awake()
    {
        roomController.Init();
    }
    private void Start()
    {
        SetBrush(0);
    }
    
    private void Load()
    {
        loadedRoom = roomController.LoadRoom(levelName);
        UpdateRoom();
        roomLoaded = true;
    }
    private void UpdateRoom()
    {
        roomController.UpdateWorld(loadedRoom, levelName, true);
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
            loadedRoom.SetValue(mp, Layer.Other, NOTHING);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            loadedRoom.SetValue(mp, Layer.Other, FOUNTAIN);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            loadedRoom.SetValue(mp, Layer.Other, ARMSHROOM);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            loadedRoom.SetValue(mp, Layer.Other, DOOR);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            loadedRoom.SetValue(mp, Layer.Other, KEY);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            loadedRoom.SetValue(mp, Layer.Other, GEBLIN);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            loadedRoom.SetValue(mp, Layer.Other, NOMOVEU);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            loadedRoom.SetValue(mp, Layer.Other, NOMOVED);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            loadedRoom.SetValue(mp, Layer.Other, NOMOVEL);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            loadedRoom.SetValue(mp, Layer.Other, NOMOVER);
        }

        //Room travel
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Load(loadedRoom.exitNorth);
            UpdateRoom();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Load(loadedRoom.exitSouth);
            UpdateRoom();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Load(loadedRoom.exitWest);
            UpdateRoom();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Load(loadedRoom.exitEast);
            UpdateRoom();
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
                    UpdateRoom();
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
                case FOUNTAIN: //fountain
                    SimpGL.DrawSprite(rect, 0.1f, 0f, 0.2f, 0.1f);
                    break;
                case DOOR: //door
                    SimpGL.DrawSprite(rect, 0.2f, 0f, 0.3f, 0.1f);
                    break;
                case KEY: //key
                    SimpGL.DrawSprite(rect, 0.3f, 0f, 0.4f, 0.1f);
                    break;
                case ARMSHROOM: //armshroom
                    SimpGL.DrawSprite(rect, 0f, 0f, 0.1f, 0.1f);
                    break;
                case GEBLIN: //goblin
                    SimpGL.DrawSprite(rect, 0.4f, 0f, 0.5f, 0.1f);
                    break;
                case SPODER: //spider
                    SimpGL.DrawSprite(rect, 0.5f, 0f, 0.6f, 0.1f);
                    break;
                case NOMOVEU:
                    SimpGL.DrawSprite(rect, 0.8f, 0f, 0.9f, 0.1f);
                    break;
                case NOMOVED:
                    SimpGL.DrawSprite(rect, 0.6f, 0f, 0.7f, 0.1f);
                    break;
                case NOMOVEL:
                    SimpGL.DrawSprite(rect, 0.7f, 0f, 0.8f, 0.1f);
                    break;
                case NOMOVER:
                    SimpGL.DrawSprite(rect, 0.9f, 0f, 1f, 0.1f);
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
