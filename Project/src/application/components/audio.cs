using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;
using System;
using System.IO;
using Avalonia.Media;
using Avalonia.Threading;

namespace Project.presentation.components
{
    public partial class Audio : UserControl
    {
        private string? _audioPath;
        private System.Diagnostics.Process? _currentProcess;
        private bool _isPlaying = false;

        public Audio()
        {
            Console.WriteLine("Audio constructor start");
            InitializeComponent();
            Console.WriteLine("InitializeComponent completed");
            ExtractAudioFile();
            var playButton = this.FindControl<Button>("PlayButton");
            if (playButton != null)
            {
                Console.WriteLine("PlayButton found, registering click event");
                playButton.Click += PlayButton_Click;
            }
            else
            {
                Console.WriteLine("ERROR: PlayButton not found in the UI");
            }
            Console.WriteLine("Audio constructor completed");
        }

        private void ExtractAudioFile()
        {
            Console.WriteLine("ExtractAudioFile start");
            try
            {
                // Alternative approach using direct file access
                var resourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "audio", "relaxingPiano.mp3");
                Console.WriteLine($"Looking for audio file at: {resourcePath}");

                if (File.Exists(resourcePath))
                {
                    _audioPath = resourcePath;
                    Console.WriteLine("Audio file found successfully");
                }
                else
                {
                    Console.WriteLine("ERROR: Audio file not found at the specified path");
                    // Try to list the directory contents to see what's available
                    var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    Console.WriteLine($"Base directory: {baseDir}");

                    if (Directory.Exists(Path.Combine(baseDir, "resources")))
                    {
                        var folders = Directory.GetDirectories(Path.Combine(baseDir, "resources"));
                        Console.WriteLine($"Available folders in resources: {string.Join(", ", folders)}");

                        if (Directory.Exists(Path.Combine(baseDir, "resources", "audio")))
                        {
                            var files = Directory.GetFiles(Path.Combine(baseDir, "resources", "audio"));
                            Console.WriteLine($"Available files in audio folder: {string.Join(", ", files)}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in audio extraction: {ex.Message}");
                Console.WriteLine($"Exception stack trace: {ex.StackTrace}");
            }
            Console.WriteLine("ExtractAudioFile end");
        }

        private void PlayButton_Click(object? sender, RoutedEventArgs e)
        {
            Console.WriteLine("PlayButton_Click triggered");

            try
            {
                if (_isPlaying && _currentProcess != null && !_currentProcess.HasExited)
                {
                    Console.WriteLine("Stopping audio playback");
                    _currentProcess.Kill();
                    _isPlaying = false;
                    Console.WriteLine("Audio playback stopped");
                }
                else if (!string.IsNullOrEmpty(_audioPath))
                {
                    Console.WriteLine($"Starting audio playback from: {_audioPath}");

                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "powershell",
                        Arguments = $"-c (New-Object Media.SoundPlayer '{_audioPath}').PlaySync()",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    Console.WriteLine("Starting process with parameters:");
                    Console.WriteLine($"  FileName: {psi.FileName}");
                    Console.WriteLine($"  Arguments: {psi.Arguments}");

                    _currentProcess = System.Diagnostics.Process.Start(psi);

                    if (_currentProcess != null)
                    {
                        Console.WriteLine($"Process started with ID: {_currentProcess.Id}");
                        _isPlaying = true;

                        // Capture process output (optional)
                        string output = _currentProcess.StandardOutput.ReadToEnd();
                        string error = _currentProcess.StandardError.ReadToEnd();

                        if (!string.IsNullOrEmpty(output))
                            Console.WriteLine($"Process output: {output}");
                        if (!string.IsNullOrEmpty(error))
                            Console.WriteLine($"Process error: {error}");
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Failed to start process - Process.Start returned null");
                    }
                }
                else
                {
                    Console.WriteLine("ERROR: Cannot play audio - audioPath is null or empty");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in audio playback: {ex.Message}");
                Console.WriteLine($"Exception stack trace: {ex.StackTrace}");
            }
        }
    }
}