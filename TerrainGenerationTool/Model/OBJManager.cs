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

            string fileData = string.Empty;
            Vector3[] verticies = new Vector3[vertexWidth * vertexHeight];

            int index = 0;
            for(int i = -scale.x; i <= scale.x; i++)
            {
                for (int j = -scale.y; j <= scale.y; j++)
                {
                    verticies[index] = new Vector3() { x = i, y = 0, z = j };

                    fileData += $"v {verticies[index].x} {verticies[index].y} {verticies[index].z}\n";

                    index++;
                }
            }

            fileData += "\n";

            for (int i = 1; i < vertexWidth; i++)
            {
                for (int j = 0; j < vertexHeight; j++)
                {
                    Vector3 topTriangle = new Vector3(
                        i + (j * vertexHeight),
                        (i + 1) + (j * vertexHeight),
                        (i + 1) + ((j + 1) * vertexHeight) 
                        );

                    fileData += $"f {topTriangle.x} {topTriangle.y} {topTriangle.z}\n";

                    Vector3 bottomTriangle = new Vector3(
                        i + (j * vertexHeight),
                        (i + 1) + ((j + 1) * vertexHeight),
                        (i) + ((j + 1) * vertexHeight)
                        );

                    fileData += $"f {bottomTriangle.x} {bottomTriangle.y} {bottomTriangle.z}\n";
                }
            }

            Debug.Write(fileData);

            /*string tempData =
                """
                v -1 0 -1
                v 1 0 -1
                v 1 0 1
                v -1 0 1

                f 1 2 3
                f 1 3 4
                """;*/

            return fileData;
        }
    }
}
