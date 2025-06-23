using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Project.presentation.components;

namespace Project.presentation.components
{
    public partial class Card : UserControl
    {
        private TextBlock _titleTextBlock;
        private Button _titleButton;
        private ContentControl _titleContent;
        private TextBlock _descriptionText;
        private Audio _audioPlayer;
        private bool _isPlaying = false; // Add state tracking

        public static readonly StyledProperty<string> AudioFileNameProperty =
            AvaloniaProperty.Register<Card, string>("AudioFileName");

        public static readonly StyledProperty<string> TitleTextProperty =
            AvaloniaProperty.Register<Card, string>("TitleText");

        public static readonly StyledProperty<string> CardDescriptionProperty =
            AvaloniaProperty.Register<Card, string>("CardDescription");

        public static readonly StyledProperty<bool> UseButtonTitleProperty =
            AvaloniaProperty.Register<Card, bool>("UseButtonTitle", true);

        // Store the original button text
        private string _originalButtonText;

        public string AudioFileName
        {
            get => GetValue(AudioFileNameProperty);
            set => SetValue(AudioFileNameProperty, value);
        }

        public string TitleText
        {
            get => GetValue(TitleTextProperty);
            set => SetValue(TitleTextProperty, value);
        }

        public string CardDescription
        {
            get => GetValue(CardDescriptionProperty);
            set => SetValue(CardDescriptionProperty, value);
        }

        public bool UseButtonTitle
        {
            get => GetValue(UseButtonTitleProperty);
            set => SetValue(UseButtonTitleProperty, value);
        }

        public Card()
        {
            InitializeComponent();
            this.PropertyChanged += OnPropertyChanged;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            _titleContent = this.FindControl<ContentControl>("TitleContent");
            _descriptionText = this.FindControl<TextBlock>("DescriptionText");

            // Define the blue accent color
            var accentBlue = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#3CADE0"));

            _titleTextBlock = new TextBlock
            {
                FontWeight = Avalonia.Media.FontWeight.SemiBold,
                FontSize = 16,
                Foreground = accentBlue
            };

            _titleButton = new Button
            {
                FontWeight = Avalonia.Media.FontWeight.SemiBold,
                FontSize = 16,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                Padding = new Thickness(0),
                Background = Avalonia.Media.Brushes.Transparent,
                Foreground = accentBlue,
                Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
            };

            // Initialize audio functionality
            InitializeAudio();

            UpdateTitleContent();
        }

        private void InitializeAudio()
        {
            // Create the Audio instance but don't add it to the visual tree
            _audioPlayer = new Audio();

            // Important: Set properties AFTER creating the instance, not during initialization
            if (!string.IsNullOrEmpty(AudioFileName))
            {
                _audioPlayer.AudioFileName = AudioFileName;
            }
            else
            {
                // Set a default to avoid null reference issues
                _audioPlayer.AudioFileName = "relaxingPiano.mp3";
            }

            if (!string.IsNullOrEmpty(TitleText))
            {
                _audioPlayer.ButtonText = TitleText;
                _originalButtonText = TitleText; // Store original text
            }

            if (!string.IsNullOrEmpty(CardDescription))
            {
                _audioPlayer.Description = CardDescription;
            }

            _audioPlayer.IsVisible = false; // We don't want to show the Audio control

            // Set up property change notifications to update the audio player
            this.PropertyChanged += (s, e) =>
            {
                if (e.Property == AudioFileNameProperty && _audioPlayer != null)
                {
                    _audioPlayer.AudioFileName = AudioFileName ?? "relaxingPiano.mp3";
                }
                else if (e.Property == TitleTextProperty && _audioPlayer != null && !_isPlaying)
                {
                    // Only update original button text if we're not currently playing
                    _originalButtonText = TitleText ?? string.Empty;
                }
            };
        }

        private void OnPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == TitleTextProperty)
            {
                if (!_isPlaying) // Only update if not playing
                {
                    _titleTextBlock.Text = TitleText;
                    _titleButton.Content = TitleText;
                    _originalButtonText = TitleText; // Store original text
                }

                if (_audioPlayer != null) _audioPlayer.ButtonText = TitleText;
            }
            else if (e.Property == CardDescriptionProperty)
            {
                _descriptionText.Text = CardDescription;
                if (_audioPlayer != null) _audioPlayer.Description = CardDescription;
            }
            else if (e.Property == UseButtonTitleProperty)
            {
                UpdateTitleContent();
            }
        }

        private void UpdateTitleContent()
        {
            if (UseButtonTitle)
            {
                // Remove old handler to avoid multiple registrations
                _titleButton.Click -= OnTitleButtonClick;

                _titleButton.Content = _isPlaying ? "Detener audio" : TitleText;
                _titleContent.Content = _titleButton;
                _titleButton.Click += OnTitleButtonClick;
            }
            else
            {
                _titleTextBlock.Text = TitleText;
                _titleContent.Content = _titleTextBlock;
            }
        }

        private void OnTitleButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ToggleAudio();
        }

        private void ToggleAudio()
        {
            if (_audioPlayer == null) return;

            if (_isPlaying)
            {
                // Stop audio
                _audioPlayer.StopAudio();
                _isPlaying = false;

                // Reset button text to original
                _titleButton.Content = _originalButtonText;
                Console.WriteLine($"Stopping audio: {AudioFileName}");
            }
            else
            {
                // Play audio
                _audioPlayer.PlayAudio();
                _isPlaying = true;

                // Change button text
                _titleButton.Content = "Detener audio";
                Console.WriteLine($"Playing audio via Audio component: {AudioFileName}");
            }
        }
    }
}