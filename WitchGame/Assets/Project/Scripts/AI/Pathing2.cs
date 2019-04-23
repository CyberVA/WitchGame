using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathing2 : MonoBehaviour
{
    node[] open; //Nodes that still need to be evaluated
    node[] closed; //Nodes that have been evaluated

    public void Initialize()
    {

    }

    private void Update()
    {
        for(int i = 0; i < open.Length; i++)
        {
            node current;
            //if(open[i] < )
        }
    }

}
struct node
{
    int g;
    int h;
    int f;

}
