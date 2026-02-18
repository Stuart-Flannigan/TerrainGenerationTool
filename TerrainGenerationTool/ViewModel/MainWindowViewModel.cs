using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using TerrainGenerationTool.MVVM;
using TerrainGenerationTool.Model;
using System.Drawing;
using System.IO;

namespace TerrainGenerationTool.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private RelayCommand? selectCommand;
        public RelayCommand SelectCommand => selectCommand??= new RelayCommand(execute => SelectFile());

        private string? filePath;
        public string FilePath { get => filePath??= defaultFilePathText;  set { filePath = value; OnPropertyChanged(nameof(FilePath)); } }

        private string defaultFilePathText;

        public MainWindowViewModel()
        {
            defaultFilePathText = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets", "TestMap.bmp");

            Debug.WriteLine(defaultFilePathText);
        }

        private void SelectFile()
        {
            Debug.WriteLine("Selecting File");
            FileManager fileManager = new FileManager();
            string path;
            FilePath = (!string.IsNullOrEmpty(path = fileManager.SelectFile("Select Image", "Bitmap files (*.bmp)|*.bmp")) ? path : FilePath);
            Debug.Write(FilePath);

            
        }
    }
}
