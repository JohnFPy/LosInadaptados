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

            AttachedToVisualTree += (s, e) => ExtractAudioFile();
        }

        public bool IsPlaying => _isPlaying;

        /// Registers this audio instance with the AudioManager
        public void RegisterWithAudioManager(Action? onAudioStopped = null)
        {
            AudioManager.Instance.RegisterAudio(this, onAudioStopped);
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

        private void ExtractAudioFile()
        {
            if (string.IsNullOrEmpty(AudioFileName))
            {
                Debug.WriteLine("Warning: AudioFileName is null or empty");
                return;
            }

            try
            {
                var resourceNames = GetType().Assembly.GetManifestResourceNames();

                if (resourceNames == null || resourceNames.Length == 0)
                {
                    Debug.WriteLine("Warning: No resource names found in assembly");
                    return;
                }

                string? audioResourceName = resourceNames.FirstOrDefault(n =>
                    n != null && n.EndsWith(AudioFileName, StringComparison.OrdinalIgnoreCase));

                if (audioResourceName == null)
                {
                    audioResourceName = resourceNames.FirstOrDefault(n =>
                        n != null && (n.Contains("mp3", StringComparison.OrdinalIgnoreCase) ||
                        n.Contains("wav", StringComparison.OrdinalIgnoreCase)));
                }

                if (audioResourceName != null)
                {
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
                Debug.WriteLine($"Error extracting audio file: {ex.Message}");
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

        public void PlayAudio()
        {
            try
            {
                if (!AudioManager.Instance.RequestPlayAudio(this))
                {
                    Debug.WriteLine("AudioManager denied play request");
                    return;
                }

                if (_audioPath == null)
                {
                    ExtractAudioFile();
                }

                if (string.IsNullOrEmpty(_audioPath))
                {
                    Debug.WriteLine("Audio path is not available");
                    return;
                }

                DisposeAudioResources();
                _audioFileReader = new AudioFileReader(_audioPath);
                _outputDevice = new WaveOutEvent();
                _outputDevice.PlaybackStopped += OnPlaybackStopped;
                _outputDevice.Init(_audioFileReader);
                _outputDevice.Play();
                _isPlaying = true;

                Debug.WriteLine($"Started playing audio: {AudioFileName}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error playing audio: {ex.Message}");
                DisposeAudioResources();
                AudioManager.Instance.NotifyAudioStopped(this);
            }
        }

        public void StopAudio()
        {
            if (_outputDevice != null)
            {
                _outputDevice.Stop();
                DisposeAudioResources();
                _isPlaying = false;
                AudioManager.Instance.NotifyAudioStopped(this);
                
                Debug.WriteLine($"Stopped playing audio: {AudioFileName}");
            }
        }

        private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                _isPlaying = false;
                UpdateButtonText();
                
                AudioManager.Instance.NotifyAudioStopped(this);
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
            
            AudioManager.Instance.UnregisterAudio(this);
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            Dispose();
            base.OnDetachedFromVisualTree(e);
        }
    }
}