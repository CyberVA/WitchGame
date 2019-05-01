using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Grid : MonoBehaviour
{
    // Option to display the node grid or not
    public bool displayGrid;
    //Slider for adjusting grid transparency
    [Range(0, 1)]
    public float gizmoTransparency;
    //the node grid used by A*
    Node[,] grid;
    //Size of the grid
    GridTransform gridTransform;
    //Reference to the Room script
    Room room;

    //Size of the grid in integer value
    int gridSizeX, gridSizeY;

    /// <summary>
    /// Reference to the grids maximum size
    /// </summary>
    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
    }

    /// <summary>
    /// Function we call to create a new grid for the currently loaded room
    /// </summary>
    public void CreateGrid()
    {
        //Sets the nodes size to the tile sizes states in RoomController
        gridTransform = GameController.Main.roomController.gridInfo;
        //Sets room to the GameController
        room = GameController.Main.currentRoom;
        //Sets the grids width to the rooms width
        gridSizeX = room.width;
        //sets the grids height to the rooms height
        gridSizeY = room.height;

        grid = new Node[gridSizeX, gridSizeY];
        //For every point on the grid x axis...
        for (int x = 0; x < gridSizeX; x++)
        {
            //for every point on the grids y axis...
            for (int y = 0; y < gridSizeY; y++)
            {
                grid[x, y] = new Node(false, Vector3.zero, x, y);
            }
        }
    }
    /// <summary>
    /// Function called to update grid for a new room
    /// </summary>
    public void UpdateGrid()
    {
        room = GameController.Main.currentRoom;

        //For every point on the grid x axis...
        for (int x = 0; x < gridSizeX; x++)
        {
            //for every point on the grids y axis...
            for (int y = 0; y < gridSizeY; y++)
            {
                //Sets the position of the node
                grid[x, y].worldPosition = gridTransform.GetGridVector(new GridPos(x, y));
                //Determines if hte node is walkable
                grid[x, y].walkable = room.GetValue(x, y, Layer.Collision) == 0 && room.GetValue(x, y, Layer.Other) != 2; //2 = door
            }
        }
    }

    /// <summary>
    /// Gets the neighbor of the current node
    /// </summary>
    /// <param name="node">Current node</param>
    /// <returns>The neighbors of the current node</returns>
    public List<Node> GetNeighbours(Node node)
    {
        // Makes a new list of nodes called neighbors
        List<Node> neighbours = new List<Node>();

        // Runs 3 times, represents an x value
        for (int x = -1; x <= 1; x++)
        {
            // Runs 3 times, represents a y value
            for (int y = -1; y <= 1; y++)
            {
                //if x is equal to 0 and y is equal to 0, exit the loop
                if (x == 0 && y == 0) continue;

                //New integer we use for checking
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        //returns the neighbour
        return neighbours;
    }

    /// <summary>
    /// Gets a node from a set of coordinates
    /// </summary>
    /// <param name="worldPosition">Coordinates we want to find a node from</param>
    /// <returns></returns>
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        //associates p with the world position of the node
        GridPos p = gridTransform.GetGridPos(worldPosition);
        //returns P
        return grid[p.x, p.y];
    }

    /// <summary>
    /// Handles displaying the grid
    /// </summary>
    void OnDrawGizmos()
    {
        // If our grid exists and displayGrid is true
        if (grid != null && displayGrid)
        {
            //For every node in the grid
            foreach (Node n in grid)
            {
                //Color the walkable nodes white and the unwalkable nodes red
                Gizmos.color = (n.walkable) ? Color.white - new Color(0, 0, 0, gizmoTransparency) : Color.red - new Color(0, 0, 0, gizmoTransparency);
                //Draw the nodes as cubes
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (1 - .1f));
            }
        }
    }

}
