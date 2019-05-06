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
    public PathRequestManager requestManager;
    public Pathfinding pathfinding;
    public RoomController roomController;
    public PixPerf pixelPerfect;
    public PixelPerfectUIAnchorManager uiAnchorManager;
    public Pause pause;

    //Runtime References
    public Room currentRoom;

    //Editor + Runtime values
    public string roomName;

    //Resolution Settings
    public int targetResolutionY = 360;
    public int scale = 1;
    public int maxScale;

    public void Awake()
    {
        instance = this;
        //initialize pathfinding systems
        pathfinding.Initialize();
        requestManager.Initialize();
        //Setup Game world
        LoadMain();

        //Screen Scaling
        pixelPerfect.Init();
        uiAnchorManager.Init();

        maxScale = pixelPerfect.GetMaxScale(targetResolutionY); //gets largest viable pixel scaler for current resolution
        scale = maxScale;
        pixelPerfect.SetScale(scale); //sets camera size for current scale
        uiAnchorManager.UpdateAnchors();

        //pause script
        pause.paused = false; //unpauses the game when you start the game

#if UNITY_EDITOR
        pixelPerfect.FixViewport(); //deal with odd-numbered resolutions if in editor
#endif
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("Door in room " + roomName + " unlocked");
            roomController.UnlockDoor();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            scale = Mathf.Min(scale + 1, maxScale);
            pixelPerfect.SetScale(scale);
            uiAnchorManager.UpdateAnchors();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            scale = Mathf.Max(scale - 1, 1);
            pixelPerfect.SetScale(scale);
            uiAnchorManager.UpdateAnchors();
        }
    }

    public void OnPaused(bool p)
    {

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

        //Create pathfinding grid
        grid.CreateGrid();

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
        grid.UpdateGrid(); //create pathfinding grid for current room
        roomController.UpdateWorld(currentRoom, roomName, false); //update tiles, spawn enemies, other shit
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
