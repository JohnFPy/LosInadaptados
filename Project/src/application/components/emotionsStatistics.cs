using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Project.presentation.components;

namespace Project.application.components
{
    public class emotionsStatisticsViewModel : INotifyPropertyChanged
    {
        private string _emotionsStatisticsText = "Estadísticas de Emociones";

        public string EmotionsStatisticsText
        {
            get => _emotionsStatisticsText;
            set
            {
                _emotionsStatisticsText = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CreateStatisticsCards(StackPanel container)
        {
            if (container == null) return;

            // Limpiar el contenedor
            container.Children.Clear();

            // Datos de ejemplo para las estadísticas de emociones
            string todayEmotionsDescription = GetTodayEmotionsExample();
            string totalEmotionsDescription = GetTotalEmotionsExample();
            string favoriteEmotionDescription = GetMostRegisteredEmotionExample();
            string weeklyEmotionsDescription = GetWeeklyEmotionsExample();

            // Crear cards para diferentes estadísticas de emociones
            var todayEmotionsCard = CardBuilder.CreateStandard()
                .WithTitle("Emociones de Hoy")
                .WithDescription(todayEmotionsDescription)
                .Build();

            var totalEmotionsCard = CardBuilder.CreateStandard()
                .WithTitle("Emociones Totales")
                .WithDescription(totalEmotionsDescription)
                .Build();

            var favoriteEmotionCard = CardBuilder.CreateStandard()
                .WithTitle("Emoción Más Registrada")
                .WithDescription(favoriteEmotionDescription)
                .Build();

            var weeklyEmotionsCard = CardBuilder.CreateStandard()
                .WithTitle("Emociones Semanales")
                .WithDescription(weeklyEmotionsDescription)
                .Build();

            // Añadir las cards al contenedor
            container.Children.Add(todayEmotionsCard);
            container.Children.Add(totalEmotionsCard);
            container.Children.Add(favoriteEmotionCard);
            container.Children.Add(weeklyEmotionsCard);
        }

        /// <summary>
        /// Actualiza las estadísticas de emociones
        /// </summary>
        public void RefreshStatistics(StackPanel container)
        {
            CreateStatisticsCards(container);
        }

        private string GetTodayEmotionsExample()
        {
            var today = DateTime.Now;
            return $"Fecha: {today:dd/MM/yyyy}\nEmoción de hoy: Feliz\nHora de registro: {today:HH:mm}";
        }

        private string GetTotalEmotionsExample()
        {
            return "Período: Enero 2025 - Presente\nRegistros por emoción:\n• Feliz: 45 registros\n• Triste: 23 registros\n• Enojado: 15 registros\n• Preocupado: 28 registros\n• Serio: 16 registros\nEmoción más registrada: Feliz";
        }

        private string GetMostRegisteredEmotionExample()
        {
            return "Feliz\n45 registros\nÚltima vez: Hoy 14:30";
        }

        private string GetWeeklyEmotionsExample()
        {
            var startOfWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + 1);
            var endOfWeek = startOfWeek.AddDays(6);

            return $"Período: {startOfWeek:dd/MM} - {endOfWeek:dd/MM/yyyy}\nRegistros por emoción:\n• Feliz: 12 registros\n• Triste: 6 registros\n• Enojado: 3 registros\n• Preocupado: 5 registros\n• Serio: 2 registros\nRegistros totales: 28";
        }
    }
}