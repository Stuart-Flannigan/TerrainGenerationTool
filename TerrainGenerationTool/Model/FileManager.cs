using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TerrainGenerationTool.Model
{
    internal class FileManager
    {
        public string SelectFile(string title = "Select File", string filter = "All files (*.*)|*.*")
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = filter,
                Multiselect = false,
                Title = title
            };
            bool? result = fileDialog.ShowDialog();

            if (result == true)
                return fileDialog.FileName;
            else
                return String.Empty;
        }

        public string SelectFolder()
        {
            return string.Empty;
        }

        public string SaveFilePath(string title = "Save File", string filter = "All files (*.*)|*.*")
        {
            SaveFileDialog fileDialog = new SaveFileDialog
            {
                Filter = filter,
                Title = title,
                FileName = "Terrain.obj"
            };

            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }
            else
            {
                return String.Empty;
            }
        }
    }
}
