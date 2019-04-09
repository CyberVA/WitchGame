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
        public static void SuperTranslate(Box agent, Vector2 movement, IEnumerable<Box> boxes)
        {
            IEnumerator<Box> boxEnum = boxes.GetEnumerator();
            float f;
            if (movement.y != 0f)
            {
                f = movement.y > 0f ? -0.5f : 0.5f;
                agent.y += movement.y;
                while (boxEnum.MoveNext())
                {
                    if (Intersects(agent, boxEnum.Current))
                    {
                        agent.y = boxEnum.Current.y + f * (boxEnum.Current.height + agent.height + padding);
                    }
                }
            }
            if (movement.x != 0f)
            {
                f = movement.x > 0f ? -0.5f : 0.5f;
                agent.x += movement.x;
                boxEnum.Reset();
                while (boxEnum.MoveNext())
                {
                    if (Intersects(agent, boxEnum.Current))
                    {
                        agent.x = boxEnum.Current.x + f * (boxEnum.Current.width + agent.width + padding);
                    }
                }
            }
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

    [Serializable]
    public class Box : IEquatable<Box>
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
        public float Top
        {
            get
            {
                return x - width * 0.5f;
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

}