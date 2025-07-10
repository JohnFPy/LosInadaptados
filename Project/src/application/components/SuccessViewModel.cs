using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.application.components
{
    public class SuccessViewModel
    {
        public string FilePath { get; }
        public relayCommand OpenFolderCommand { get; }
        public relayCommand CloseCommand { get; }

        private Window _window;

        public SuccessViewModel(string filePath, Window window)
        {
            FilePath = $"Ubicación:\n{filePath}";
            _window = window;

            OpenFolderCommand = new relayCommand(_ => OpenFolder(filePath));
            CloseCommand = new relayCommand(_ => _window.Close());
        }

        private void OpenFolder(string filePath)
        {
            try
            {
                var folderPath = System.IO.Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(folderPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = folderPath,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al abrir carpeta: {ex.Message}");
            }
        }

    }
}
