using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController instance; //global instance
    public static GameController Main //public accessor for global instance
    {
        get
        {
            return instance;
        }
    }
    
    //Editor References
    public Player player;
    public StatusBars statusBars;
    public CombatSettings combatSettings;
    public Grid grid;
    public RoomController roomController;

    //Runtime References
    public Room currentRoom;

    //Editor + Runtime values
    public string roomName;

    public void Awake()
    {
        instance = this;

        LoadMain();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("Door in room " + roomName + " unlocked");
            roomController.UnlockDoor(roomName);
        }
    }

    #region Room Loading
    private void LoadMain()
    {
        //init roomcontroller first for layering reasons
        roomController.Init();

        //load first level
        currentRoom = roomController.LoadRoom(roomName);

        //Setup room dimensions + renderers for room size
        roomController.gridInfo.SetOffset(currentRoom.width, currentRoom.height);
        roomController.Setup(currentRoom.width, currentRoom.height);
        roomController.Position = Vector2.zero;

        //update renderers + pathfinding
        UpdateRoom();
    }

    private void Load(string lvlName)
    {
        roomName = lvlName;
        currentRoom = roomController.LoadRoom(roomName);
        UpdateRoom();
    }
    private void UpdateRoom()
    {
        roomController.UpdateTiles(currentRoom, roomName, false);
        grid.CreateGrid();
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
