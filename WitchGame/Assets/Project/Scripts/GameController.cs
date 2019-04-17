using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController instance;
    public static GameController Main
    {
        get
        {
            return instance;
        }
    }

    public RoomController roomController;
    public string roomName;
    public Room currentRoom;
    public Player player;
    public StatusBars statusBars;

    public void Awake()
    {
        instance = this;

        LoadMain();
    }

    #region Room Loading
    private void LoadMain()
    {
        currentRoom = roomController.LoadRoom(roomName);
        roomController.gridInfo.SetOffset(currentRoom.width, currentRoom.height);
        roomController.Setup(currentRoom.width, currentRoom.height);
        roomController.UpdateTiles(currentRoom, false);
        roomController.Position = Vector2.zero;
    }

    private void Load(string lvlName)
    {
        roomName = lvlName;
        currentRoom = roomController.LoadRoom(roomName);
        roomController.UpdateTiles(currentRoom, false);
    }

    public void LoadNorth()
    {
        Load(currentRoom.exitNorth);
    }
    public void LoadSouth()
    {
        Load(currentRoom.exitSouth);
    }
    public void LoadEast()
    {
        Load(currentRoom.exitEast);
    }
    public void LoadWest()
    {
        Load(currentRoom.exitWest);
    }
    #endregion
}
