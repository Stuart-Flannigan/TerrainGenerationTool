using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows;

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

            List<Vector3F> verticies = new List<Vector3F>();
            List<Vector2F> textureCoords = new List<Vector2F>();
            List<Vector3F> normals = new List<Vector3F>();
            List<Vector3> triangles = new List<Vector3>();
            StringBuilder fileData = new StringBuilder(4 * (vertexHeight * vertexWidth));

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
                    float height = saturation * magnitude;

                    verticies.Add(new Vector3F() { x = i, y = height, z = j });

                    fileData.AppendLine($"v {verticies[index].x} {verticies[index].y} {verticies[index].z }");

                    index++;
                }
            }

            fileData.AppendLine("");

            MessageBox.Show("Completed verticies", "verticies", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);

            /*
             * -------------------
             * Texture Coordinates
             * -------------------
            **/

            const float baseTile = 1.0f;
            const float tileScale = 3.0f;

            for (int i = 0; i < verticies.Count; i++)
            {
                float steepness = 1.0f - Math.Abs(verticies[i].y);
                float steepScale = 1.0f + (steepness * tileScale);
                float totalScale = baseTile * steepScale;

                float u = ((verticies[i].x - (-scale.x)) / (2 * scale.x)) * totalScale;
                float v = ((verticies[i].z - (-scale.y)) / (2 * scale.y)) * totalScale;

                fileData.AppendLine($"vt {u} {v}");
            }

            fileData.AppendLine("");

            MessageBox.Show("Completed texture coords", "Textre Coords", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);

            /*
             * -----
             * Faces
             * -----
            **/

            fileData.AppendLine("s1");

            index = 0;
            for (int i = 0; i < vertexWidth - 1; i++)
            {
                for (int j = 0; j < vertexHeight - 1; j++)
                {
                    int a = j + (i * vertexHeight);
                    int b = (j + 1) + (i * vertexHeight);
                    int c = j + ((i + 1) * vertexHeight);
                    int d = (j + 1) + ((i + 1) * vertexHeight);

                    triangles.Add(new Vector3(a, b, c));

                    fileData.AppendLine($"f {triangles[index].x + 1} {triangles[index].y + 1} {triangles[index].z + 1}");

                    index++;

                    triangles.Add(new Vector3(b, d, c));

                    fileData.AppendLine($"f {triangles[index].x + 1} {triangles[index].y + 1} {triangles[index].z + 1}");

                    index++;
                }
            }

            MessageBox.Show("Completed Triangles", "Triangles", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);

            /*
             * -------
             * Normals
             * -------
            **/

            for (int i = 0; i < verticies.Count; i++)
                normals.Add(new Vector3F(0, 0, 0));

            // Bottleneck here - due to the Normalise function using square roots

            index = 0;
            for (int i = 0; i < triangles.Count; i++)
            {
                //Debug.WriteLine($"Triangle {i} of {triangles.Count}");
                //Debug.WriteLine($"Vertex Indicies: {triangles[i].x}, {triangles[i].y}, {triangles[i].z} of {verticies.Count}");
                Vector3F a = new Vector3F(verticies[triangles[i].x].x, verticies[triangles[i].x].y, verticies[triangles[i].x].z);
                Vector3F b = new Vector3F(verticies[triangles[i].y].x, verticies[triangles[i].y].y, verticies[triangles[i].y].z);
                Vector3F c = new Vector3F(verticies[triangles[i].z].x, verticies[triangles[i].z].y, verticies[triangles[i].z].z);

                Vector3F ab = b - a;
                Vector3F ac = c - a;

                Vector3F cross = Vector3F.Cross(ab, ac);
                Vector3F normal = Vector3F.Normalise(cross);

                normals[triangles[i].x] += normal;
                normals[triangles[i].y] += normal;
                normals[triangles[i].z] += normal;
            }

            MessageBox.Show("Generated normals", "Normals", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);

            // Must Normalise the normals

            for (int i = 0; i < normals.Count; i++)
            {
                normals[i] = Vector3F.Normalise(normals[i]);
                fileData.AppendLine($"vn {normals[i].x} {normals[i].y} {normals[i].z}");
                //Debug.WriteLine($"Normal {i}: {normals[i].x}, {normals[i].y}, {normals[i].z}");
            }

            MessageBox.Show("Completed normals", "Normals", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);

            //////

            //Debug.Write(fileData);

            return fileData.ToString();
        }
    }
}
