using UnityEngine;
using System.Collections;
using TwoStepCollision;

public class TriTest : MonoBehaviour
{
    public Tri tri;
    public Vector2 v;

    bool InTri()
    {
        float w = v.x - tri.pos.x;
        float h = v.y- tri.pos.y;
        Debug.Log(w);
        Debug.Log(h);
        Debug.Log(tri.x);
        return false;
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            InTri();
        }
    }


}
