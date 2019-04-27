﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using UnityEngine;


public class Pathfinding : MonoBehaviour
{

    PathRequestManager requestManager;

    //Reference to the grid script
    Grid grid;

    void Awake()
    {
        //defines requestManager as the PathRequestManager
        requestManager = GetComponent<PathRequestManager>();

        //defines grid as the Grid component of the attatched game object
        grid = GetComponent<Grid>();
    }

    /// <summary>
    /// Initiates FindPath and has it process over mutliple frames
    /// </summary>
    /// <param name="startPos">position of our unit</param>
    /// <param name="targetPos">position of our target</param>
    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    /// <summary>
    /// Calculates the path between a unit and a target
    /// </summary>
    /// <param name="startPos">Position of the unit</param>
    /// <param name="targetPos">Position of the target</param>
    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        //Array we'll be passing into our FinishedProcessingPath function in PathRequestManager
        Vector3[] waypoints = new Vector3[0];
        // Confirms if we actually found a path or not
        bool pathSuccess = false;

        //Defines the units vector2 position and the targets vector2 position as a node position in the grid
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        //Only processes a path if it's possible to make a path.
        if (startNode.walkable && targetNode.walkable)
        {
            //list of nodes that have yet to be processed as part of the path
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            //list of nodes that are already part of the path
            HashSet<Node> closedSet = new HashSet<Node>();
            //adds startNode to the open list
            openSet.Add(startNode);

            //loops until all tiles are nolonger in openSet
            while (openSet.Count > 0)
            {
                //removes the current node from the openSet.
                Node currentNode = openSet.RemoveFirst();

                //throws the node that was just processed into the closed set.
                closedSet.Add(currentNode);

                //if the currently processed node is the same node as the target.
                //this essentially means we've completed our path.
                if (currentNode == targetNode)
                {
                    //stops the stopwatch.
                    sw.Stop();
                    //prints the current time of the stopwatch.
                    print("Path found: " + sw.ElapsedTicks + "ticks");
                    //sets that we've completed processing our path.
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                    int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                    }
                }
            }
        }
        yield return null;
        //if finding a path was successfull
        if(pathSuccess)
        {
            //assigns out retraced path to waypoint
            waypoints = RetracePath(startNode, targetNode);
        }
        //Passes our waypoint into the PathRequestManager, also confirms success in finding a path.
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    /// <summary>
    /// Retraces the calculated path between two nodes
    /// </summary>
    /// <param name="startNode">The node where the unit is</param>
    /// <param name="endNode">The node where the target is </param>
    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        // Takes the simplified path and applies it to the waypoints
        Vector3[] waypoints = SimplifyPath(path);
        // Reverses the waypoints into a usable path
        Array.Reverse(waypoints);
        return waypoints;
    }

    /// <summary>
    /// Simplifies the path, places waypoints only where the path changes.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    Vector3[] SimplifyPath(List<Node> path)
    {
        // Makes a list of waypoints.
        List<Vector3> waypoints = new List<Vector3>();
        // Sets a new vector2 called directionOld to equal 0.
        Vector2 directionOld = Vector2.zero;
        
        // Runs for every path in the path array
        for(int i = 1; i < path.Count; i++)
        {
            // 
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if(directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }


    /// <summary>
    /// Grabs the distance between two nodes
    /// </summary>
    /// <param name="nodeA">the first node</param>
    /// <param name="nodeB">the second node</param>
    /// <returns></returns>
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY) return 14 * dstY + 10 * (dstX - dstY);
        else return 14 * dstX + 10 * (dstY - dstX);
    }

}