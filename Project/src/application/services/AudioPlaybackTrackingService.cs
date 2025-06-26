using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

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

        private AudioPlaybackTrackingService() //saca la fecha y el diccionario de playback
        {
            _todayPlaybackTimes = new Dictionary<string, List<int>>();
            _activeStopwatches = new Dictionary<string, Stopwatch>();
            _currentDate = DateTime.Now.ToString("ddMMyy");
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

        /// Inicializa las listas para el día actual y procesa datos de días anteriores si es necesario
        private void InitializeForToday()
        {
            var today = DateTime.Now.ToString("ddMMyy");
            
            // Si cambió la fecha, procesar datos del día anterior
            if (_currentDate != today)
            {
                ProcessPreviousDayData();
                _currentDate = today;
                _todayPlaybackTimes.Clear();
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
                InitializeForToday(); // Verificar si cambió la fecha
                
                var key = $"{_currentDate}{audioType}";
                
                // Detener cualquier seguimiento activo para este tipo
                if (_activeStopwatches.ContainsKey(key))
                {
                    _activeStopwatches[key].Stop();
                }

                // Iniciar nuevo seguimiento
                _activeStopwatches[key] = Stopwatch.StartNew();
                Debug.WriteLine($"Iniciado seguimiento para: {key}");
            }
        }

        /// Detiene el seguimiento y registra el tiempo transcurrido
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
                    }
                    
                    _activeStopwatches.Remove(key);
                }
            }
        }

        /// Obtiene el tipo de audio basado en el nombre del archivo
        private string GetAudioTypeFromFileName(string fileName)
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

        /// Procesa los datos del día anterior enviándolos a la capa de dominio
        private void ProcessPreviousDayData()
        {
            if (_todayPlaybackTimes.Count == 0) return;

            try
            {
                // Calcular totales por tipo de audio
                var dailyTotals = new Dictionary<string, int>();
                
                foreach (var kvp in _todayPlaybackTimes)
                {
                    var audioType = kvp.Key.Substring(6); // Remover la fecha (ddMMyy)
                    var totalSeconds = 0;
                    
                    foreach (var seconds in kvp.Value)
                    {
                        totalSeconds += seconds;
                    }
                    
                    if (totalSeconds > 0)
                    {
                        dailyTotals[audioType] = totalSeconds;
                    }
                }

                // Enviar a la capa de dominio
                if (dailyTotals.Count > 0)
                {
                    _ = SaveDailyTotalsAsync(_currentDate, dailyTotals);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error procesando datos del día anterior: {ex.Message}");
            }
        }

        /// Guarda los totales diarios de forma asíncrona en el appdata
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
                Debug.WriteLine($"Datos guardados para fecha {date}: {filePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error guardando datos diarios: {ex.Message}");
            }
        }

        /// Obtiene estadísticas del día actual
        public Dictionary<string, int> GetTodayStatistics()
        {
            lock (_lock)
            {
                InitializeForToday();
                
                var stats = new Dictionary<string, int>();
                
                foreach (var kvp in _todayPlaybackTimes)
                {
                    var audioType = kvp.Key.Substring(6); // Remover fecha
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

        /// Fuerza el guardado de datos actuales (útil al cerrar la aplicación)
        public void ForceSave()
        {
            lock (_lock)
            {
                // Detener todos los stopwatches activos
                foreach (var kvp in _activeStopwatches)
                {
                    kvp.Value.Stop();
                    var audioType = kvp.Key.Substring(6);
                    var elapsedSeconds = (int)kvp.Value.Elapsed.TotalSeconds;
                    
                    if (elapsedSeconds > 0)
                    {
                        if (!_todayPlaybackTimes.ContainsKey(kvp.Key))
                        {
                            _todayPlaybackTimes[kvp.Key] = new List<int>();
                        }
                        _todayPlaybackTimes[kvp.Key].Add(elapsedSeconds);
                    }
                }
                _activeStopwatches.Clear();
                
                // Procesar datos actuales
                ProcessPreviousDayData();
            }
        }
    }
}