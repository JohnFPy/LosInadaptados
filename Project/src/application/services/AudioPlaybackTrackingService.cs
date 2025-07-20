using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Project.domain.services;

namespace Project.application.services
{
    public class AudioPlaybackTrackingService
    {
        private static AudioPlaybackTrackingService? _instance;
        private static readonly object _lock = new object();

        private readonly Dictionary<string, List<int>> _todayPlaybackTimes;
        private readonly Dictionary<string, Stopwatch> _activeStopwatches;
        private string _currentDate;
        private readonly string _dataDirectory;

        private AudioPlaybackTrackingService()
        {
            _todayPlaybackTimes = new Dictionary<string, List<int>>();
            _activeStopwatches = new Dictionary<string, Stopwatch>();
            _currentDate = DateTime.Now.ToString("ddMMyyyy");
            _dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AudioTracker");

            InitializeForToday();
        }

        public static AudioPlaybackTrackingService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new AudioPlaybackTrackingService();
                    }
                }
                return _instance;
            }
        }

        /// Inicializa las listas para el día actual
        private void InitializeForToday()
        {
            var today = DateTime.Now.ToString("ddMMyyyy");

            // Si cambió la fecha, solo limpiar los datos locales sin registrar en base de datos
            if (_currentDate != today)
            {
                Debug.WriteLine($"Cambio de fecha detectado: {_currentDate} -> {today}");
                _currentDate = today;
                _todayPlaybackTimes.Clear();
                _activeStopwatches.Clear(); // Limpiar stopwatches activos sin registrar
            }

            // Inicializar listas para los tipos de audio
            var audioTypes = new[] { "japon", "ethno", "piano" };
            foreach (var audioType in audioTypes)
            {
                var key = $"{_currentDate}{audioType}";
                if (!_todayPlaybackTimes.ContainsKey(key))
                {
                    _todayPlaybackTimes[key] = new List<int>();
                }
            }
        }

        /// Inicia el seguimiento de tiempo para un audio específico
        public void StartTracking(string audioFileName)
        {
            if (string.IsNullOrEmpty(audioFileName)) return;

            var audioType = GetAudioTypeFromFileName(audioFileName);
            if (string.IsNullOrEmpty(audioType)) return;

            lock (_lock)
            {
                InitializeForToday();

                var key = $"{_currentDate}{audioType}";

                // Detener cualquier seguimiento activo para este tipo sin registrar
                if (_activeStopwatches.ContainsKey(key))
                {
                    _activeStopwatches[key].Stop();
                    _activeStopwatches.Remove(key);
                }

                // Iniciar nuevo seguimiento
                _activeStopwatches[key] = Stopwatch.StartNew();
                Debug.WriteLine($"Iniciado seguimiento para: {key}");
            }
        }

        /// Detiene el seguimiento y registra el tiempo transcurrido - ÚNICO PUNTO DE REGISTRO
        public void StopTracking(string audioFileName)
        {
            if (string.IsNullOrEmpty(audioFileName)) return;

            var audioType = GetAudioTypeFromFileName(audioFileName);
            if (string.IsNullOrEmpty(audioType)) return;

            lock (_lock)
            {
                var key = $"{_currentDate}{audioType}";

                if (_activeStopwatches.TryGetValue(key, out var stopwatch))
                {
                    stopwatch.Stop();
                    var elapsedSeconds = (int)stopwatch.Elapsed.TotalSeconds;

                    // Registrar el tiempo solo si es mayor a 1 segundo
                    if (elapsedSeconds > 0)
                    {
                        if (!_todayPlaybackTimes.ContainsKey(key))
                        {
                            _todayPlaybackTimes[key] = new List<int>();
                        }

                        _todayPlaybackTimes[key].Add(elapsedSeconds);
                        Debug.WriteLine($"Registrado: {elapsedSeconds} segundos para {key}");

                        // ÚNICO PUNTO DE REGISTRO: Solo aquí se registra en la base de datos
                        try
                        {
                            AudioStatisticsProcessor.Instance.RegisterAudioPlayback(audioType, elapsedSeconds);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error registrando en base de datos: {ex.Message}");
                        }
                    }

                    _activeStopwatches.Remove(key);
                }
            }
        }

        /// Obtiene el tipo de audio basado en el nombre del archivo
        public string GetAudioTypeFromFileName(string fileName)
        {
            fileName = fileName.ToLowerInvariant();

            if (fileName.Contains("japon") || fileName.Contains("japan"))
                return "japon";
            if (fileName.Contains("ethno") || fileName.Contains("ethnic"))
                return "ethno";
            if (fileName.Contains("piano") || fileName.Contains("relaxing"))
                return "piano";

            return string.Empty;
        }

        /// Guarda los totales diarios de forma asíncrona en el appdata (solo backup local)
        private async Task SaveDailyTotalsAsync(string date, Dictionary<string, int> dailyTotals)
        {
            try
            {
                if (!Directory.Exists(_dataDirectory))
                {
                    Directory.CreateDirectory(_dataDirectory);
                }

                var filePath = Path.Combine(_dataDirectory, $"audio_usage_{date}.json");

                var data = new
                {
                    Date = date,
                    Totals = dailyTotals,
                    SavedAt = DateTime.Now
                };

                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await File.WriteAllTextAsync(filePath, json);
                Debug.WriteLine($"Backup local guardado para fecha {date}: {filePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error guardando backup local: {ex.Message}");
            }
        }

        /// Obtiene estadísticas del día actual (solo datos locales)
        public Dictionary<string, int> GetTodayStatistics()
        {
            lock (_lock)
            {
                InitializeForToday();

                var stats = new Dictionary<string, int>();

                foreach (var kvp in _todayPlaybackTimes)
                {
                    var audioType = kvp.Key.Substring(8);
                    var total = 0;

                    foreach (var seconds in kvp.Value)
                    {
                        total += seconds;
                    }

                    stats[audioType] = total;
                }

                return stats;
            }
        }

        /// Método para obtener estadísticas desde la base de datos
        public Dictionary<string, int> GetTodayStatisticsFromDatabase()
        {
            try
            {
                return AudioStatisticsProcessor.Instance.GetTodayStatisticsFromDatabase();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error obteniendo estadísticas de la base de datos: {ex.Message}");
                return new Dictionary<string, int>
                {
                    ["ethno"] = 0,
                    ["japon"] = 0,
                    ["piano"] = 0
                };
            }
        }

        /// Método para crear backup local sin registrar en base de datos
        public void CreateLocalBackup()
        {
            lock (_lock)
            {
                var currentStats = GetTodayStatistics();
                if (currentStats.Count > 0 && currentStats.Values.Any(v => v > 0))
                {
                    _ = SaveDailyTotalsAsync(_currentDate, currentStats);
                    Debug.WriteLine("Backup local creado");
                }
            }
        }

        /// Limpia datos locales sin forzar registro en base de datos
        public void ClearLocalData()
        {
            lock (_lock)
            {
                // Detener todos los stopwatches activos sin registrar
                foreach (var kvp in _activeStopwatches)
                {
                    kvp.Value.Stop();
                    Debug.WriteLine($"Stopwatch detenido sin registrar: {kvp.Key}");
                }

                _activeStopwatches.Clear();
                _todayPlaybackTimes.Clear();

                Debug.WriteLine("Datos locales limpiados sin registro en base de datos");
            }
        }
    }
}