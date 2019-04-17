using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridTransform
{
    public Vector2 origin;
    public Vector2 offset;
    public float tileSize;

    public GridTransform()
    {
        origin = Vector2.zero;
        tileSize = 1f;
    }
    public GridTransform(Vector2 _origin, float _tileSize)
    {
        origin = _origin;
        tileSize = _tileSize;
    }
    public void SetOffset(int width, int height)
    {
        offset.x = (width - 1) * tileSize *- 0.5f;
        offset.y = (height - 1) * tileSize * -0.5f;
    }
    public Rect GetRect(GridPos p)
    {
        Rect r = new Rect(0f, 0f, tileSize, tileSize);
        r.center = GetGridVector(p);
        return r;
    }
    public GridPos GetGridPos(Vector2 v)
    {
        v -= origin + offset;
        GridPos p = new GridPos((int)((v.x + tileSize / 2) / tileSize), (int)((v.y + tileSize / 2) / tileSize));
        return p;
    }
    public Vector2 GetGridVector(GridPos p)
    {
        return new Vector2(p.x * tileSize, p.y * tileSize) + origin + offset;
    }
    public Vector2 SnappedVector(Vector2 v)
    {
        return GetGridVector(GetGridPos(v));
    }
}
