using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Position of our target
    public Transform target;
    // The units Speed
    float speed = 5;
    // The path associated with this unit
    Vector3[] path;
    //index of our target
    int targetIndex;

    void Start()
    {
        // Requests a path from the PathRequestManager
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccesfull)
    {
        if(pathSuccesfull)
        {
            path = newPath;

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
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed);
            yield return null;
        }
    }
}
