using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace TerrainGenerationTool.Model
{
    struct Vector2<T> where T : INumber<T>
    {
        public T x, y;

        public Vector2(T x, T y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2<T> operator +(Vector2<T> a, Vector2<T> b)
        {
            return new Vector2<T>(a.x + b.x, a.y + b.y);
        }


        public static Vector2<T> operator -(Vector2<T> a, Vector2<T> b)
        {
            return new Vector2<T>(a.x - b.x, a.y - b.y);
        }

        public Vector2<U> Cast<U>() where U : INumber<U>
        {
            return new Vector2<U>(U.CreateChecked(this.x), U.CreateChecked(this.y));
        }
    }

    struct Vector3<T> where T : INumber<T>
    {
        public T x, y, z;

        public Vector3(T x, T y, T z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3<T> operator +(Vector3<T> a, Vector3<T> b)
        {
            return new Vector3<T>(a.x + b.x, a.y + b.y, a.z + b.z);
        }


        public static Vector3<T> operator -(Vector3<T> a, Vector3<T> b)
        {
            return new Vector3<T>(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public Vector3<U> Cast<U>() where U : INumber<U>
        {
            return new Vector3<U>(U.CreateChecked(this.x), U.CreateChecked(this.y), U.CreateChecked(this.z));
        }

        public static Vector3<T> Cross(Vector3<T> a, Vector3<T> b)
        {
            return new Vector3<T>(
                a.y * b.z - a.z * b.y,
                a.z* b.x - a.x * b.z,
                a.x * b.y - a.y * b.x
                );
        }

        public static Vector3<double> Normalise(Vector3<T> vector)
        {
            double magnitude = Magnitude(vector);

            double oldX = double.CreateChecked(vector.x);
            double oldY = double.CreateChecked(vector.y);
            double oldZ = double.CreateChecked(vector.z);

            Vector3<double> newXYZ = new Vector3<double>(
                double.CreateChecked(vector.x) / magnitude,
                double.CreateChecked(vector.y) / magnitude,
                double.CreateChecked(vector.z) / magnitude
                );

            if(oldX == 0)
                newXYZ.x = 0;
            if (oldY == 0)
                newXYZ.y = 0;
            if (oldZ == 0)
                newXYZ.z = 0;

            return newXYZ;
        }

        public static double Magnitude(Vector3<T> vector)
        {
            return Math.Sqrt(double.CreateChecked(vector.x * vector.x) + double.CreateChecked(vector.y * vector.y) + double.CreateChecked(vector.z * vector.z));
        }
    }

}
