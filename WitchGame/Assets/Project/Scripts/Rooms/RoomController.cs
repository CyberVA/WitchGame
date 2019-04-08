using UnityEngine;
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
    //Editor Data
    public GridTransform gridInfo;
    public TileSet tileSet;
    int width;
    int height;

    public bool showBoxes;
    public Material glMaterial;
    public Color glColor;

    //Runtime Data
    [NonSerialized]
    public List<Box> staticColliders = new List<Box>();
    SpriteRenderer[] sprites;
    
    public void Setup(int w, int h)
    {
        width = w;
        height = h;
        GridPos p = new GridPos(0, 0);
        sprites = new SpriteRenderer[width * height];
        GameObject obj;
        while (p.y < height)
        {
            while (p.x < width)
            {
                obj = new GameObject("Tile");
                obj.transform.SetParent(transform);
                obj.transform.position = gridInfo.GetGridVector(p);
                sprites[p.GetIndex(width)] = obj.AddComponent<SpriteRenderer>();
                p.x++;
            }
            p.y++;
            p.x = 0;
        }
    }
    public void UpdateTiles(Room room)
    {
        GridPos p = new GridPos(0, 0);
        staticColliders.Clear();
        int i;
        //counter for colliders placed consecutively
        int colStack;
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
                    if(colStack == 0)
                    {
                        staticColliders.Add(new Box(gridInfo.GetGridVector(p), 1f, 1f));
                    }
                    else
                    {
                        //combine consecutive colliders
                        Box box = staticColliders[staticColliders.Count - 1];
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

                //iterate
                p.x++;
            }
            p.y++;
            p.x = 0;
        }
        IEnumerator<Decoration> enumerator = room.GetDecorations();
        Decoration dec;
        while(enumerator.MoveNext())
        {
            dec = enumerator.Current;
            CreateDecoration(new Vector2(dec.x, dec.y), dec.value);
        }
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
        staticColliders.Add(new Box(gridInfo.GetGridVector(p), 1f, 1f));
    }
    public void RemoveBox(GridPos p)
    {
        Box m = new Box(gridInfo.GetGridVector(p), 1f, 1f);
        staticColliders.Remove(m);
    }

    public void CreateDecoration(Vector2 v, byte value)
    {
        GameObject obj = new GameObject();
        obj.AddComponent<SpriteRenderer>().sprite = tileSet[value];
        obj.transform.position = v;
    }
    public void AddSpecialObject(GridPos p, byte value)
    {
        switch (value)
        {
            case 0: //nothing
                break;
            case 1: //something

                break;
            case 2: //something else

                break;
        }

    }

    public void Rotate(GridPos p, Room room)
    {
        int r = room.GetValue(p, Layer.Rotation);
        r = (r + 1) % 4;
        sprites[p.GetIndex(room.width)].transform.rotation = Quaternion.Euler(0f, 0f, 90f * r);
        room.SetValue(p, Layer.Rotation, (byte)r);
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

    private void OnRenderObject()
    {
        if(showBoxes)
        {
            BeginBoxesGL(glMaterial, glColor);
            if (staticColliders != null)
            {
                foreach (Box box in staticColliders)
                {
                    DrawBoxGL(box);
                }
            }
            EndBoxesGL();
        }
    }
}
