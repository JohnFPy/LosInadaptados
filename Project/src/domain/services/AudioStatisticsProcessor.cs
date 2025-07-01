using System;
using System.Collections.Generic;
using System.Diagnostics;
using Project.infrastucture;

namespace Project.domain.services
{
    public class AudioStatisticsProcessor
    {
        private static AudioStatisticsProcessor? _instance;
        private static readonly object _lock = new object();
        private readonly AudioCRUD _audioCRUD;

        private AudioStatisticsProcessor()
        {
            _audioCRUD = new AudioCRUD();
        }

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

        public void ProcessDailyStatistics(Dictionary<string, int> todayStatistics)
        {
            if (todayStatistics == null || todayStatistics.Count == 0)
            {
                Debug.WriteLine("ESTADISTICAS DE AUDIO - HOY");
                Debug.WriteLine("No hay datos de reproduccion para hoy");
                return;
            }

            var success = _audioCRUD.SaveDailyStatistics(todayStatistics);
            if (success)
            {
                Debug.WriteLine("Estadisticas guardadas en la base de datos");
            }
            else
            {
                Debug.WriteLine("Error guardando estadisticas en la base de datos");
            }

            Debug.WriteLine("ESTADISTICAS DE AUDIO - HOY");
            Debug.WriteLine(string.Format("Fecha: {0:dddd, dd/MM/yyyy}", DateTime.Now));
            Debug.WriteLine("DateId: " + AudioCRUD.GetTodayDateId());

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
                    Debug.WriteLine(string.Format("{0}: {1}m {2}s ({3} segundos)", typeName, minutes, seconds, kvp.Value));
                }
            }

            if (hasData)
            {
                var totalMinutes = totalSeconds / 60;
                var remainingSeconds = totalSeconds % 60;

                Debug.WriteLine(string.Format("TOTAL: {0}m {1}s ({2} segundos)", totalMinutes, remainingSeconds, totalSeconds));

                ShowBasicInsights(todayStatistics);
            }
            else
            {
                Debug.WriteLine("No hay reproduccion registrada para hoy");
            }
        }

        public void RegisterAudioPlayback(string audioType, int seconds)
        {
            if (seconds <= 0) return;

            var dateId = AudioCRUD.GetTodayDateId();
            var success = _audioCRUD.UpdateAudioTime(dateId, audioType, seconds);

            if (success)
            {
                var typeName = GetAudioTypeName(audioType);
                Debug.WriteLine(string.Format("Registrado: {0}s de {1} para {2}", seconds, typeName, dateId));
            }
            else
            {
                Debug.WriteLine("Error registrando reproduccion de " + audioType);
            }
        }

        public Dictionary<string, int> GetTodayStatisticsFromDatabase()
        {
            var dateId = AudioCRUD.GetTodayDateId();
            var dbStats = _audioCRUD.GetAudioTimesByDate(dateId);

            var result = new Dictionary<string, int>();

            if (dbStats.HasValue)
            {
                result["ethno"] = dbStats.Value.ethnoTime;
                result["japon"] = dbStats.Value.japanTime;
                result["piano"] = dbStats.Value.pianoTime;
            }
            else
            {
                result["ethno"] = 0;
                result["japon"] = 0;
                result["piano"] = 0;
            }

            return result;
        }

        private void ShowBasicInsights(Dictionary<string, int> statistics)
        {
            Debug.WriteLine("=======================================");
            Debug.WriteLine("INSIGHTS:");

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
                Debug.WriteLine("Mas escuchado: " + typeName);
            }
        }

        private string GetAudioTypeName(string audioType)
        {
            var audioTypeLower = audioType.ToLowerInvariant();

            if (audioTypeLower == "japon" || audioTypeLower == "japan")
                return "Japones";
            if (audioTypeLower == "ethno" || audioTypeLower == "ethnic")
                return "Etnico";
            if (audioTypeLower == "piano")
                return "Piano";

            // Default case for unknown audio types
            return audioType;
        }
    }
}