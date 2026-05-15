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
            defaultStatusValue = "Not Started";

            Vector3F a = new Vector3F(2, 3, 5);

            double mag = Vector3F.Magnitude(a);
            Debug.WriteLine($"Mag: {mag}");
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
            string modelData = await RunCreateObjFile(new Vector2(int.Parse(XValue), int.Parse(YValue)), heightmap, float.Parse(MagnitudeValue));

            FileManager fileManager = new FileManager();
            if (!fileManager.GenerateObj(modelData, "Save OBJ", "Bitmap files (*.obj)|*.obj"))
            {
                string messageBoxText = "There was an error creating an OBJ file.";
                string caption = "Error";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBoxResult result = MessageBoxResult.Yes;

                result = MessageBox.Show(messageBoxText, caption, button, icon, result);
            }
        }

        public async Task<string> RunCreateObjFile(Vector2 scale, Bitmap heightmap, float magnitude = 1.0f)
        {
            OBJManager objManager = new OBJManager();
            return await Task.Run(() => objManager.CreateObjFile(UpdateGenerationState, scale, heightmap, magnitude));
        }

        private void UpdateGenerationState(GenerationState state)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                StatusValue = state switch
                {
                    GenerationState.GeneratingVerticies => "Generating Verticies...",
                    GenerationState.GeneratingFaces => "Generating Faces...",
                    GenerationState.GeneratingNormals => "Generating Normals...",
                    GenerationState.GeneratingUVs => "Generating UVs...",
                    GenerationState.GeneratingFile => "Generating File...",
                    _ => ""
                };
            });
        }
    }
}
