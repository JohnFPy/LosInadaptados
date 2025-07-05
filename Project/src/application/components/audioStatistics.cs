using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Project.presentation.components;
using Project.domain.services;

namespace Project.application.components
{
    internal class audioStatistics : INotifyPropertyChanged
    {
        private string _audioStatisticsText = "Estadísticas de Audio";
        private readonly DatabaseAudioFetcher _databaseAudioFetcher = new DatabaseAudioFetcher();

        public string AudioStatisticsText
        {
            get => _audioStatisticsText;
            set
            {
                _audioStatisticsText = value;
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

            // Obtener estadísticas de hoy desde la base de datos (datos frescos)
            string todayStatsDescription = _databaseAudioFetcher.GetTodayStatisticsSummary();

            // Obtener estadísticas totales desde la base de datos (datos frescos)
            string totalStatsDescription = _databaseAudioFetcher.GetTotalStatisticsSummary();

            // Obtener audio más reproducido desde la base de datos (datos frescos)
            string favoriteAudioDescription = _databaseAudioFetcher.GetMostPlayedAudioSummary();

            // Obtener estadísticas semanales desde la base de datos (datos frescos)
            string weeklyStatsDescription = _databaseAudioFetcher.GetWeeklyStatisticsSummary();

            // Crear cards para diferentes estadísticas de audio
            var todayStatsCard = CardBuilder.CreateStandard()
                .WithTitle("Estadísticas de Hoy")
                .WithDescription(todayStatsDescription)
                .Build();

            var totalStatsCard = CardBuilder.CreateStandard()
                .WithTitle("Estadísticas Totales")
                .WithDescription(totalStatsDescription)
                .Build();

            var favoriteAudioCard = CardBuilder.CreateStandard()
                .WithTitle("Audio Más Reproducido")
                .WithDescription(favoriteAudioDescription)
                .Build();

            var weeklyStatsCard = CardBuilder.CreateStandard()
                .WithTitle("Estadísticas Semanales")
                .WithDescription(weeklyStatsDescription)
                .Build();

            // Añadir las cards al contenedor
            container.Children.Add(todayStatsCard);
            container.Children.Add(totalStatsCard);
            container.Children.Add(favoriteAudioCard);
            container.Children.Add(weeklyStatsCard);
        }

        /// <summary>
        /// Actualiza las estadísticas del día actual
        /// </summary>
        public void RefreshTodayStatistics(StackPanel container)
        {
            if (container == null || container.Children.Count == 0) return;

            // Actualizar solo la primera card (estadísticas de hoy)
            if (container.Children[0] is UserControl todayCard)
            {
                string updatedDescription = _databaseAudioFetcher.GetTodayStatisticsSummary();

                // Recrear la card con la información actualizada
                var newTodayCard = CardBuilder.CreateStandard()
                    .WithTitle("Estadísticas de Hoy")
                    .WithDescription(updatedDescription)
                    .Build();

                container.Children[0] = newTodayCard;
            }
        }

        /// <summary>
        /// Actualiza las estadísticas totales
        /// </summary>
        public void RefreshTotalStatistics(StackPanel container)
        {
            if (container == null || container.Children.Count < 2) return;

            // Actualizar la segunda card (estadísticas totales)
            if (container.Children[1] is UserControl totalCard)
            {
                string updatedDescription = _databaseAudioFetcher.GetTotalStatisticsSummary();

                // Recrear la card con la información actualizada
                var newTotalCard = CardBuilder.CreateStandard()
                    .WithTitle("Estadísticas Totales")
                    .WithDescription(updatedDescription)
                    .Build();

                container.Children[1] = newTotalCard;
            }
        }

        /// <summary>
        /// Actualiza el audio más reproducido
        /// </summary>
        public void RefreshFavoriteAudio(StackPanel container)
        {
            if (container == null || container.Children.Count < 3) return;

            // Actualizar la tercera card (audio más reproducido)
            if (container.Children[2] is UserControl favoriteCard)
            {
                string updatedDescription = _databaseAudioFetcher.GetMostPlayedAudioSummary();

                // Recrear la card con la información actualizada
                var newFavoriteCard = CardBuilder.CreateStandard()
                    .WithTitle("Audio Más Reproducido")
                    .WithDescription(updatedDescription)
                    .Build();

                container.Children[2] = newFavoriteCard;
            }
        }

        /// <summary>
        /// Actualiza las estadísticas semanales
        /// </summary>
        public void RefreshWeeklyStatistics(StackPanel container)
        {
            if (container == null || container.Children.Count < 4) return;

            // Actualizar la cuarta card (estadísticas semanales)
            if (container.Children[3] is UserControl weeklyCard)
            {
                string updatedDescription = _databaseAudioFetcher.GetWeeklyStatisticsSummary();

                // Recrear la card con la información actualizada
                var newWeeklyCard = CardBuilder.CreateStandard()
                    .WithTitle("Estadísticas Semanales")
                    .WithDescription(updatedDescription)
                    .Build();

                container.Children[3] = newWeeklyCard;
            }
        }

        /// <summary>
        /// Refresca todas las estadísticas obteniendo datos frescos de la base de datos
        /// </summary>
        public void RefreshAllStatistics(StackPanel container)
        {
            // Recreamos todas las cards con datos frescos
            CreateStatisticsCards(container);
        }
    }
}