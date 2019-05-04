using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PixelPerfectUIAnchorManager : MonoBehaviour
{
    public PixPerf pp;
    UIAnchor[] uiAnchors;

    public void Init()
    {
        uiAnchors = FindObjectsOfType<UIAnchor>();
    }

    public void UpdateAnchors()
    {
        Vector2 v = new Vector2(Screen.width, Screen.height);
        v *= pp.f;
        float minPrecision = 1f / pp.pixelsPerUnit;

        if (minPrecision != 0)
        {
            v = new Vector2(v.x - v.x % minPrecision, v.y - v.y % minPrecision);
        }

        foreach (UIAnchor anchor in uiAnchors)
        {
            anchor.Position = anchor.anchorScaler * v;
        }
    }
}
