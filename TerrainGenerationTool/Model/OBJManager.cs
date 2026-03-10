using System;
using System.Collections.Generic;
using System.Text;

namespace TerrainGenerationTool.Model
{
    internal class OBJManager
    {
        public string CreateObjFile()
        {
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
