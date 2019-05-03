using UnityEngine;
using System.Collections;

public class UIAnchor : MonoBehaviour
{
    public Vector2 Position
    {
        set
        {
            transform.localPosition = value;
        }
    }
    public Vector2 anchorScaler = Vector2.zero;
}
