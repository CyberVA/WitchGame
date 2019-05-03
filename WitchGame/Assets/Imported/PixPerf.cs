﻿using UnityEngine;
using System.Collections;
using System;

public class PixPerf : MonoBehaviour
{
    Camera mainCamera;
    public int pixelsPerUnit;
    [NonSerialized]
    public float f;
    /*
    int lastHeight = -1;
#if UNITY_EDITOR
    int lastWidth = -1;
    bool badW = false;
    bool badH = false;
    bool change;
#endif*/

    public void Init()
    {
        mainCamera = GetComponent<Camera>();
    }

    public void SetScale(int scale)
    {
        f = (1f / (pixelsPerUnit * 2)) / scale;
        mainCamera.orthographicSize = Screen.height * f;
    }
    public void FixViewport()
    {
        Rect r = new Rect(0f, 0f, 1f, 1f);
        if (Screen.height % 2 == 1)
        {
            r.height = 1f - 1f / Screen.height;
            Debug.LogWarning("Viewport Height Altered");
        }
        if (Screen.width % 2 == 1)
        {
            r.width = 1f - 1f / Screen.width;
            Debug.LogWarning("Viewport Width Altered");
        }
        mainCamera.rect = r;
    }
    public int GetMaxScale(int targetHeight)
    {
        int scale = Screen.height / targetHeight;
        if (scale <= 0)
        {
            scale = 1;
            Debug.LogWarning("size too small");
        }
        return scale;
    }
}
