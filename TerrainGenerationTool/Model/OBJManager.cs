using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace TerrainGenerationTool.Model
{
    internal class OBJManager
    {
        public string CreateObjFile(Vector2 scale)
        {
            int vertexWidth = scale.x * 2 + 1;
            int vertexHeight = vertexWidth;

            if (scale.x != scale.y)
                vertexHeight = scale.y * 2 + 1;

            Vector3[] verticies = new Vector3[vertexWidth * vertexHeight];

            int index = 0;
            for(int i = -scale.x; i <= scale.x; i++)
            {
                for (int j = -scale.y; j <= scale.y; j++)
                {
                    verticies[index] = new Vector3() { x = i, y = 0, z = j };
                    index++;
                }
            }

            foreach (Vector3 vertex in verticies)
            {
                Debug.WriteLine($"v {vertex.x} {vertex.y} {vertex.z}");
            }




            string tempData =
                """
                v -1 0 -1
                v 1 0 -1
                v 1 0 1
                v -1 0 1

                f 1 2 3
                f 1 3 4
                """;

            return tempData;
        }
    }
}
