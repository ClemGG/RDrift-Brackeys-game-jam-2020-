using UnityEngine;


namespace Clement.Utilities.Vectors
{
    public static class Vectors2
    {
        public static Vector2 Tov2(this float f)
        {
            return new Vector2(f, f);
        }

        public static Vector2 Tov2(this Vector3 v3)
        {
            return new Vector2(v3.x, v3.y);
        }

        public static Vector2 ToNegative(this Vector2 v2)
        {
            return new Vector2(-v2.x, -v2.y);
        }

        public static Vector2 ToNegativeX(this Vector2 v2)
        {
            return new Vector2(-v2.x, v2.y);
        }

        public static Vector2 ToNegativeY(this Vector2 v2)
        {
            return new Vector2(v2.x, -v2.y);
        }





        public static Vector2 Mul(this Vector2 a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }
        public static Vector2 Mul(this Vector2 a, Vector3 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }
        public static Vector2 Mul(this Vector2 a, float f)
        {
            return new Vector2(a.x * f, a.y * f);
        }
        public static Vector2 Mul(this Vector2 a, int i)
        {
            return new Vector2(a.x * i, a.y * i);
        }



        public static Vector2 Div(this Vector2 a, Vector2 b)
        {
            return new Vector2(a.x / b.x, a.y / b.y);
        }
        public static Vector2 Div(this Vector2 a, Vector3 b)
        {
            return new Vector2(a.x / b.x, a.y / b.y);
        }
        public static Vector2 Div(this Vector2 a, float f)
        {
            return new Vector2(a.x / f, a.y / f);
        }
        public static Vector2 Div(this Vector2 a, int i)
        {
            return new Vector2(a.x / i, a.y / i);
        }

    }


    public static class Vectors3
    {
        public static Vector3 Tov3(this float f)
        {
            return new Vector3(f, f, f);
        }



        public static Vector3 To_XYz(this Vector2 v2)
        {
            return new Vector3(v2.x, v2.y, 0f);
        }

        public static Vector3 To_XyZ(this Vector2 v2)
        {
            return new Vector3(v2.x, 0f, v2.y);
        }

        public static Vector3 To_xYZ(this Vector2 v2)
        {
            return new Vector3(0f, v2.y, v2.x);
        }





        public static Vector3 ToNegative(this Vector3 v3)
        {
            return new Vector3(-v3.x, -v3.y, -v3.z);
        }

        public static Vector3 ToNegativeX(this Vector3 v3)
        {
            return new Vector3(-v3.x, v3.y, v3.z);
        }

        public static Vector3 ToNegativeY(this Vector3 v3)
        {
            return new Vector3(v3.x, -v3.y, v3.z);
        }

        public static Vector3 ToNegativeZ(this Vector3 v3)
        {
            return new Vector3(v3.x, v3.y, -v3.z);
        }




        public static Vector3 ToNegativeXY(this Vector3 v3)
        {
            return new Vector3(-v3.x, -v3.y, v3.z);
        }

        public static Vector3 ToNegativeXZ(this Vector3 v3)
        {
            return new Vector3(-v3.x, v3.y, -v3.z);
        }

        public static Vector3 ToNegativeYZ(this Vector3 v3)
        {
            return new Vector3(v3.x, -v3.y, -v3.z);
        }






        public static Vector2 Mul(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z);
        }
        public static Vector3 Mul(this Vector3 a, Vector2 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z);
        }
        public static Vector3 Mul(this Vector3 a, float f)
        {
            return new Vector3(a.x * f, a.y * f, a.z);
        }
        public static Vector3 Mul(this Vector3 a, int i)
        {
            return new Vector3(a.x * i, a.y * i, a.z);
        }



        public static Vector3 Div(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z);
        }
        public static Vector3 Div(this Vector3 a, Vector2 b)
        {
            return new Vector3(a.x / b.x, a.y / b.y, a.z);
        }
        public static Vector3 Div(this Vector3 a, float f)
        {
            return new Vector3(a.x / f, a.y / f, a.z);
        }
        public static Vector3 Div(this Vector3 a, int i)
        {
            return new Vector3(a.x / i, a.y / i, a.z);
        }


    }
}