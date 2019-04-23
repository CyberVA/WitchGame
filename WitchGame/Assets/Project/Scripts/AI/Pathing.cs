using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathing : MonoBehaviour
{
    /*public GridPos AStar(GridPos source, GridPos target, Room map)
    {
        int mapWidth = map[0].Length;
        int mapHeight = map.Count;
        source.GetIndex(mapWidth);
        List<Vector3> pathGrid = new List<Vector3>();
        bool[,] checkedPath = new bool[mapWidth, mapHeight];
        pathGrid.Add(new Vector3(target.X, target.Y, 0));
        bool pathFound = false;
        int i = 0;
        while (i < pathGrid.Count)
        {
            //try not to die.txt
            //UP
            if (pathGrid[i].y > 0)
            {
                if (pathGrid[i].x == source.X && pathGrid[i].y - 1 == source.Y)
                {
                    checkedPath[(int)pathGrid[i].x, (int)pathGrid[i].y - 1] = true;
                    pathFound = true;
                    return new GridPos(0, 1);
                }
                else if (!checkedPath[(int)pathGrid[i].x, (int)pathGrid[i].y - 1] && map[(int)pathGrid[i].y - 1][(int)pathGrid[i].x] == '.')
                {
                    pathGrid.Add(new Vector3(pathGrid[i].x, pathGrid[i].y - 1, pathGrid[i].z + 1));
                    checkedPath[(int)pathGrid[i].x, (int)pathGrid[i].y - 1] = true;
                }
            }
            //RIGHT
            if (pathGrid[i].x < mapWidth - 1 && !pathFound)
            {
                if (pathGrid[i].x + 1 == source.X && pathGrid[i].y == source.y)
                {
                    checkedPath[(int)pathGrid[i].x + 1, (int)pathGrid[i].y] = true;
                    pathFound = true;
                    return new GridPos(-1, 0);
                }
                else if (!checkedPath[(int)pathGrid[i].x + 1, (int)pathGrid[i].y] && map[(int)pathGrid[i].y][(int)pathGrid[i].x + 1] == '.')
                {
                    pathGrid.Add(new Vector3(pathGrid[i].x + 1, pathGrid[i].y, pathGrid[i].z + 1));
                    checkedPath[(int)pathGrid[i].x + 1, (int)pathGrid[i].y] = true;
                }
            }
            //LEFT
            if (pathGrid[i].x > 0 && !pathFound)
            {
                /*if (pathGrid[i].x - 1 == source.x && pathGrid[i].y == source.Y)
                {
                    checkedPath[(int)pathGrid[i].x - 1, (int)pathGrid[i].y] = true;
                    pathFound = true;
                    return new GridPos(1, 0);
                }
                else if (!checkedPath[(int)pathGrid[i].x - 1, (int)pathGrid[i].y] && map[(int)pathGrid[i].y][(int)pathGrid[i].x - 1] == '.')
                {
                    pathGrid.Add(new Vector3(pathGrid[i].x - 1, pathGrid[i].y, pathGrid[i].Z + 1));
                    checkedPath[(int)pathGrid[i].x - 1, (int)pathGrid[i].y] = true;
                }
}

            //DOWN
            if (pathGrid[i].y < mapHeight - 1 && !pathFound)
            {
                if (pathGrid[i].x == source.x && pathGrid[i].y + 1 == source.y)
                {
                    checkedPath[(int)pathGrid[i].x, (int)pathGrid[i].y + 1] = true;
                    pathFound = true;
                    return new GridPos(0, -1);
                }
                else if (!checkedPath[(int)pathGrid[i].x, (int)pathGrid[i].y + 1] && map[(int)pathGrid[i].y + 1][(int)pathGrid[i].x] == '.')
                {
                    pathGrid.Add(new Vector3(pathGrid[i].x, pathGrid[i].y + 1, pathGrid[i].Z + 1));
                    checkedPath[(int)pathGrid[i].x, (int)pathGrid[i].y + 1] = true;
                }
            }

            //after down
            i++;
        }
        return new GridPos(0, 0);
    }*/
}
