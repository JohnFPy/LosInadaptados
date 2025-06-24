using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;
using System;
using System.IO;
using Avalonia.Media;
using Avalonia.Threading;
using NAudio.Wave;
using System.Diagnostics;
using System.Linq;

namespace Project.presentation.components
{
    public partial class Audio : UserControl, IDisposable
    {
        public static readonly StyledProperty<string> AudioFileNameProperty =
            AvaloniaProperty.Register<Audio, string>(nameof(AudioFileName), "relaxingPiano.mp3");

        public string AudioFileName
        {
            get => GetValue(AudioFileNameProperty);
            set => SetValue(AudioFileNameProperty, value);
        }

        public static readonly StyledProperty<string> ButtonTextProperty =
            AvaloniaProperty.Register<Audio, string>(nameof(ButtonText), "Reproducir audio");

        public string ButtonText
        {
            get => GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly StyledProperty<string> DescriptionProperty =
            AvaloniaProperty.Register<Audio, string>(nameof(Description), string.Empty);

        public string Description
        {
            get => GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        private string? _audioPath;
        private bool _isPlaying = false;
        private AudioFileReader? _audioFileReader;
        private WaveOutEvent? _outputDevice;
        private Button? _playButton;

        public Audio()
        {
            InitializeComponent();

            // Registrar manejadores para cambios de propiedad
            PropertyChanged += OnAudioPropertyChanged;

            _playButton = this.FindControl<Button>("PlayButton");
            if (_playButton != null)
            {
                _playButton.Click += PlayButton_Click;
                UpdateButtonText();
            }

            // Cargar audio cuando el componente esté inicializado
            AttachedToVisualTree += (s, e) => ExtractAudioFile();
        }

        private void OnAudioPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == AudioFileNameProperty)
            {
                ExtractAudioFile();
            }
            else if (e.Property == ButtonTextProperty)
            {
                UpdateButtonText();
            }
        }

        private void UpdateButtonText()
        {
            if (_playButton != null)
            {
                _playButton.Content = new TextBlock { Text = ButtonText };
            }
        }

        // Updated ExtractAudioFile method with null safety
        private void ExtractAudioFile()
        {
            // Return early if the audio file name is null
            if (string.IsNullOrEmpty(AudioFileName))
            {
                Console.WriteLine("Warning: AudioFileName is null or empty");
                return;
            }

            try
            {
                // Intentar primero cargar desde recursos embebidos
                var resourceNames = GetType().Assembly.GetManifestResourceNames();

                // Check if resourceNames is null or empty
                if (resourceNames == null || resourceNames.Length == 0)
                {
                    Console.WriteLine("Warning: No resource names found in assembly");
                    return;
                }

                // Buscar archivo por nombre especificado - with null safety
                string? audioResourceName = resourceNames.FirstOrDefault(n =>
                    n != null && n.EndsWith(AudioFileName, StringComparison.OrdinalIgnoreCase));

                // Si no encontramos el específico, buscar cualquier audio
                if (audioResourceName == null)
                {
                    audioResourceName = resourceNames.FirstOrDefault(n =>
                        n != null && (n.Contains("mp3", StringComparison.OrdinalIgnoreCase) ||
                        n.Contains("wav", StringComparison.OrdinalIgnoreCase)));
                }

                if (audioResourceName != null)
                {
                    // Usar el nombre del archivo original para la extensión correcta
                    string extension = Path.GetExtension(AudioFileName);
                    if (string.IsNullOrEmpty(extension))
                        extension = ".mp3";

                    var tempPath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Guid.NewGuid().ToString()) + extension);

                    using (var stream = GetType().Assembly.GetManifestResourceStream(audioResourceName))
                    {
                        if (stream != null)
                        {
                            using (var fileStream = File.Create(tempPath))
                            {
                                stream.CopyTo(fileStream);
                            }
                            _audioPath = tempPath;
                            return;
                        }
                    }
                }

                // Si no está como recurso embebido, intentar cargar desde sistema de archivos
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var resourcePath = Path.Combine(baseDir, "resources", "audio", AudioFileName);

                if (File.Exists(resourcePath))
                {
                    _audioPath = resourcePath;
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting audio file: {ex.Message}");
            }
        }

        private void PlayButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                StopAudio();
                UpdateButtonText();
            }
            else if (!string.IsNullOrEmpty(_audioPath))
            {
                PlayAudio();
                if (_playButton != null)
                {
                    _playButton.Content = new TextBlock { Text = "Detener audio" };
                }
            }
        }

        // Changed from private to public to allow access from Card class
        public void PlayAudio()
        {
            try
            {
                // Make sure the audio file is extracted first
                if (_audioPath == null)
                {
                    ExtractAudioFile();
                }

                DisposeAudioResources();
                _audioFileReader = new AudioFileReader(_audioPath);
                _outputDevice = new WaveOutEvent();
                _outputDevice.PlaybackStopped += OnPlaybackStopped;
                _outputDevice.Init(_audioFileReader);
                _outputDevice.Play();
                _isPlaying = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing audio: {ex.Message}");
                DisposeAudioResources();
            }
        }

        // Changed from private to public to allow stopping audio from outside
        public void StopAudio()
        {
            if (_outputDevice != null)
            {
                _outputDevice.Stop();
                DisposeAudioResources();
                _isPlaying = false;
            }
        }

        private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                _isPlaying = false;
                UpdateButtonText();
            });

            DisposeAudioResources();
        }

        private void DisposeAudioResources()
        {
            if (_outputDevice != null)
            {
                _outputDevice.PlaybackStopped -= OnPlaybackStopped;
                _outputDevice.Dispose();
                _outputDevice = null;
            }

            if (_audioFileReader != null)
            {
                _audioFileReader.Dispose();
                _audioFileReader = null;
            }
        }

        public void Dispose()
        {
            PropertyChanged -= OnAudioPropertyChanged;
            StopAudio();
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            Dispose();
            base.OnDetachedFromVisualTree(e);
        }
    }
}