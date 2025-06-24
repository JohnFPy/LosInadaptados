using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NAudio.CoreAudioApi;
using System;

namespace Project.presentation.components
{
    public partial class VolumeController : UserControl
    {
        private Slider _volumeSlider;
        private MMDeviceEnumerator _deviceEnumerator;
        private MMDevice _device;

        public VolumeController()
        {
            InitializeComponent();

            _volumeSlider = this.FindControl<Slider>("VolumeSlider");

            if (_volumeSlider != null)
            {
                InitializeAudio();

                // Set initial value to 100 as requested
                _volumeSlider.Value = 70;
                SetSystemVolume(0.7f); // Set system volume to match

                // Add event handler for value changes
                _volumeSlider.PropertyChanged += (sender, e) =>
                {
                    if (e.Property.Name == "Value")
                    {
                        SetSystemVolume((float)_volumeSlider.Value / 100f);
                    }
                };
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void InitializeAudio()
        {
            try
            {
                _deviceEnumerator = new MMDeviceEnumerator();
                _device = _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing audio: {ex.Message}");
            }
        }

        private float GetSystemVolume()
        {
            try
            {
                if (_device != null)
                {
                    return _device.AudioEndpointVolume.MasterVolumeLevelScalar;
                }
                return 1.0f; // Default to 100%
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting volume: {ex.Message}");
                return 1.0f;
            }
        }

        private void SetSystemVolume(float level)
        {
            try
            {
                if (_device != null)
                {
                    _device.AudioEndpointVolume.MasterVolumeLevelScalar = level;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting volume: {ex.Message}");
            }
        }

        // Make sure to dispose resources
        protected override void OnUnloaded(RoutedEventArgs e)
        {
            base.OnUnloaded(e);
            _device?.Dispose();
            _deviceEnumerator?.Dispose();
        }
    }
}