using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Utils
{
    public static Direction GetDirection(Vector2 vector)
    {
        if(vector.x < vector.y) //up/left
        {
            if(Mathf.Abs(vector.x) < vector.y)
            {
                return Direction.Up;
            }
            else
            {
                return Direction.Left;
            }
        }
        else
        {
            if (Mathf.Abs(vector.y) < vector.x)
            {
                return Direction.Right;
            }
            else
            {
                return Direction.Down;
            }
        }
    }
}
