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
    }

}
