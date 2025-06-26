using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using Avalonia.Threading;
using System;
using System.IO;
using System.Reflection;
using Project.presentation.components;

namespace Project.presentation.components
{
    public partial class Card : UserControl
    {
        private TextBlock _titleTextBlock;
        private Button _titleButton;
        private ContentControl _titleContent;
        private TextBlock _descriptionText;
        private Border _imageContainer;
        private Image _cardImage;
        private Audio _audioPlayer;
        private bool _isPlaying = false;

        // Propiedades existentes
        public static readonly StyledProperty<string> AudioFileNameProperty =
            AvaloniaProperty.Register<Card, string>("AudioFileName");

        public static readonly StyledProperty<string> TitleTextProperty =
            AvaloniaProperty.Register<Card, string>("TitleText");

        public static readonly StyledProperty<string> CardDescriptionProperty =
            AvaloniaProperty.Register<Card, string>("CardDescription");

        public static readonly StyledProperty<bool> UseButtonTitleProperty =
            AvaloniaProperty.Register<Card, bool>("UseButtonTitle", true);

        // Nueva propiedad para la imagen
        public static readonly StyledProperty<string> ImageResourceProperty =
            AvaloniaProperty.Register<Card, string>("ImageResource");

        private string _originalButtonText;

        // Getters y setters para propiedades existentes
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

        // Getter y setter para la nueva propiedad de imagen
        public string ImageResource
        {
            get => GetValue(ImageResourceProperty);
            set => SetValue(ImageResourceProperty, value);
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
            _imageContainer = this.FindControl<Border>("ImageContainer");
            _cardImage = this.FindControl<Image>("CardImage");

            // Color de acento para t�tulos
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

            InitializeAudio();
            UpdateTitleContent();

            // Cargar imagen si est� configurada
            if (!string.IsNullOrEmpty(ImageResource))
            {
                LoadImageFromResource(ImageResource);
            }
        }

        private void InitializeAudio()
        {
            // [C�digo existente de inicializaci�n de audio]
            _audioPlayer = new Audio();

            if (!string.IsNullOrEmpty(AudioFileName))
            {
                _audioPlayer.AudioFileName = AudioFileName;
            }
            else
            {
                _audioPlayer.AudioFileName = "relaxingPiano.mp3";
            }

            if (!string.IsNullOrEmpty(TitleText))
            {
                _audioPlayer.ButtonText = TitleText;
                _originalButtonText = TitleText;
            }

            if (!string.IsNullOrEmpty(CardDescription))
            {
                _audioPlayer.Description = CardDescription;
            }

            _audioPlayer.IsVisible = false;

            // Register with AudioManager and provide callback for when audio is stopped externally
            _audioPlayer.RegisterWithAudioManager(OnAudioStoppedByManager);

            // Agregar observador para cambios en la propiedad de imagen
            this.PropertyChanged += (s, e) =>
            {
                if (e.Property == ImageResourceProperty)
                {
                    LoadImageFromResource(ImageResource);
                }
                // [Otros observadores existentes]
                else if (e.Property == AudioFileNameProperty && _audioPlayer != null)
                {
                    _audioPlayer.AudioFileName = AudioFileName ?? "relaxingPiano.mp3";
                }
                else if (e.Property == TitleTextProperty && _audioPlayer != null && !_isPlaying)
                {
                    _originalButtonText = TitleText ?? string.Empty;
                }
            };
        }

        /// <summary>
        /// Callback method called by AudioManager when this card's audio is stopped externally
        /// </summary>
        private void OnAudioStoppedByManager()
        {
            // Use Dispatcher to ensure UI updates happen on the UI thread
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (_isPlaying)
                {
                    _isPlaying = false;
                    if (_titleButton != null && !string.IsNullOrEmpty(_originalButtonText))
                    {
                        _titleButton.Content = _originalButtonText;
                    }
                    Console.WriteLine($"Card audio stopped by AudioManager: {TitleText}");
                }
            });
        }

        // M�todo para cargar la imagen desde recursos embebidos
        private void LoadImageFromResource(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                _imageContainer.IsVisible = false;
                return;
            }

            try
            {
                // Asegurarse de que la ruta est� correctamente formateada
                if (!resourcePath.StartsWith("resources/"))
                    resourcePath = "resources/" + resourcePath;

                var assembly = Assembly.GetExecutingAssembly();
                string fullResourceName = null;

                // Buscar el recurso por nombre
                foreach (var name in assembly.GetManifestResourceNames())
                {
                    if (name.EndsWith(resourcePath.Replace('/', '.'), StringComparison.OrdinalIgnoreCase))
                    {
                        fullResourceName = name;
                        break;
                    }
                }

                if (fullResourceName != null)
                {
                    // Cargar la imagen desde el recurso
                    using (var stream = assembly.GetManifestResourceStream(fullResourceName))
                    {
                        if (stream != null)
                        {
                            var bitmap = new Bitmap(stream);
                            _cardImage.Source = bitmap;

                            // Hacer visible y dimensionar el contenedor de la imagen
                            _imageContainer.IsVisible = true;
                            _imageContainer.Width = 60;  // Tama�o fijo cuadrado 60x60
                            _imageContainer.Height = 60;

                            Console.WriteLine($"Imagen cargada correctamente: {resourcePath}");
                            return;
                        }
                    }
                }

                Console.WriteLine($"No se pudo encontrar el recurso de imagen: {resourcePath}");
                _imageContainer.IsVisible = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar la imagen: {ex.Message}");
                _imageContainer.IsVisible = false;
            }
        }

        private void OnPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            // [C�digo existente de manejo de propiedades]
            if (e.Property == TitleTextProperty)
            {
                if (!_isPlaying)
                {
                    _titleTextBlock.Text = TitleText;
                    _titleButton.Content = TitleText;
                    _originalButtonText = TitleText;
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
            else if (e.Property == ImageResourceProperty)
            {
                LoadImageFromResource(ImageResource);
            }
        }

        private void UpdateTitleContent()
        {
            if (UseButtonTitle)
            {
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
                _audioPlayer.StopAudio();
                _isPlaying = false;
                _titleButton.Content = _originalButtonText;
            }
            else
            {
                // The AudioManager will automatically stop any currently playing audio
                // before allowing this one to play
                _audioPlayer.PlayAudio();
                _isPlaying = true;
                _titleButton.Content = "Detener audio";
            }
        }

        /// <summary>
        /// Forces this card to stop playing audio if it's currently playing.
        /// This method is called by the AudioManager when another audio starts playing.
        /// </summary>
        internal void ForceStopAudio()
        {
            if (_isPlaying && _audioPlayer != null)
            {
                _audioPlayer.StopAudio();
                _isPlaying = false;
                if (_titleButton != null)
                {
                    _titleButton.Content = _originalButtonText;
                }
            }
        }

        /// <summary>
        /// Gets whether this card's audio is currently playing
        /// </summary>
        public bool IsAudioPlaying => _isPlaying;

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            // Make sure we stop audio when the card is removed from the visual tree
            if (_isPlaying && _audioPlayer != null)
            {
                _audioPlayer.StopAudio();
                _isPlaying = false;
            }
            
            // Unregister from AudioManager to clean up callbacks
            if (_audioPlayer != null)
            {
                AudioManager.Instance.UnregisterAudio(_audioPlayer);
            }
            
            base.OnDetachedFromVisualTree(e);
        }
    }
}