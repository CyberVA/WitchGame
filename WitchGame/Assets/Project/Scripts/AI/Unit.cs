using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Position of our target
    public Transform target;
    // The units Speed
    public float speed = 20;
    // The path associated with this unit
    Vector3[] path;
    //index of our target
    int targetIndex;

    void Awake()
    {
        // Applies the player to the target variable
        target = GameController.Main.player.transform;
        // Requests a path from the PathRequestManager
        PathRequestManager.RequestPath(transform.position, GameController.Main.player.pos, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccesfull)
    {
        if(pathSuccesfull)
        {
            //Sets path to the new Path
            path = newPath;
            //Sets the targets index to 0
            targetIndex = 0;
            // Stops the Coroutine in case it's already running
            StopCoroutine("FollowPath");
            // Starts Coroutine that will get our character to begin following the path
            StartCoroutine("FollowPath");
        }
    }

    /// <summary>
    /// Makes the enemy actually move along the path
    /// </summary>
    /// <returns></returns>
    IEnumerator FollowPath()
    {
        // Assigns path to currentWaypoint
        Vector3 currentWaypoint = path[0];
        while(true)
        {
            // If the positon of our unit is the same position as it's target
            if(transform.position == currentWaypoint)
            {
                // Increments the target index
                targetIndex ++;
                // If the targetIndex is greater than the length of our path, exit Coroutine
                if(targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            //Moves the unit towards the waypoint
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    /// <summary>
    /// Handles drawing the path
    /// </summary>
    public void OnDrawGizmos()
    {
        //If the path exists and we want to display the grid
        if (path != null && GameController.Main.grid.displayGrid)
        {
            //for every node that is in path
            for (int i = targetIndex; i < path.Length; i++)
            {
                //Sets the color of the path nodes to black
                Gizmos.color = new Color(255, 255, 255, GameController.Main.grid.gizmoTransparency);
                //Draws our path nodes as a cube
                Gizmos.DrawCube(path[i], Vector3.one);

                //If the node is part of the target index
                if (i == targetIndex)
                {
                    //Draws a line to target
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    //Draws a line to the path
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
