using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PixelPerfectUIAnchorManager : MonoBehaviour
{
    UIAnchor[] uiAnchors;
    public PixelPerfectCamera ppc;

    private void Awake()
    {
        uiAnchors = FindObjectsOfType<UIAnchor>();
        Debug.Log(uiAnchors.Length);
    }

    private void LateUpdate()
    {
        enabled = false;

        Vector2 v = Vector2.zero;
        float p = (1f / (ppc.assetsPPU * 2)) / ppc.pixelRatio;
        v.y = Screen.height * p;
        v.x = Screen.width * p;

        foreach(UIAnchor anchor in uiAnchors)
        {
            anchor.Position = anchor.anchorScaler * v;
        }
    }
}
