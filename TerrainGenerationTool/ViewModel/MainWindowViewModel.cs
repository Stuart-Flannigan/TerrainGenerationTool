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

        public MainWindowViewModel()
        {
            defaultFilePathText = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets", "TestMap.bmp");

            Debug.WriteLine(defaultFilePathText);

            defaultXValue = "63";
            defaultYValue = "63";
            defaultMagnitudeValue = "10.0";

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

        private void SaveObj()
        {
            OBJManager objManager = new OBJManager();
            Bitmap heightmap = new Bitmap(FilePath);
            string modelData = objManager.CreateObjFile(new Vector2(int.Parse(XValue), int.Parse(YValue)), heightmap, float.Parse(MagnitudeValue));
            FileManager fileManager = new FileManager();
            if (fileManager.GenerateObj(modelData, "Save OBJ", "Bitmap files (*.obj)|*.obj"))
            {
                //Successfully made file
            }
            else
            {
                string messageBoxText = "There was an error creating an OBJ file.";
                string caption = "Error";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBoxResult result;

                result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
            }
        }
    }
}
