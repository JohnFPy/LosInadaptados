using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Project.domain.services
{
    /// Manejador de estadisticas de reproducci�n de audio que en el futuro enviara todo a infrastructure
    public class AudioStatisticsProcessor
    {
        private static AudioStatisticsProcessor? _instance;
        private static readonly object _lock = new object();

        private AudioStatisticsProcessor() { }

        public static AudioStatisticsProcessor Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new AudioStatisticsProcessor();
                    }
                }
                return _instance;
            }
        }

        /// Procesa las estad�sticas diarias de reproducci�n de audio
        public void ProcessDailyStatistics(Dictionary<string, int> todayStatistics)
        {
            if (todayStatistics == null || todayStatistics.Count == 0)
            {
                Debug.WriteLine("ESTAD�STICAS DE AUDIO - HOY");
                Debug.WriteLine("No hay datos de reproducci�n para hoy");
                return;
            }

            Debug.WriteLine("ESTAD�STICAS DE AUDIO - HOY");
            Debug.WriteLine($"Fecha: {DateTime.Now:dddd, dd/MM/yyyy}");

            var totalSeconds = 0;
            var hasData = false;

            foreach (var kvp in todayStatistics)
            {
                if (kvp.Value > 0)
                {
                    hasData = true;
                    totalSeconds += kvp.Value;
                    
                    var minutes = kvp.Value / 60;
                    var seconds = kvp.Value % 60;
                    var typeName = GetAudioTypeName(kvp.Key);
                    // Saca minutos y segundos de la reproducci�n de c/u
                    Debug.WriteLine($"{typeName}: {minutes}m {seconds}s ({kvp.Value} segundos)");
                }
            }

            if (hasData)
            {
                var totalMinutes = totalSeconds / 60;
                var remainingSeconds = totalSeconds % 60;
                
                Debug.WriteLine($"TOTAL: {totalMinutes}m {remainingSeconds}s ({totalSeconds} segundos)");
                
                // Mostrar estad�sticas adicionales (solo el m�s escuchado)
                ShowBasicInsights(todayStatistics);
            }
            else
            {
                Debug.WriteLine("No hay reproducci�n registrada para hoy");
            }
        }

        /// Insigiths de mas escuchado
        private void ShowBasicInsights(Dictionary<string, int> statistics)
        {
            Debug.WriteLine("=======================================");
            Debug.WriteLine("INSIGHTS:");

            // Encontrar el tipo m�s usado
            var mostUsed = "";
            var maxTime = 0;
            foreach (var kvp in statistics)
            {
                if (kvp.Value > maxTime)
                {
                    maxTime = kvp.Value;
                    mostUsed = kvp.Key;
                }
            }

            if (!string.IsNullOrEmpty(mostUsed))
            {
                var typeName = GetAudioTypeName(mostUsed);
                Debug.WriteLine($"M�s escuchado: {typeName}");
            }
        }

        private string GetAudioTypeName(string audioType)
        {
            return audioType.ToLowerInvariant() switch
            {
                "japon" => "Japon�s",
                "ethno" => "�tnico",
                "piano" => "Piano Relajante",
                _ => "Desconocido"
            };
        }
    }
}