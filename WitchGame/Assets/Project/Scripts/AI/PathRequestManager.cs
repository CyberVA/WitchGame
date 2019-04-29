using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Purposed for allowing unitys or objects to make a request to generate an A* path.
/// Will generate a path sequentially over a few frames to avoid performance impacts when possible.
/// Also allows for multiple enemies to make path requests essentially at the same time.
/// </summary>
public class PathRequestManager : MonoBehaviour
{
    //First in First Out collection of objects
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    //used to access stuff from our static function RequestPath
    static PathRequestManager instance;
    //reference to our pathfinding class
    Pathfinding pathfinding;

    //tells us if we're currently processing a path
    bool isProcessingPath;

    public void Initialize()
    {
        instance = this;
        // Sets our reference to reference the pathfinding script
        pathfinding = GetComponent<Pathfinding>();
    }

    /// <summary>
    /// Unitys can make a path request when calling this function, this will generate a path for said unit
    /// </summary>
    /// <param name="pathStart"> Where the path starts (position of unit)</param>
    /// <param name="pathEnd"> Where the path ends (position of target)</param>
    /// <param name="callback"> Storing our path to give to the unit for later</param>
    public static void RequestPath (Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        // Adds newRequest to the instances pathRequestQueue
        instance.pathRequestQueue.Enqueue(newRequest);
        // Attempts to process the next path
        instance.TryProcessNext();
    }

    /// <summary>
    /// Makes an attempt at processing the next path
    /// </summary>
    void TryProcessNext()
    {
        //does stuff if we are not currently processing something, and if our queue isn't empty
        if(!isProcessingPath && pathRequestQueue.Count > 0)
        {
            //sets currentPathRequest to the first item in pathRequestQueue
            currentPathRequest = pathRequestQueue.Dequeue();

            //sets that the script is currently processing a path
            isProcessingPath = true;

            //yells at pathfinding to begin processing a new path.
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);

        }
    }

    /// <summary>
    /// States that we've finished processing the path
    /// </summary>
    /// <param name="path">The path in an array of Vector3's</param>
    /// <param name="success">Whether processing the path was a success or not</param>
    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        //Sets the current path and the success of the path to currentPathRequest
        currentPathRequest.callback(path, success);
        //Sets that we are currently not processing a path
        isProcessingPath = false;
        //Attempts to process a new path
        TryProcessNext();
    }

    /// <summary>
    /// Datatype for our path, should store everything we need to make a path
    /// </summary>
    struct PathRequest
    {
        //Starting position of our path
        public Vector3 pathStart;
        //Targets position
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}
