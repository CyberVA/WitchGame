using System;
using UnityEngine;

namespace Byteable
{
    public partial class ByteScribe
    {
        public void Write(Vector2 v)
        {
            Write(v.x);
            Write(v.y);
        }
        public Vector2 ReadVector2()
        {
            return new Vector2(ReadFloat(), ReadFloat());
        }
        public void Write(Vector3 v)
        {
            Write(v.x);
            Write(v.y);
            Write(v.z);
        }
        public Vector3 ReadVector3()
        {
            return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
        }
    }
}


