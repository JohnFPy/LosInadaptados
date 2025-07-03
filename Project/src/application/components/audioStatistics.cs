using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Project.presentation.components;

namespace Project.application.components
{
    internal class audioStatistics : INotifyPropertyChanged
    {
        private string _audioStatisticsText = "Estadísticas de Audio";

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

            // Crear cards para diferentes estadísticas de audio
            var todayStatsCard = CardBuilder.CreateStandard()
                .WithTitle("Estadísticas de Hoy")
                .WithDescription("Tiempo de reproducción de audio del día actual")
                .Build();

            var totalStatsCard = CardBuilder.CreateStandard()
                .WithTitle("Estadísticas Totales")
                .WithDescription("Tiempo total acumulado de reproducción de audio")
                .Build();

            var favoriteAudioCard = CardBuilder.CreateStandard()
                .WithTitle("Audio Más Reproducido")
                .WithDescription("El tipo de audio que más has escuchado")
                .Build();

            var weeklyStatsCard = CardBuilder.CreateStandard()
                .WithTitle("Estadísticas Semanales")
                .WithDescription("Resumen de tu actividad de la última semana")
                .Build();

            // Añadir las cards al contenedor
            container.Children.Add(todayStatsCard);
            container.Children.Add(totalStatsCard);
            container.Children.Add(favoriteAudioCard);
            container.Children.Add(weeklyStatsCard);
        }
    }
}