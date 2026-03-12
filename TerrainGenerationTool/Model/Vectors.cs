using System;
using System.Collections.Generic;
using System.Text;

namespace TerrainGenerationTool.Model
{
    struct Vector2
    {
        public int x, y;

        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    struct Vector2F
    {
        public float x, y;

        public Vector2F(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    struct Vector3
    {
        public int x, y, z;

        public Vector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    struct Vector3F
    {
        public float x, y, z;

        public Vector3F(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3F operator +(Vector3F a, Vector3F b)
        {
            return new Vector3F(a.x + b.x, a.y + b.y, a.z + b.z);
        }


        public static Vector3F operator -(Vector3F a, Vector3F b)
        {
            return new Vector3F(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3F Cross(Vector3F a, Vector3F b)
        {
            return new Vector3F(
                a.y * b.z - a.z * b.y,
                a.z* b.x - a.x * b.z,
                a.x * b.y - a.y * b.x
                );
        }

        public static Vector3F Normalise(Vector3F vector)
        {
            double magnitude = Magnitude(vector);

            return new Vector3F(
                vector.x / (float)magnitude,
                vector.y / (float)magnitude,
                vector.z / (float)magnitude
                );
        }

        public static double Magnitude(Vector3F vector)
        {
            return Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }
    }

}
