﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoStepCollision
{
    public static class Func
    {
        const float padding = 0.00002f;
        public static bool Intersects(Box a, Box b)
        {
            return (Mathf.Abs(a.x - b.x) * 2f < (a.width + b.width)) && (Mathf.Abs(a.y - b.y) * 2f < (a.height + b.height));
        }
        public static bool IntersectsAlt(Box a, Box b)
        {
            return a.x <= b.x + b.width && a.x + a.width >= b.x && a.y <= b.y + b.height && a.y + a.height >= b.y;
        }
        public static bool Touch(Box a, Box b)
        {
            return (Mathf.Abs(a.x - b.x) * 2f <= (a.width + b.width)) && (Mathf.Abs(a.y - b.y) * 2f <= (a.height + b.height));
        }
        public static bool Contains(Box a, float x, float y)
        {
            float hwidth = a.width * 0.5f;
            float hheight = a.height * 0.5f;
            return x > a.x - hwidth && x < a.x + hwidth && y > a.y - hheight && y < a.y + hheight;
        }
        public static bool Contains(Box a, Vector2 p)
        {
            return Contains(a, p.x, p.y);
        }
        public static void SuperTranslate(IMover mover, Vector2 movement, IEnumerator<Box> boxEnum)
        {
            Box box = mover.box;
            float f;
            if (movement.y != 0f)
            {
                f = movement.y > 0f ? -0.5f : 0.5f;
                box.y += movement.y;
                while (boxEnum.MoveNext())
                {
                    if (Intersects(box, boxEnum.Current))
                    {
                        box.y = boxEnum.Current.y + f * (boxEnum.Current.height + box.height + padding);
                    }
                }
            }
            if (movement.x != 0f)
            {
                f = movement.x > 0f ? -0.5f : 0.5f;
                box.x += movement.x;
                boxEnum.Reset();
                while (boxEnum.MoveNext())
                {
                    if (Intersects(box, boxEnum.Current))
                    {
                        box.x = boxEnum.Current.x + f * (boxEnum.Current.width + box.width + padding);
                    }
                }
            }
            mover.SetPosition(box.Center);
        }
        public static void SuperTranslate(IMover mover, Vector2 movement, IEnumerable<Box> boxes)
        {
            SuperTranslate(mover, movement, boxes.GetEnumerator());
        }
        public static void BeginBoxesGL(Material mat)
        {
            GL.Begin(GL.QUADS);
            mat.SetPass(0);
            GL.Color(mat.color);
            GL.wireframe = true;
        }
        public static void BeginBoxesGL(Material mat, Color color)
        {
            GL.Begin(GL.QUADS);
            mat.SetPass(0);
            GL.Color(color);
            GL.wireframe = true;
        }
        public static void DrawBoxGL(Box box)
        {
            GL.Vertex3(box.x - box.width * 0.5f, box.y + box.height * 0.5f, 0);
            GL.Vertex3(box.x + box.width * 0.5f, box.y + box.height * 0.5f, 0);
            GL.Vertex3(box.x + box.width * 0.5f, box.y - box.height * 0.5f, 0);
            GL.Vertex3(box.x - box.width * 0.5f, box.y - box.height * 0.5f, 0);
        }

        public static void EndBoxesGL()
        {
            GL.End();
            GL.wireframe = false;
        }
    }

    public interface ICollidesWith<T>
    {
        bool Intersects(T other);
    }

    [Serializable]
    public class Tri : ICollidesWith<Box>
    {
        public Vector2 pos;
        public float x;
        public float y;

        bool ICollidesWith<Box>.Intersects(Box other)
        {
            throw new NotImplementedException();
        }
    }


    [Serializable]
    public class Box : IEquatable<Box>, ICollidesWith<Box>, ICollidesWith<Tri>
    {
        public float x, y, width, height;
        public Vector2 Center
        {
            get
            {
                return new Vector2(x, y);
            }
            set
            {
                x = value.x;
                y = value.y;
            }
        }
        public float Left
        {
            get
            {
                return x - width * 0.5f;
            }
        }
        public float Right
        {
            get
            {
                return x + width * 0.5f;
            }
        }
        public float Top
        {
            get
            {
                return y + height * 0.5f;
            }
        }
        public float Bottom
        {
            get
            {
                return y - height * 0.5f;
            }
        }
        public Box(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
        public Box(Vector2 v, float width, float height)
        {
            this.x = v.x;
            this.y = v.y;
            this.width = width;
            this.height = height;
        }

        bool ICollidesWith<Box>.Intersects(Box other)
        {
            return Func.Intersects(this, other);
        }

        bool ICollidesWith<Tri>.Intersects(Tri other)
        {
            throw new NotImplementedException();
        }

        bool IEquatable<Box>.Equals(Box other)
        {
            return this == other;
        }

        public static bool operator ==(Box a, Box b)
        {
            return a.x == b.x && a.y == b.y && a.width == b.width && a.height == b.height;
        }
        public static bool operator !=(Box a, Box b)
        {
            return a.x != b.x || a.y != b.y || a.width != b.width || a.height != b.height;
        }

        #region AutoGenerated
        public override bool Equals(object obj)
        {
            var box = obj as Box;
            return box != null && this == box;
        }

        public override int GetHashCode()
        {
            var hashCode = -1222528132;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            hashCode = hashCode * -1521134295 + width.GetHashCode();
            hashCode = hashCode * -1521134295 + height.GetHashCode();
            return hashCode;
        }
        #endregion
    }

    public interface IMover
    {
        Box box { get; }
        void SetPosition(Vector2 position);
    }


}
