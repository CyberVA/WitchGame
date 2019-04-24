﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    //reference to our seeker and target
    public Transform seaker, target;

    //Reference to the grid script
    Grid grid;

    void Awake()
    {
        //defines grid as the Grid component of the attatched game object
        grid = GetComponent<Grid>();
    }

    void Update()
    {
        if(target != null && seaker != null) //fuck you vincent
        {
            //continuously finds the path between the seaker and the target
            FindPath(seaker.position, target.position);
        }
    }


    /// <summary>
    /// Calculates the path between a seeker and a target
    /// </summary>
    /// <param name="startPos">Position of the seeker</param>
    /// <param name="targetPos">Position of the target</param>
    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //Defines the seekers vector2 position and the targets vector2 position as a node position in the grid
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        //list of nodes that have yet to be processed as part of the path
        List<Node> openSet = new List<Node>();
        //list of nodes that are already part of the path
        HashSet<Node> closedSet = new HashSet<Node>();
        //adds startNode to the open list
        openSet.Add(startNode);

        //loops until all tiles are nolonger in openSet
        while (openSet.Count > 0)
        {
            Node node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                {
                    if (openSet[i].hCost < node.hCost) node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            if (node == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(node))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                }
            }
        }
    }

    /// <summary>
    /// Retraces the calculated path between two nodes
    /// </summary>
    /// <param name="startNode">The node where the seeker is</param>
    /// <param name="endNode">The node where the target is </param>
    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path;

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
