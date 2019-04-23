using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Vector2 gridWorldSize;
    Node[,] grid;

    GridTransform gridTransform;
    Room room;
    
    int gridSizeX, gridSizeY;

    void Start()
    {
        CreateGrid();
    }
    
    void CreateGrid()
    {
        gridTransform = GameController.Main.roomController.gridInfo;
        room = GameController.Main.currentRoom;
        gridSizeX = room.width;
        gridSizeY = room.height;

        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for(int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = gridTransform.GetGridVector(new GridPos(x, y));
                bool walkable = room.GetValue(x, y, Layer.Collision) == 0;
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y));
        if(grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (1 - .1f));

            }
        }
    }

}
