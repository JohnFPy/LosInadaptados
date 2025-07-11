using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Project.presentation.components;
using Project.domain.services;

namespace Project.application.components
{
    public class emotionsStatistics : INotifyPropertyChanged
    {
        private string _emotionsStatisticsText = "Estadísticas de Emociones";
        private readonly DatabaseEmotionFetcher _databaseEmotionFetcher = new();

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

            container.Children.Clear();

            var todayStatsCard = CardBuilder.CreateStandard()
                .WithTitle("Emociones de Hoy")
                .WithDescription(_databaseEmotionFetcher.GetTodayEmotionSummary())
                .Build();

            var totalStatsCard = CardBuilder.CreateStandard()
                .WithTitle("Emociones Totales")
                .WithDescription(_databaseEmotionFetcher.GetTotalEmotionSummary())
                .Build();

            var favoriteEmotionCard = CardBuilder.CreateStandard()
                .WithTitle("Emoción Más Registrada")
                .WithDescription(_databaseEmotionFetcher.GetMostRegisteredEmotionSummary())
                .Build();

            var weeklyStatsCard = CardBuilder.CreateStandard()
                .WithTitle("Emociones Semanales")
                .WithDescription(_databaseEmotionFetcher.GetWeeklyEmotionSummary())
                .Build();

            container.Children.Add(todayStatsCard);
            container.Children.Add(totalStatsCard);
            container.Children.Add(favoriteEmotionCard);
            container.Children.Add(weeklyStatsCard);
        }
    }
}
