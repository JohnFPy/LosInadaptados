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

        public void RefreshTodayStatistics(StackPanel container)
        {
            if (container?.Children.Count >= 1 && container.Children[0] is UserControl todayCard)
            {
                var updated = CardBuilder.CreateStandard()
                    .WithTitle("Emociones de Hoy")
                    .WithDescription(_databaseEmotionFetcher.GetTodayEmotionSummary())
                    .Build();

                container.Children[0] = updated;
            }
        }

        public void RefreshTotalStatistics(StackPanel container)
        {
            if (container?.Children.Count >= 2 && container.Children[1] is UserControl totalCard)
            {
                var updated = CardBuilder.CreateStandard()
                    .WithTitle("Emociones Totales")
                    .WithDescription(_databaseEmotionFetcher.GetTotalEmotionSummary())
                    .Build();

                container.Children[1] = updated;
            }
        }

        public void RefreshFavoriteEmotion(StackPanel container)
        {
            if (container?.Children.Count >= 3 && container.Children[2] is UserControl favCard)
            {
                var updated = CardBuilder.CreateStandard()
                    .WithTitle("Emoción Más Registrada")
                    .WithDescription(_databaseEmotionFetcher.GetMostRegisteredEmotionSummary())
                    .Build();

                container.Children[2] = updated;
            }
        }

        public void RefreshWeeklyStatistics(StackPanel container)
        {
            if (container?.Children.Count >= 4 && container.Children[3] is UserControl weekCard)
            {
                var updated = CardBuilder.CreateStandard()
                    .WithTitle("Emociones Semanales")
                    .WithDescription(_databaseEmotionFetcher.GetWeeklyEmotionSummary())
                    .Build();

                container.Children[3] = updated;
            }
        }

        public void RefreshAllStatistics(StackPanel container)
        {
            CreateStatisticsCards(container);
        }
    }
}
