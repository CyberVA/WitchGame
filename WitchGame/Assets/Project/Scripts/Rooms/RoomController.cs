﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using TwoStepCollision;
using static TwoStepCollision.Func;

/// <summary>
/// Creates and manages the runtime objects of a room
/// </summary>
public class RoomController : MonoBehaviour
{
    //Editor Ref
    public RoomPrefabs roomPrefabs;

    //Editor Values
    public GridTransform gridInfo;
    public TileSet tileSet;
    public bool showBoxes;

    //Runtime Values
    int width;
    int height;
    int spriteLayerId;
    bool init = false;
    /// <summary>
    /// solid objects in the room
    /// </summary>
    [NonSerialized]
    public List<Box> wallColliders = new List<Box>();
    /// <summary>
    /// enemies in the loaded room
    /// </summary>
    [NonSerialized]
    public List<IHurtable> enemies = new List<IHurtable>();
    /// <summary>
    /// tiles to remove when doors unlocked
    /// </summary>
    List<GridPos> doors = new List<GridPos>();
    /// <summary>
    /// box colliders for doors
    /// </summary>
    List<Box> doorBoxes = new List<Box>();
    /// <summary>
    /// set of room names where doors have been unlocked
    /// </summary>
    HashSet<string> unlockedDoors = new HashSet<string>();
    public HashSet<string> pickedUpKeys = new HashSet<string>();
    /// <summary>
    /// collider for fountain
    /// </summary>
    [NonSerialized]
    public Box winbox = null;
    /// <summary>
    /// collider for a key
    /// </summary>
    [NonSerialized]
    public Box keyBox = null;
    [NonSerialized]
    public GameObject keyObj = null;
    /// <summary>
    /// objects to be deleted when a new room is loaded
    /// </summary>
    [NonSerialized]
    public List<GameObject> removeOnLoad = new List<GameObject>();
    /// <summary>
    /// Box collider used to determine if something is within the room
    /// </summary>
    [NonSerialized]
    public Box roomBounds = new Box(0f, 0f, 0f, 0f);
    SpriteRenderer[] sprites;

    //Gizmos
    public Material glMaterial;
    public Color glColor;

    //Properties
    /// <summary>
    /// Sets positions all grid related objects/settings
    /// </summary>
    public Vector2 Position
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
            gridInfo.origin = value;
            roomBounds.Center = value;
        }
    }

    private void Awake()
    {
        if(!init) //init for scenes without GameController
        {
            Init();
        }
    }
    public void Init()
    {
        spriteLayerId = SortingLayer.NameToID("Tiles"); //get layerid used for tiles
        init = true;
    }

    public void Setup(int w, int h)
    {
        width = w;
        height = h;
        roomBounds.width = width * gridInfo.tileSize;
        roomBounds.height = height * gridInfo.tileSize;
        GridPos p = new GridPos(0, 0); //used for iteration and placement
        sprites = new SpriteRenderer[width * height];
        GameObject obj; //reference for current tile object
        while (p.y < height)
        {
            while (p.x < width)
            {
                obj = new GameObject("Tile");
                obj.transform.SetParent(transform);
                obj.transform.position = gridInfo.GetGridVector(p);
                sprites[p.GetIndex(width)] = obj.AddComponent<SpriteRenderer>();
                sprites[p.GetIndex(width)].sortingLayerID = spriteLayerId;
                p.x++;
            }
            p.y++;
            p.x = 0;
        }
    }
    public void UpdateTiles(Room room, string roomName, bool inEditor)
    {
        wallColliders.Clear(); //remove old wall colliders
        doors.Clear(); //clear old doorlist
        doorBoxes.Clear(); //remove old door colliders
        enemies.Clear(); //remove old enemies
        foreach (GameObject r in removeOnLoad) //destroy other objects in old room
        {
            Destroy(r);
        }
        removeOnLoad.Clear(); //reset list after objects destroyed
        if(!inEditor)
        {
            GameController.Main.player.checkWin = false;
            GameController.Main.player.checkKey = false;
        }
        if (keyObj != null) Destroy(keyObj);

        GridPos p = new GridPos(0, 0); //iterator/placement position
        int i; //one-dimensional index of iterator
        int colStack; //counter for colliders placed consecutively
        ArmShroom.layerOrder = 0; //reset layercounter for armshrooms
        ArmShroom.enemyCount = 0; //reset counter for armshrooms
        while (p.y < height)
        {
            colStack = 0;
            while (p.x < width)
            {
                i = p.GetIndex(width);
                //Tile textrure / rotation
                sprites[i].transform.rotation = Quaternion.Euler(0f, 0f, 90f * room.GetValue(p, Layer.Rotation));
                sprites[i].sprite = tileSet[room.GetValue(p, Layer.Tex)];
                //Collision
                if(room.GetValue(p, Layer.Collision) == 1)
                {
                    if(inEditor || colStack == 0) //new box
                    {
                        wallColliders.Add(new Box(gridInfo.GetGridVector(p), 1f, 1f));
                    }
                    else //extend previous box
                    {
                        //combine consecutive colliders
                        Box box = wallColliders[wallColliders.Count - 1];
                        box.x += 0.5f;
                        box.width = colStack + 1;
                    }
                    colStack++;
                }
                else
                {
                    colStack = 0;
                }
                //Other Objects
                if(!inEditor)
                {
                    AddSpecialObject(p, room.GetValue(p, Layer.Other), roomName);
                }

                //iterate
                p.x++;
            }
            p.y++;
            p.x = 0;
        }
        /*
        IEnumerator<Decoration> enumerator = room.GetDecorations();
        Decoration dec;
        while(enumerator.MoveNext())
        {
            dec = enumerator.Current;
            CreateDecoration(new Vector2(dec.x, dec.y), dec.value);
        }*/
    }
    public bool Inbounds(GridPos p)
    {
        return !(p.x < 0 || p.x >= width || p.y < 0 || p.y >= height);
    }

    public void SetTex(GridPos p, byte tex)
    {
        sprites[p.GetIndex(width)].sprite = tileSet[tex];
    }
    public void AddBox(GridPos p)
    {
        wallColliders.Add(new Box(gridInfo.GetGridVector(p), 1f, 1f));
    }
    public void RemoveBox(GridPos p)
    {
        Box m = new Box(gridInfo.GetGridVector(p), 1f, 1f);
        wallColliders.Remove(m);
    }
    public void UnlockDoor(string roomName)
    {
        unlockedDoors.Add(roomName);
        foreach(GridPos p in doors)
        {
            sprites[p.GetIndex(width)].sprite = tileSet[0];
        }
        doors.Clear();
    }

    public void CreateDecoration(Vector2 v, byte value)
    {
        GameObject obj = new GameObject();
        obj.AddComponent<SpriteRenderer>().sprite = tileSet[value];
        obj.transform.position = v;
    }
    public void AddSpecialObject(GridPos p, byte value, string roomName)
    {
        Vector2 v = gridInfo.GetGridVector(p);
        GameObject go;
        switch (value)
        {
            case 1: //fountain
                go = Instantiate(roomPrefabs.fountain);
                go.transform.position = v;
                winbox = new Box(v, 2f, 2f);
                GameController.Main.player.checkWin = true;
                removeOnLoad.Add(go);
                break;
            case 2: //door
                if(!unlockedDoors.Contains(roomName))
                {
                    doors.Add(p);
                    doorBoxes.Add(new Box(v, 1f, 1f));
                }
                else
                {
                    sprites[p.GetIndex(width)].sprite = tileSet[0];
                }
                break;
            case 3: //key
                if(!pickedUpKeys.Contains(roomName))
                {
                    go = Instantiate(roomPrefabs.key);
                    keyObj = go;
                    go.transform.position = v;
                    keyBox = new Box(v, 1f, 0.5f);
                    GameController.Main.player.checkKey = true;
                }
                break;
            case 50: //armShroom
                ArmShroom armShroom = Instantiate(roomPrefabs.armShroom, v, Quaternion.identity).GetComponent<ArmShroom>();
                armShroom.box.Center = v;
                removeOnLoad.Add(armShroom.gameObject);
                break;
        }

    }
    //File Management
    public void SaveAs(string fileName, Room room)
    {
        string path = Application.streamingAssetsPath + "/Rooms/" + fileName + ".beans";
        Byteable.IO.WriteToFile(room, path);
        Debug.Log("File Saved");
    }

    public Room LoadRoom(string fileName)
    {
        string path = Application.streamingAssetsPath + "/Rooms/" + fileName + ".beans";
        if(File.Exists(path))
        {
            try
            {
                Debug.Log("Level \"" + fileName + "\" loaded");
                return Byteable.IO.ReadFromFile<Room>(path);
            }
            catch(ArgumentOutOfRangeException)
            {
                Debug.LogError("Error reading file, creating new room");
                return new Room(width, height);
            }
        }
        else
        {
            Debug.Log("File not found, creating new room");
            return new Room(width, height);
        }
    }

    public IEnumerator<Box> GetStaticBoxes(bool doorsAreStatic)
    {
        if(doorsAreStatic && doors.Count > 0)
        {
            return new MultiEnumerator<Box>(wallColliders, doorBoxes);
        }
        else
        {
            return wallColliders.GetEnumerator();
        }
    }

    private void OnRenderObject()
    {
        if(showBoxes)
        {
            BeginBoxesGL(glMaterial, glColor);
            if (wallColliders != null)
            {
                foreach (Box box in wallColliders)
                {
                    DrawBoxGL(box);
                }
            }
            EndBoxesGL();
        }
    }
}
