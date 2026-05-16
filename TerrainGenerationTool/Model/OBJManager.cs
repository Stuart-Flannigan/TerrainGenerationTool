using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Shapes;

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


        public bool CreateObjFile(Action<GenerationState> reportState, Vector2 scale, Bitmap heightmap, FileManager fileManager, float magnitude = 1.0f)
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
             * ------------------
             * Generate Verticies
             * ------------------
            **/
            try
            {
                reportState(GenerationState.GeneratingVerticies);

                for (int i = -scale.x; i <= scale.x; i++)
                {
                    for (int j = -scale.y; j <= scale.y; j++)
                    {
                        Vector2 heightmapPos = new Vector2((int)((i + scale.x) * scaleDifference.x), (int)((j + scale.y) * scaleDifference.y));

                        Color pixelColour = heightmap.GetPixel(heightmapPos.x, heightmapPos.y);
                        float saturation = ((pixelColour.R / 255f) + (pixelColour.G / 255f) + (pixelColour.B / 255f)) / 3;
                        float height = saturation * magnitude;

                        verticies.Add(new Vector3F() { x = i, y = height, z = j });
                    }
                }
            }
            catch 
            {
                reportState(GenerationState.Cancelled);
                return false;
            }

            /*
             * -----
             * Faces
             * -----
            **/

            try
            {
                reportState(GenerationState.GeneratingFaces);

                for (int i = 0; i < vertexWidth - 1; i++)
                {
                    for (int j = 0; j < vertexHeight - 1; j++)
                    {
                        int a = j + (i * vertexHeight);
                        int b = (j + 1) + (i * vertexHeight);
                        int c = j + ((i + 1) * vertexHeight);
                        int d = (j + 1) + ((i + 1) * vertexHeight);

                        triangles.Add(new Vector3(a, b, c));
                        triangles.Add(new Vector3(b, d, c));
                    }
                }
            }
            catch
            {
                reportState(GenerationState.Cancelled);
                return false;
            }

            /*
             * ----------------
             * Generate Normals
             * ----------------
            **/

            try
            {
                reportState(GenerationState.GeneratingNormals);

                for (int i = 0; i < verticies.Count; i++)
                    normals.Add(new Vector3F(0, 0, 0));

                // Bottleneck here - due to the Normalise function using square roots

                for (int i = 0; i < triangles.Count; i++)
                {
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

                // Normalise the normals
                for (int i = 0; i < normals.Count; i++)
                {
                    normals[i] = Vector3F.Normalise(normals[i]);
                }
            }
            catch
            {
                reportState(GenerationState.Cancelled);
                return false;
            }

            /*
             * -------------------
             * Texture Coordinates
             * -------------------
            **/

            try
            {
                reportState(GenerationState.GeneratingUVs);

                for (int i = 0; i < verticies.Count; i++)
                {

                    float u = ((verticies[i].x - (-scale.x)) / (2 * scale.x));
                    float v = ((verticies[i].z - (-scale.y)) / (2 * scale.y));

                    textureCoords.Add(new Vector2F(u, v));
                }
            }
            catch
            {
                reportState(GenerationState.Cancelled);
                return false;
            }


            //////
            /////

            reportState(GenerationState.GeneratingFile);

            string filePath = fileManager.SaveFilePath(fileData, "Save OBJ", "Bitmap files (*.obj)|*.obj");

            if(String.IsNullOrEmpty(filePath))
            {
                reportState(GenerationState.Cancelled);
                return false;
            }

            StreamWriter streamWriter = null; 
            try
            {
                streamWriter = new StreamWriter(filePath);

                for (int i = 0; i < verticies.Count; i++)
                {
                    streamWriter.WriteLine($"v {verticies[i].x} {verticies[i].y} {verticies[i].z}");
                    Task.Yield();
                }

                streamWriter.WriteLine("");

                for (int i = 0; i < textureCoords.Count; i++)
                {
                    streamWriter.WriteLine($"vt {textureCoords[i].x} {textureCoords[i].y}");
                    Task.Yield();
                }

                streamWriter.WriteLine("");
                streamWriter.WriteLine("");

                for (int i = 0; i < normals.Count; i++)
                {
                    streamWriter.WriteLine($"vn {normals[i].x} {normals[i].y} {normals[i].z}");
                    Task.Yield();
                }

                streamWriter.WriteLine("");

                for (int i = 0; i < triangles.Count; i++)
                {
                    streamWriter.WriteLine($"f {triangles[i].x + 1} {triangles[i].y + 1} {triangles[i].z + 1}");
                    Task.Yield();
                }

                streamWriter?.Flush();
            }
            catch (Exception ex)
            {
                reportState(GenerationState.Cancelled);
                return false;
            }
            finally
            {
                try { streamWriter?.Dispose(); } catch { }
            }

            reportState(GenerationState.Completed);
            return true;
        }
    }
}
