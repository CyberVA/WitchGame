using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class SimpGL
{
    public static void DrawSprite(Rect r, float uvr, float uvb, float uvl, float uvt)
    {
        GL.TexCoord2(uvl, uvb);
        GL.Vertex3(r.xMin, r.yMin, 0f);
        GL.TexCoord2(uvl, uvt);
        GL.Vertex3(r.xMin, r.yMax, 0f);
        GL.TexCoord2(uvr, uvt);
        GL.Vertex3(r.xMax, r.yMax, 0f);
        GL.TexCoord2(uvr, uvb);
        GL.Vertex3(r.xMax, r.yMin, 0f);
    }
}
