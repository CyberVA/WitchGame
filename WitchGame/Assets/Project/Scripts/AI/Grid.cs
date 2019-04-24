using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
    

public class Grid : MonoBehaviour
{
    Node[,] grid;
    
    GridTransform gridTransform;
    Room room;

    int gridSizeX, gridSizeY;
        
    public void CreateGrid()
    {
        gridTransform = GameController.Main.roomController.gridInfo;
        room = GameController.Main.currentRoom;
        gridSizeX = room.width;
        gridSizeY = room.height;

        grid = new Node[gridSizeX, gridSizeY];

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

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }


    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        GridPos p = gridTransform.GetGridPos(worldPosition);
        return grid[p.x, p.y];
    }

    public List<Node> path;
    void OnDrawGizmos()
    {
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white - new Color(0, 0, 0, 0.5f) : Color.red - new Color(0, 0, 0, 0.5f);
                if (path != null)
                    if (path.Contains(n))
                        Gizmos.color = Color.black;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (1 - .1f));
            }
        }
    }

}
