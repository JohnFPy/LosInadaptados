﻿using Avalonia;
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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;

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
        private Window? _parentWindow;

        public ObservableCollection<Emotion> Emotions { get; set; } = new();
        private String notFoundPath = "avares://Project/resources/emotions/notfound.png";

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
                    _emotionLogCRUD.SaveNewPersonalizedEmotion(result.Name, result.Path);

                    var customEmotion = new Emotion
                    {
                        Name = result.Name,
                        ImagePath = result.Path,
                        IsLocalImage = true
                    };

                    Emotions.Insert(Emotions.Count - 1, customEmotion);

                    SelectedEmotion = customEmotion;

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
        public ICommand ClearEmotionCommand { get; }


        public event EventHandler? RequestClose;

        public emotionRegisterView(dayView day, Window? parentWindow = null)
        {
            _day = day;
            _parentWindow = parentWindow;

            SaveCommand = new relayCommand(_ => Save(), _ => CanSave);
            CancelCommand = new relayCommand(_ => RequestClose?.Invoke(this, EventArgs.Empty));
            SelectImageCommand = new relayCommand(async _ => await SelectImageAsync());
            ClearEmotionCommand = new relayCommand(_ => ClearEmotion());

            LoadStandardEmotions();
            LoadAllPersonalizedEmotions();

            // Load emotion for day (selected)
            string dateId = _day.DateId ?? DateTime.Today.ToString("yyyy-MM-dd");
            var log = _emotionLogCRUD.GetEmotionByDate(dateId);

            if (log != null)
            {
                string? name = null;
                bool isPersonalized = false;

                if (log.Value.idEmotion.HasValue)
                {
                    name = _emotionLogCRUD.GetEmotionNameById(log.Value.idEmotion.Value);
                }
                else if (log.Value.idPersonalized.HasValue)
                {
                    name = _emotionLogCRUD.GetPersonalizedEmotionNameById(log.Value.idPersonalized.Value);
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
                        // If personalized but not present
                        string? path = _emotionLogCRUD.GetPersonalizedEmotionImagePathByName(name);

                        Emotions.Insert(Emotions.Count - 1, new Emotion
                        {
                            Name = name,
                            ImagePath = path ?? notFoundPath,
                            IsLocalImage = true
                        });

                        SelectedEmotion = Emotions[^2]; // Before '+'
                    }

                }
            }
        }

        private void LoadAllPersonalizedEmotions()
        {
            var personalizedEmotions = _emotionLogCRUD.GetAllPersonalizedEmotionsWithPaths();

            foreach (var kvp in personalizedEmotions)
            {
                var name = kvp.Key;
                var path = kvp.Value;

                // Avoid duplicates
                if (!Emotions.Any(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && e.IsLocalImage))
                {
                    Emotions.Insert(Emotions.Count - 1, new Emotion
                    {
                        Name = name,
                        ImagePath = path ?? notFoundPath,
                        IsLocalImage = true
                    });
                }
            }
        }

        private void LoadStandardEmotions()
        {
            var standardEmotions = _emotionLogCRUD.GetAllStandardEmotionsWithPaths();

            foreach (var kvp in standardEmotions)
            {
                var name = kvp.Key;
                var path = kvp.Value;

                Emotions.Add(new Emotion
                {
                    Name = name,
                    ImagePath = path ?? notFoundPath,
                    IsLocalImage = false
                });
            }

            // Custom emotion button
            Emotions.Add(new Emotion
            {
                Name = "Emoción personalizada",
                ImagePath = "avares://Project/resources/emotions/add.png",
                IsAddButton = true
            });
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
                return;

            if (SelectedEmotion.IsLocalImage)
                idPersonalized = _emotionLogCRUD.GetPersonalizedEmotionIdByName(SelectedEmotion.Name);
            else
                idEmotion = _emotionLogCRUD.GetEmotionIdByName(SelectedEmotion.Name);

            if (idEmotion == null && idPersonalized == null)
            {
                Debug.WriteLine("Error en guardado de emoción: Ids nulos");
                return;
            }

            // Verificar si ya hay registro para la fecha
            var existing = _emotionLogCRUD.GetEmotionByDate(dateId);
            bool success;

            if (existing != null)
            {
                // Si existe, actualizar
                success = _emotionLogCRUD.UpdateEmotion(dateId, idEmotion, idPersonalized);
                if (!success)
                {
                    Debug.WriteLine("Error actualizando emoción");
                    return;
                }
            }
            else
            {
                // Si no existe, registrar nuevo
                success = _emotionLogCRUD.RegisterEmotion(dateId, idEmotion, idPersonalized);
                if (!success)
                {
                    Debug.WriteLine("Error registrando emoción");
                    return;
                }
            }

            // Actualizar color del día
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

        private void ClearEmotion()
        {
            string dateId = _day.DateId ?? DateTime.Today.ToString("yyyy-MM-dd");

            bool deleted = _emotionLogCRUD.DeleteEmotionEntry(dateId);
            if (!deleted)
            {
                Debug.WriteLine("No se pudo limpiar la emoción.");
                return;
            }

            _day.EmotionColor = new SolidColorBrush(Brushes.Transparent.Color);
            SelectedEmotion = null;

            RequestClose?.Invoke(this, EventArgs.Empty);
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


}
