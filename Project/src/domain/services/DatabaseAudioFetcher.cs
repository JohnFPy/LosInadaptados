using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.infrastucture;

namespace Project.domain.services
{
    public class DatabaseAudioFetcher
    {
        private readonly connectionSqlite _connectionSqlite = new connectionSqlite();

        /// <summary>
        /// Obtiene los tiempos de reproducción de audio para la fecha actual del usuario actual
        /// </summary>
        public (int ethnoSeconds, int japanSeconds, int pianoSeconds)? GetTodayAudioTimes()
        {
            try
            {
                // Generar el dateId para hoy (formato: ddMMyyyy)
                string todayDateId = DateTime.Now.ToString("ddMMyyyy");

                using var connection = _connectionSqlite.GetConnection();
                connection.Open();

                // CAMBIO: Agregar filtro por CurrentUsername
                string query = @"
                    SELECT audioType, time 
                    FROM audioReproductionTimes 
                    WHERE dateId = @dateId AND CurrentUsername = @currentUsername";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@dateId", todayDateId);
                command.Parameters.AddWithValue("@currentUsername", UserSession.GetCurrentUsername());

                int ethnoTime = 0, japanTime = 0, pianoTime = 0;

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var audioType = reader["audioType"].ToString()?.ToLower();
                    var time = Convert.ToInt32(reader["time"]);

                    switch (audioType)
                    {
                        case "ethno":
                            ethnoTime = time;
                            break;
                        case "japan":
                            japanTime = time;
                            break;
                        case "piano":
                            pianoTime = time;
                            break;
                    }
                }

                return (ethnoTime, japanTime, pianoTime);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo tiempos de audio para hoy: {ex.Message}");
            }

            // Si no hay datos para hoy, retornar ceros
            return (0, 0, 0);
        }

        /// <summary>
        /// Obtiene los tiempos de reproducción de audio para una fecha específica del usuario actual
        /// </summary>
        public (int ethnoSeconds, int japanSeconds, int pianoSeconds)? GetAudioTimesByDate(string dateId)
        {
            try
            {
                using var connection = _connectionSqlite.GetConnection();
                connection.Open();

                // CAMBIO: Agregar filtro por CurrentUsername
                string query = @"
                    SELECT audioType, time 
                    FROM audioReproductionTimes 
                    WHERE dateId = @dateId AND CurrentUsername = @currentUsername";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@dateId", dateId);
                command.Parameters.AddWithValue("@currentUsername", UserSession.GetCurrentUsername());

                int ethnoTime = 0, japanTime = 0, pianoTime = 0;

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var audioType = reader["audioType"].ToString()?.ToLower();
                    var time = Convert.ToInt32(reader["time"]);

                    switch (audioType)
                    {
                        case "ethno":
                            ethnoTime = time;
                            break;
                        case "japan":
                            japanTime = time;
                            break;
                        case "piano":
                            pianoTime = time;
                            break;
                    }
                }

                return (ethnoTime, japanTime, pianoTime);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo tiempos de audio para fecha {dateId}: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Obtiene las estadísticas totales de todos los tiempos registrados del usuario actual
        /// </summary>
        public (int ethnoSeconds, int japanSeconds, int pianoSeconds)? GetTotalStatistics()
        {
            try
            {
                using var connection = _connectionSqlite.GetConnection();
                connection.Open();

                // CAMBIO: Agregar filtro por CurrentUsername
                string query = @"
                    SELECT audioType, SUM(time) as totalTime 
                    FROM audioReproductionTimes 
                    WHERE CurrentUsername = @currentUsername
                    GROUP BY audioType";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@currentUsername", UserSession.GetCurrentUsername());
                using var reader = command.ExecuteReader();

                int totalEthno = 0, totalJapan = 0, totalPiano = 0;

                while (reader.Read())
                {
                    var audioType = reader["audioType"].ToString()?.ToLower();
                    var totalTime = reader["totalTime"] != DBNull.Value ? Convert.ToInt32(reader["totalTime"]) : 0;

                    switch (audioType)
                    {
                        case "ethno":
                            totalEthno = totalTime;
                            break;
                        case "japan":
                            totalJapan = totalTime;
                            break;
                        case "piano":
                            totalPiano = totalTime;
                            break;
                    }
                }

                Console.WriteLine($"Estadísticas totales: Ethno={totalEthno}, Japan={totalJapan}, Piano={totalPiano}");

                return (totalEthno, totalJapan, totalPiano);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo estadísticas totales: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Obtiene el rango de fechas de todos los registros en la tabla del usuario actual
        /// </summary>
        public (string firstDate, string lastDate)? GetTotalDateRange()
        {
            try
            {
                using var connection = _connectionSqlite.GetConnection();
                connection.Open();

                // CAMBIO: Agregar filtro por CurrentUsername
                string query = @"
                    SELECT MIN(dateId) as firstDate, MAX(dateId) as lastDate 
                    FROM audioReproductionTimes 
                    WHERE time > 0 AND CurrentUsername = @currentUsername";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@currentUsername", UserSession.GetCurrentUsername());
                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    var firstDateId = reader["firstDate"]?.ToString();
                    var lastDateId = reader["lastDate"]?.ToString();

                    if (!string.IsNullOrEmpty(firstDateId) && !string.IsNullOrEmpty(lastDateId))
                    {
                        try
                        {
                            var firstDateTime = DateTime.ParseExact(firstDateId, "ddMMyyyy", null);
                            var lastDateTime = DateTime.ParseExact(lastDateId, "ddMMyyyy", null);

                            var firstDateFormatted = firstDateTime.ToString("dd/MM/yyyy");
                            var lastDateFormatted = lastDateTime.ToString("dd/MM/yyyy");

                            return (firstDateFormatted, lastDateFormatted);
                        }
                        catch
                        {
                            // Si hay error de formato, devolver las fechas sin formatear
                            return (firstDateId, lastDateId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo rango de fechas totales: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Obtiene el audio más reproducido con su tiempo total del usuario actual
        /// </summary>
        public (string audioType, int totalSeconds)? GetMostPlayedAudio()
        {
            var totalStats = GetTotalStatistics();

            if (!totalStats.HasValue)
            {
                return null;
            }

            var (ethnoSeconds, japanSeconds, pianoSeconds) = totalStats.Value;

            // Crear diccionario para facilitar la comparación
            var audioStats = new Dictionary<string, int>
            {
                { "Étnico", ethnoSeconds },
                { "Japonés", japanSeconds },
                { "Piano", pianoSeconds }
            };

            // Encontrar el audio con más tiempo
            var mostPlayed = audioStats.OrderByDescending(kvp => kvp.Value).First();

            // Verificar que tenga al menos algún tiempo registrado
            if (mostPlayed.Value <= 0)
            {
                return null;
            }

            return (mostPlayed.Key, mostPlayed.Value);
        }

        /// <summary>
        /// Busca el primer día con registros hacia atrás desde una fecha dada
        /// </summary>
        private string? FindFirstValidDateBackwards(DateTime startDate, int maxDaysBack)
        {
            for (int i = 0; i <= maxDaysBack; i++)
            {
                var checkDate = startDate.AddDays(-i);
                var dateId = checkDate.ToString("ddMMyyyy");
                var times = GetAudioTimesByDate(dateId);

                // Corregido: verificar que existe el registro (no null) y que tiene al menos algún valor > 0
                if (times.HasValue && (times.Value.ethnoSeconds > 0 || times.Value.japanSeconds > 0 || times.Value.pianoSeconds > 0))
                {
                    Console.WriteLine($"Encontrado registro válido hacia atrás: {dateId} - Ethno: {times.Value.ethnoSeconds}, Japan: {times.Value.japanSeconds}, Piano: {times.Value.pianoSeconds}");
                    return dateId;
                }
            }
            return null;
        }

        /// <summary>
        /// Busca el último día con registros hacia adelante desde una fecha dada
        /// </summary>
        private string? FindLastValidDateForwards(DateTime startDate, int maxDaysForward)
        {
            for (int i = 0; i <= maxDaysForward; i++)
            {
                var checkDate = startDate.AddDays(i);
                var dateId = checkDate.ToString("ddMMyyyy");
                var times = GetAudioTimesByDate(dateId);

                // Corregido: verificar que existe el registro (no null) y que tiene al menos algún valor > 0
                if (times.HasValue && (times.Value.ethnoSeconds > 0 || times.Value.japanSeconds > 0 || times.Value.pianoSeconds > 0))
                {
                    Console.WriteLine($"Encontrado registro válido hacia adelante: {dateId} - Ethno: {times.Value.ethnoSeconds}, Japan: {times.Value.japanSeconds}, Piano: {times.Value.pianoSeconds}");
                    return dateId;
                }
            }
            return null;
        }

        /// <summary>
        /// Obtiene todas las fechas con registros en un rango dado del usuario actual
        /// </summary>
        private List<string> GetAllValidDatesInRange(DateTime startDate, DateTime endDate)
        {
            var validDates = new List<string>();

            try
            {
                using var connection = _connectionSqlite.GetConnection();
                connection.Open();

                // CAMBIO: Agregar filtro por CurrentUsername
                string query = @"
                    SELECT DISTINCT dateId 
                    FROM audioReproductionTimes 
                    WHERE time > 0 AND CurrentUsername = @currentUsername
                    ORDER BY dateId";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@currentUsername", UserSession.GetCurrentUsername());
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var dateIdStr = reader["dateId"].ToString();
                    if (!string.IsNullOrEmpty(dateIdStr))
                    {
                        try
                        {
                            var date = DateTime.ParseExact(dateIdStr, "ddMMyyyy", null);
                            if (date >= startDate && date <= endDate)
                            {
                                validDates.Add(dateIdStr);
                            }
                        }
                        catch
                        {
                            // Ignorar fechas con formato inválido
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo fechas válidas en rango: {ex.Message}");
            }

            return validDates;
        }

        /// <summary>
        /// Obtiene las estadísticas semanales ajustando el rango para encontrar datos válidos
        /// </summary>
        public (int ethnoSeconds, int japanSeconds, int pianoSeconds, string dateRange)? GetWeeklyStatistics()
        {
            try
            {
                var today = DateTime.Now;
                var weekAgo = today.AddDays(-7);

                Console.WriteLine($"Buscando estadísticas semanales desde {weekAgo:ddMMyyyy} hasta {today:ddMMyyyy}");

                // Obtener todas las fechas válidas en el rango de la semana
                var validDates = GetAllValidDatesInRange(weekAgo, today);

                if (validDates.Count == 0)
                {
                    Console.WriteLine("No se encontraron registros válidos en la última semana");
                    return null;
                }

                Console.WriteLine($"Fechas válidas encontradas: {string.Join(", ", validDates)}");

                // Sumar todos los datos de las fechas válidas
                int totalEthno = 0, totalJapan = 0, totalPiano = 0;

                foreach (var dateId in validDates)
                {
                    var times = GetAudioTimesByDate(dateId);
                    if (times.HasValue)
                    {
                        totalEthno += times.Value.ethnoSeconds;
                        totalJapan += times.Value.japanSeconds;
                        totalPiano += times.Value.pianoSeconds;

                        Console.WriteLine($"Fecha {dateId}: Ethno={times.Value.ethnoSeconds}, Japan={times.Value.japanSeconds}, Piano={times.Value.pianoSeconds}");
                    }
                }

                // Crear el string del rango de fechas
                var startDateTime = DateTime.ParseExact(validDates.First(), "ddMMyyyy", null);
                var endDateTime = DateTime.ParseExact(validDates.Last(), "ddMMyyyy", null);
                var dateRange = $"{startDateTime:dd/MM/yyyy} - {endDateTime:dd/MM/yyyy}";

                Console.WriteLine($"Totales semanales: Ethno={totalEthno}, Japan={totalJapan}, Piano={totalPiano}");

                return (totalEthno, totalJapan, totalPiano, dateRange);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo estadísticas semanales: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Convierte segundos a formato "Xm Ys" (minutos y segundos)
        /// </summary>
        public static string FormatSecondsToMinutesAndSeconds(int totalSeconds)
        {
            if (totalSeconds <= 0)
                return "0m 0s";

            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            return $"{minutes}m {seconds}s";
        }

        /// <summary>
        /// Obtiene un resumen formateado de las estadísticas de hoy
        /// </summary>
        public string GetTodayStatisticsSummary()
        {
            var todayTimes = GetTodayAudioTimes();

            if (!todayTimes.HasValue)
            {
                return "No hay datos de reproducción para hoy";
            }

            var (ethnoSeconds, japanSeconds, pianoSeconds) = todayTimes.Value;

            // Calcular total
            int totalSeconds = ethnoSeconds + japanSeconds + pianoSeconds;

            // Obtener fecha de hoy formateada
            var todayFormatted = DateTime.Now.ToString("dd/MM/yyyy");

            if (totalSeconds == 0)
            {
                return $"Fecha: {todayFormatted}\nNo hay reproducción registrada para hoy";
            }

            var summary = new StringBuilder();
            summary.AppendLine($"Fecha: {todayFormatted}");
            summary.AppendLine($"Étnico: {FormatSecondsToMinutesAndSeconds(ethnoSeconds)}");
            summary.AppendLine($"Japonés: {FormatSecondsToMinutesAndSeconds(japanSeconds)}");
            summary.AppendLine($"Piano: {FormatSecondsToMinutesAndSeconds(pianoSeconds)}");
            summary.AppendLine($"Total: {FormatSecondsToMinutesAndSeconds(totalSeconds)}");

            return summary.ToString().TrimEnd();
        }

        /// <summary>
        /// Obtiene un resumen formateado de las estadísticas semanales
        /// </summary>
        public string GetWeeklyStatisticsSummary()
        {
            var weeklyStats = GetWeeklyStatistics();

            if (!weeklyStats.HasValue)
            {
                return "No has reproducido ningún audio esta semana";
            }

            var (ethnoSeconds, japanSeconds, pianoSeconds, dateRange) = weeklyStats.Value;

            // Calcular total
            int totalSeconds = ethnoSeconds + japanSeconds + pianoSeconds;

            if (totalSeconds == 0)
            {
                return "No has reproducido ningún audio esta semana";
            }

            var summary = new StringBuilder();
            summary.AppendLine($"Período: {dateRange}");
            summary.AppendLine($"Étnico: {FormatSecondsToMinutesAndSeconds(ethnoSeconds)}");
            summary.AppendLine($"Japonés: {FormatSecondsToMinutesAndSeconds(japanSeconds)}");
            summary.AppendLine($"Piano: {FormatSecondsToMinutesAndSeconds(pianoSeconds)}");
            summary.AppendLine($"Total: {FormatSecondsToMinutesAndSeconds(totalSeconds)}");

            return summary.ToString().TrimEnd();
        }

        /// <summary>
        /// Obtiene un resumen formateado del audio más reproducido
        /// </summary>
        public string GetMostPlayedAudioSummary()
        {
            var mostPlayed = GetMostPlayedAudio();

            if (!mostPlayed.HasValue)
            {
                return "No hay suficientes datos de reproducción";
            }

            var (audioType, totalSeconds) = mostPlayed.Value;

            var formattedTime = FormatSecondsToMinutesAndSeconds(totalSeconds);

            return $"El audio que más has reproducido ha sido {audioType} con {formattedTime} de reproducción total";
        }

        /// <summary>
        /// Obtiene un resumen formateado de las estadísticas totales
        /// </summary>
        public string GetTotalStatisticsSummary()
        {
            var totalStats = GetTotalStatistics();

            if (!totalStats.HasValue)
            {
                return "No hay datos de reproducción registrados";
            }

            var (ethnoSeconds, japanSeconds, pianoSeconds) = totalStats.Value;

            // Calcular total
            int totalSeconds = ethnoSeconds + japanSeconds + pianoSeconds;

            if (totalSeconds == 0)
            {
                return "No hay reproducción registrada";
            }

            // Obtener rango de fechas
            var dateRange = GetTotalDateRange();
            string periodText = "Período: Todos los registros";

            if (dateRange.HasValue)
            {
                var (firstDate, lastDate) = dateRange.Value;
                if (firstDate == lastDate)
                {
                    periodText = $"Período: {firstDate}";
                }
                else
                {
                    periodText = $"Período: {firstDate} - {lastDate}";
                }
            }

            var summary = new StringBuilder();
            summary.AppendLine(periodText);
            summary.AppendLine($"Étnico: {FormatSecondsToMinutesAndSeconds(ethnoSeconds)}");
            summary.AppendLine($"Japonés: {FormatSecondsToMinutesAndSeconds(japanSeconds)}");
            summary.AppendLine($"Piano: {FormatSecondsToMinutesAndSeconds(pianoSeconds)}");
            summary.AppendLine($"Total: {FormatSecondsToMinutesAndSeconds(totalSeconds)}");

            return summary.ToString().TrimEnd();
        }
    }
}