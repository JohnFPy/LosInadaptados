using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Platform.Storage;
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
        public bool IsAddButton { get; set; } = false; // For Custom Emotions
    }



    public class emotionRegisterView : INotifyPropertyChanged
    {
        private readonly dayView _day;
        private Window? _parentWindow;

        // READ FROM DATABASE ###############
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
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
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
                            ImagePath = result.Path
                        };

                        Emotions.Insert(Emotions.Count - 1, customEmotion);
                    }

                }

                // UPDATE DATABASE ###############
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
        }


        private async Task SelectImageAsync()
        {
            if (_parentWindow == null)
                return;

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
                var path = file.Path.LocalPath;
                SelectedImagePath = path;
            }
        }

        private void Save()
        {
            // DATA BASE CONNECTION ###############

            // Color visual update
            _day.EmotionColor = new SolidColorBrush(Colors.Yellow);

            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
