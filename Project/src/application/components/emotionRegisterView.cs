using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Project.application.services;
using Project.domain.services;
using Project.infrastucture;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Project.application.components
{
    public class Emotion
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public bool IsAddButton { get; set; } = false;
        public bool IsLocalImage { get; set; } = false;
    }

    public class emotionRegisterView : INotifyPropertyChanged
    {
        private readonly dayView _day;
        private readonly EmotionLogCRUD _emotionLogCRUD = new();
        private readonly DatabaseEmotionFetcher _emotionFetcher = new();
        private Window? _parentWindow;

        public ObservableCollection<Emotion> Emotions { get; set; } = new ObservableCollection<Emotion>
        {
            new Emotion { Name = "Feliz", ImagePath = "avares://Project/resources/emotions/feliz.png" },
            new Emotion { Name = "Triste", ImagePath = "avares://Project/resources/emotions/triste.png" },
            new Emotion { Name = "Enojado", ImagePath = "avares://Project/resources/emotions/enojado.png" },
            new Emotion { Name = "Preocupado", ImagePath = "avares://Project/resources/emotions/preocupado.png" },
            new Emotion { Name = "Serio", ImagePath = "avares://Project/resources/emotions/serio.png" },
            new Emotion { Name = "Emoción personalizada", ImagePath = "avares://Project/resources/emotions/add.png", IsAddButton = true }
        };

        private Emotion? _selectedEmotion;
        public Emotion? SelectedEmotion
        {
            get => _selectedEmotion;
            set
            {
                if (_selectedEmotion != value)
                {
                    _selectedEmotion = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanSave));
                    SaveCommand.RaiseCanExecuteChanged();

                    if (_selectedEmotion?.IsAddButton == true)
                    {
                        OpenCanvasWindow();
                        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                        {
                            SelectedEmotion = null;
                        });
                    }
                }
            }
        }

        public bool CanSave => SelectedEmotion != null;

        private async void OpenCanvasWindow()
        {
            if (_parentWindow != null)
            {
                var canvasWindow = new canvas();
                var result = await canvasWindow.ShowDialog<SavedEmotion>(_parentWindow);

                if (result != null)
                {
                    var customEmotion = new Emotion
                    {
                        Name = result.Name,
                        ImagePath = result.Path,
                        IsLocalImage = true
                    };

                    Emotions.Insert(Emotions.Count - 1, customEmotion);
                }

            }
        }

        private string _comment = "";
        public string Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                OnPropertyChanged();
            }
        }

        private string? _selectedImagePath;
        public string? SelectedImagePath
        {
            get => _selectedImagePath;
            set
            {
                _selectedImagePath = value;
                OnPropertyChanged();
            }
        }

        public relayCommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SelectImageCommand { get; }

        public event EventHandler? RequestClose;

        public emotionRegisterView(dayView day, Window? parentWindow = null)
        {
            _day = day;
            _parentWindow = parentWindow;

            SaveCommand = new relayCommand(_ => Save(), _ => CanSave);
            CancelCommand = new relayCommand(_ => RequestClose?.Invoke(this, EventArgs.Empty));
            SelectImageCommand = new relayCommand(async _ => await SelectImageAsync());

            // Intentar cargar emoción previamente registrada
            string dateId = _day.DateId ?? DateTime.Today.ToString("yyyy-MM-dd");
            var log = _emotionLogCRUD.GetEmotionByDate(dateId);

            if (log != null)
            {
                string? name = null;
                bool isPersonalized = false;

                if (log.Value.idEmotion.HasValue)
                {
                    name = _emotionFetcher.GetEmotionNameById(log.Value.idEmotion.Value);
                }
                else if (log.Value.idPersonalized.HasValue)
                {
                    name = _emotionFetcher.GetPersonalizedEmotionNameById(log.Value.idPersonalized.Value);
                    isPersonalized = true;
                }

                if (name != null)
                {
                    var existing = FindMatchingEmotion(name, isPersonalized);
                    if (existing != null)
                    {
                        SelectedEmotion = existing;
                    }
                    else
                    {
                        // Si es personalizada y no está, agregarla a la lista
                        Emotions.Insert(Emotions.Count - 1, new Emotion
                        {
                            Name = name,
                            ImagePath = "avares://Project/resources/emotions/custom.png", // puedes personalizar
                            IsLocalImage = true
                        });

                        SelectedEmotion = Emotions[^2]; // penúltimo (antes del botón '+')
                    }
                }
            }
        }


        private async Task SelectImageAsync()
        {
            if (_parentWindow == null) return;

            var options = new FilePickerOpenOptions
            {
                AllowMultiple = false,
                Title = "Selecciona una imagen",
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new FilePickerFileType("Imágenes")
                    {
                        Patterns = new[] { "*.png", "*.jpg", "*.jpeg" }
                    }
                }
            };

            var result = await _parentWindow.StorageProvider.OpenFilePickerAsync(options);
            if (result.Count > 0)
            {
                var file = result[0];
                SelectedImagePath = file.Path.LocalPath;
            }
        }

        private void Save()
        {
            if (SelectedEmotion == null)
                return;

            string dateId = _day.DateId ?? DateTime.Today.ToString("yyyy-MM-dd");
            long? idEmotion = null;
            long? idPersonalized = null;

            if (SelectedEmotion.IsAddButton)
            {
                return;
            }
            else if (SelectedEmotion.IsLocalImage)
            {
                idPersonalized = _emotionFetcher.GetPersonalizedEmotionIdByName(SelectedEmotion.Name);
            }
            else
            {
                idEmotion = _emotionFetcher.GetEmotionIdByName(SelectedEmotion.Name);
            }

            if (idEmotion == null && idPersonalized == null)
            {
                return;
            }

            _emotionLogCRUD.RegisterEmotion(dateId, idEmotion, idPersonalized);

            // Actualizar color visual del día
            var colorHex = emotionColorMapper.GetColor(SelectedEmotion.Name, isPersonalized: idPersonalized != null);
            _day.EmotionColor = new SolidColorBrush(Color.Parse(colorHex));

            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        private Emotion? FindMatchingEmotion(string name, bool isPersonalized)
        {
            foreach (var emo in Emotions)
            {
                if (emo.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                    emo.IsLocalImage == isPersonalized)
                {
                    return emo;
                }
            }
            return null;
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


}
