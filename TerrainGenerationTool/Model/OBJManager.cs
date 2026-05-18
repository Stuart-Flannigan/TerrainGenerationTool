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
        public bool CreateObjFile(Action<GenerationState> reportState, Vector2<int> scale, Bitmap heightmap, FileManager fileManager, float magnitude = 1.0f, float resolutionScale = 1.0f)
        {
            int vertexWidth = (int)(scale.x * 2.0f * resolutionScale) + 1;
            int vertexHeight = vertexWidth;

            if (scale.x != scale.y)
                vertexHeight = (int)(scale.y * 2.0f * resolutionScale) + 1;

            Vector2<float> heightmapSize = new Vector2<float>(heightmap.Size.Width, heightmap.Size.Height);

            Vector2<float> heightmapStep = new Vector2<float>((heightmap.Width - 1) / (float)(vertexWidth - 1), (heightmap.Height - 1) / (float)(vertexHeight - 1));
            Vector2<float> vertexStep = new Vector2<float>((scale.x * 2) / (float)(vertexWidth - 1), (scale.y * 2) / (float)(vertexHeight - 1));

            List<Vector3<float>> verticies = new List<Vector3<float>>();
            List<Vector2<float>> textureCoords = new List<Vector2<float>>();
            List<Vector3<double>> normals = new List<Vector3<double>>();
            List<Vector3<int>> triangles = new List<Vector3<int>>();

            /*
             * ------------------
             * Generate Verticies
             * ------------------
            **/

            try
            {
                reportState(GenerationState.GeneratingVerticies);

                for (float i = 0; i < vertexWidth; i++)
                {
                    for (float j = 0; j < vertexHeight; j++)
                    {
                        Vector2<float> globalPos = new Vector2<float>(-scale.x + i * vertexStep.x, -scale.y + j * vertexStep.y);
                        Vector2<int> heightmapPos = new Vector2<int>((int)Math.Clamp(Math.Round(i * heightmapStep.x), 0, heightmap.Width - 1), (int)Math.Clamp(Math.Round(j * heightmapStep.y), 0, heightmap.Height - 1));


                        Color pixelColour = heightmap.GetPixel(heightmapPos.x, heightmapPos.y);
                        float saturation = ((pixelColour.R / 255f) + (pixelColour.G / 255f) + (pixelColour.B / 255f)) / 3;
                        float height = saturation * magnitude;

                        verticies.Add(new Vector3<float>(globalPos.x, height, globalPos.y));
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Error generating verticies");
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

                        triangles.Add(new Vector3<int>(a, b, c));
                        triangles.Add(new Vector3<int>(b, d, c));
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Error generating faces");
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
                    normals.Add(new Vector3<double>(0, 0, 0));

                for (int i = 0; i < triangles.Count; i++)
                {
                    Vector3<double> a = new Vector3<double>(verticies[triangles[i].x].x, verticies[triangles[i].x].y, verticies[triangles[i].x].z);
                    Vector3<double> b = new Vector3<double>(verticies[triangles[i].y].x, verticies[triangles[i].y].y, verticies[triangles[i].y].z);
                    Vector3<double> c = new Vector3<double>(verticies[triangles[i].z].x, verticies[triangles[i].z].y, verticies[triangles[i].z].z);

                    Vector3<double> ab = b - a;
                    Vector3<double> ac = c - a;

                    Vector3<double> cross = Vector3<double>.Cross(ab, ac);
                    Vector3<double> normal = Vector3<double>.Normalise(cross);

                    normals[triangles[i].x] += normal;
                    normals[triangles[i].y] += normal;
                    normals[triangles[i].z] += normal;
                }

                // Normalise the normals
                for (int i = 0; i < normals.Count; i++)
                {
                    normals[i] = Vector3<double>.Normalise(normals[i]);
                }
            }
            catch
            {
                Debug.WriteLine("Error generating normals");
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

                    textureCoords.Add(new Vector2<float>(u, v));
                }
            }
            catch
            {
                Debug.WriteLine("Error generating texture coordinates");
                reportState(GenerationState.Cancelled);
                return false;
            }


            //////
            //////

            reportState(GenerationState.GeneratingFile);
            return SaveData(reportState, fileManager, verticies, textureCoords, normals, triangles);
        }

        private bool SaveData(Action<GenerationState> reportState, FileManager fileManager, List<Vector3<float>> verticies, List<Vector2<float>> textureCoords, List<Vector3<double>> normals, List<Vector3<int>> triangles)
        {
            string filePath = fileManager.SaveFilePath("Save OBJ", "Bitmap files (*.obj)|*.obj");

            if (String.IsNullOrEmpty(filePath))
            {
                reportState(GenerationState.Cancelled);
                return false;
            }

            StreamWriter? streamWriter = null;
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
            catch
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
