using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using TerrainGenerationTool.MVVM;
using TerrainGenerationTool.Model;
using System.Drawing;
using System.IO;
using System.Windows;

namespace TerrainGenerationTool.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private RelayCommand? selectCommand;
        public RelayCommand SelectCommand => selectCommand??= new RelayCommand(execute => SelectFile());

        private RelayCommand? saveCommand;
        public RelayCommand SaveCommand => saveCommand??= new RelayCommand(execute => { SaveObj(); });

        private string? filePath;
        public string FilePath { get => filePath??= defaultFilePathText;  set { filePath = value; OnPropertyChanged(nameof(FilePath)); } }

        private string defaultFilePathText;

        private string? xValue;
        public string XValue { get => xValue ??= defaultXValue; set { xValue = value; OnPropertyChanged(nameof(XValue)); } }

        private string defaultXValue;

        private string? yValue;
        public string YValue { get => yValue ??= defaultYValue; set { yValue = value; OnPropertyChanged(nameof(YValue)); } }

        private string defaultYValue;

        private string? magnitudeValue;
        public string MagnitudeValue { get => magnitudeValue ??= defaultMagnitudeValue; set { magnitudeValue = value; OnPropertyChanged(nameof(MagnitudeValue)); } }

        private string defaultMagnitudeValue;

        private string? resolutionValue;
        public string ResolutionValue { get => resolutionValue ??= defaultResolutionValue; set { resolutionValue = value; OnPropertyChanged(nameof(ResolutionValue)); } }

        private string defaultResolutionValue;

        private string? statusValue;
        public string StatusValue { get => statusValue ??= defaultStatusValue; set { statusValue = value; OnPropertyChanged(nameof(StatusValue)); } }
        private string defaultStatusValue;

        public MainWindowViewModel()
        {
            defaultFilePathText = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets", "TestMap.bmp");

            Debug.WriteLine(defaultFilePathText);

            defaultXValue = "63";
            defaultYValue = "63";
            defaultMagnitudeValue = "10.0";
            defaultResolutionValue = "1.0";
            defaultStatusValue = "Status: Not Started";

            UpdateGenerationState(GenerationState.NotStarted);
        }

        private void SelectFile()
        {
            FileManager fileManager = new FileManager();
            string path;
            FilePath = (!string.IsNullOrEmpty(path = fileManager.SelectFile("Select Image", "Bitmap files (*.bmp)|*.bmp")) ? path : FilePath);
        }

        private async void SaveObj()
        {
            Bitmap heightmap = new Bitmap(FilePath);
            FileManager fileManager = new FileManager();
            bool success = await RunCreateObjFile(new Vector2<int>(int.Parse(XValue), int.Parse(YValue)), heightmap, fileManager, float.Parse(MagnitudeValue), float.Parse(ResolutionValue));
        }

        public async Task<bool> RunCreateObjFile(Vector2<int> scale, Bitmap heightmap, FileManager fileManager, float magnitude = 1.0f, float resolution = 1.0f)
        {
            OBJManager objManager = new OBJManager();
            return await Task.Run(() => objManager.CreateObjFile(UpdateGenerationState, scale, heightmap, fileManager, magnitude, resolution));
        }

        private void UpdateGenerationState(GenerationState state)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                StatusValue = "Status: " + state switch
                {
                    GenerationState.NotStarted => "Not Started.",
                    GenerationState.GeneratingVerticies => "Generating Verticies...",
                    GenerationState.GeneratingFaces => "Generating Faces...",
                    GenerationState.GeneratingNormals => "Generating Normals...",
                    GenerationState.GeneratingUVs => "Generating UVs...",
                    GenerationState.GeneratingFile => "Generating File...",
                    GenerationState.Cancelled => "Cancelled.",
                    GenerationState.Completed => "Completed.",
                    _ => ""
                };
            });
        }
    }
}
