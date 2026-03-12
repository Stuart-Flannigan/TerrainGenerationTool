using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Drawing;

namespace TerrainGenerationTool.Model
{
    internal class OBJManager
    {
        string singlePlaneData =
        """
        v -1 0 -1
        v 1 0 -1
        v 1 0 1
        v -1 0 1

        f 1 2 3
        f 1 3 4
        """;


        public string CreateObjFile(Vector2 scale, Bitmap heightmap, float magnitude = 1.0f)
        {
            int vertexWidth = scale.x * 2 + 1;
            int vertexHeight = vertexWidth;

            if (scale.x != scale.y)
                vertexHeight = scale.y * 2 + 1;

            Vector2F heightmapSize = new Vector2F(heightmap.Size.Width, heightmap.Size.Height);
            Vector2F scaleDifference = new Vector2F(heightmapSize.x / (float)vertexWidth, heightmapSize.y / (float)vertexHeight);

            Debug.WriteLine("");
            Debug.WriteLine($"Vertex Size: {vertexWidth}, {vertexHeight}");
            Debug.WriteLine($"Heightmap Size: {heightmapSize.x}, {heightmapSize.y}");
            Debug.WriteLine($"Heightmap Scale: {scaleDifference.x}, {scaleDifference.y}");
            Debug.WriteLine("");

            string fileData = string.Empty;
            Vector3F[] verticies = new Vector3F[vertexWidth * vertexHeight];
            Vector2F[] textureCoords = new Vector2F[vertexWidth * vertexHeight];
            Vector3F[] normals = new Vector3F[vertexWidth * vertexHeight];
            Vector3[] triangles = new Vector3[(scale.x * 4) * (scale.y * 4)];
            
            /*
             * ---------
             * Verticies
             * ---------
            **/

            int index = 0;
            for(int i = -scale.x; i <= scale.x; i++)
            {
                for (int j = -scale.y; j <= scale.y; j++)
                {
                    Vector2 heightmapPos = new Vector2((int)((i + scale.x) * scaleDifference.x), (int)((j + scale.y) * scaleDifference.y));
  
                    Color pixelColour = heightmap.GetPixel(heightmapPos.x, heightmapPos.y);
                    float saturation = ((pixelColour.R / 255f) + (pixelColour.G / 255f) + (pixelColour.B / 255f)) / 3;
                    float height = (1 - saturation) * magnitude;

                    verticies[index] = new Vector3F() { x = i, y = height, z = j };

                    fileData += $"v {verticies[index].x} {verticies[index].y} {verticies[index].z}\n";

                    index++;
                }
            }

            fileData += "\n";

            /*
             * -------------------
             * Texture Coordinates
             * -------------------
            **/

            fileData += "\n";

            /*
             * -----
             * Faces
             * -----
            **/

            index = 0;
            for (int i = 1; i < vertexWidth - 1; i++)
            {
                for (int j = 0; j < vertexHeight - 1; j++)
                {
                    triangles[index] = new Vector3(
                        i + (j * vertexHeight),
                        (i + 1) + (j * vertexHeight),
                        (i + 1) + ((j + 1) * vertexHeight) 
                        );

                    fileData += $"f {triangles[index].x} {triangles[index].y} {triangles[index].z}\n";

                    index++;

                    triangles[index] = new Vector3(
                        i + (j * vertexHeight),
                        (i + 1) + ((j + 1) * vertexHeight),
                        (i) + ((j + 1) * vertexHeight)
                        );

                    fileData += $"f {triangles[index].x} {triangles[index].y} {triangles[index].z}\n";

                    index++;
                }
            }

            /*
             * -------
             * Normals
             * -------
            **/

            index = 0;
            for (int i = 0; i < triangles.Length - 1; i++)
            {
                Debug.WriteLine($"Triangle {i} of {triangles.Length}");
                Debug.WriteLine($"Vertex Indicies: {triangles[i].x}, {triangles[i].y}, {triangles[i].z} of {verticies.Length}");
                Vector3F a = new Vector3F(verticies[triangles[i].x].x, verticies[triangles[i].x].y, verticies[triangles[i].x].z);
                Vector3F b = new Vector3F(verticies[triangles[i].y].x, verticies[triangles[i].y].y, verticies[triangles[i].y].z);
                Vector3F c = new Vector3F(verticies[triangles[i].z].x, verticies[triangles[i].z ].y, verticies[triangles[i].z].z);

                Vector3F ab = b - a;
                Vector3F ac = c - a;

                Vector3F cross = Vector3F.Cross(ab, ac);
                Vector3F normal = Vector3F.Normalise(cross);

                normals[triangles[i].x] += normal;
                normals[triangles[i].y] += normal;
                normals[triangles[i].z] += normal;
            }

            // Must Normalise the normals

            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = Vector3F.Normalise(normals[i]);
                fileData += $"vn {normals[i].x} {normals[i].y} {normals[i].z}\n";
                Debug.WriteLine($"Normal {i}: {normals[i].x}, {normals[i].y}, {normals[i].z}");
            }

            fileData += "\n";


            //////

            Debug.Write(fileData);

            return fileData;
        }
    }
}
