using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Functionality should be moved to gamecontroller eventually
/// </summary>
public class LoadOnAwake : MonoBehaviour
{
    public RoomController roomController;
    public string roomName;
    public Room room;

    private void Awake()
    {
        room = roomController.LoadRoom(roomName);
        roomController.Setup(room.width, room.height);
        roomController.UpdateTiles(room);
    }

    private void Load(string lvlName)
    {
        roomName = lvlName;
        room = roomController.LoadRoom(roomName);
        roomController.UpdateTiles(room);
    }

    public void LoadNorth()
    {
        Load(room.exitNorth);
    }
    public void LoadSouth()
    {
        Load(room.exitSouth);
    }
    public void LoadEast()
    {
        Load(room.exitEast);
    }
    public void LoadWest()
    {
        Load(room.exitWest);
    }
}
