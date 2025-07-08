using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Project.infrastucture
{
    public class AudioCRUD
    {
        private readonly connectionSqlite _connectionSqlite = new connectionSqlite();

        public bool UpdateAudioTime(string dateId, string audioType, int secondsToAdd, string currentUsername = null)
        {
            try
            {
                using var connection = _connectionSqlite.GetConnection();
                connection.Open();

                string normalizedAudioType = NormalizeAudioType(audioType);

                // Si no se proporciona username, usar el Username de la sesión actual (para audio logs)
                string username = currentUsername ?? UserSession.GetCurrentUsername();

                // CAMBIO: Actualizar ON CONFLICT para el nuevo PRIMARY KEY
                string upsertQuery = @"
            INSERT INTO audioReproductionTimes (dateId, audioType, time, CurrentUsername) 
            VALUES (@dateId, @audioType, @secondsToAdd, @currentUsername)
            ON CONFLICT(dateId, audioType, CurrentUsername) 
            DO UPDATE SET time = time + @secondsToAdd";

                using var command = new SQLiteCommand(upsertQuery, connection);
                command.Parameters.AddWithValue("@dateId", dateId);
                command.Parameters.AddWithValue("@audioType", normalizedAudioType);
                command.Parameters.AddWithValue("@secondsToAdd", secondsToAdd);
                command.Parameters.AddWithValue("@currentUsername", username);

                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error actualizando tiempo de audio: " + ex.Message);
                return false;
            }
        }

        public (int ethnoTime, int japanTime, int pianoTime)? GetAudioTimesByDate(string dateId)
        {
            try
            {
                using var connection = _connectionSqlite.GetConnection();
                connection.Open();

                // Filtrar por dateId Y CurrentUsername del usuario actual
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
                    var audioType = reader["audioType"].ToString();
                    var time = Convert.ToInt32(reader["time"]);

                    switch (audioType?.ToLower())
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
                Console.WriteLine("Error obteniendo tiempos de audio: " + ex.Message);
            }

            return null;
        }

        public static string FormatDateId(DateTime date)
        {
            return date.ToString("ddMMyyyy");
        }

        public static string GetTodayDateId()
        {
            return FormatDateId(DateTime.Now);
        }

        public bool SaveDailyStatistics(Dictionary<string, int> dailyTotals)
        {
            if (dailyTotals == null || dailyTotals.Count == 0)
                return false;

            var dateId = GetTodayDateId();
            var success = true;

            foreach (var kvp in dailyTotals)
            {
                if (kvp.Value > 0)
                {
                    var result = UpdateAudioTime(dateId, kvp.Key, kvp.Value);
                    success = success && result;
                }
            }

            return success;
        }

        /// <summary>
        /// Normaliza el tipo de audio para usar con el nuevo esquema
        /// </summary>
        private string NormalizeAudioType(string audioType)
        {
            var audioTypeLower = audioType.ToLowerInvariant();

            if (audioTypeLower == "japon" || audioTypeLower == "japan" || audioTypeLower == "japonés")
                return "Japan";
            if (audioTypeLower == "ethno" || audioTypeLower == "ethnic" || audioTypeLower == "étnico")
                return "ethno";
            if (audioTypeLower == "piano" || audioTypeLower == "relaxing")
                return "piano";

            throw new ArgumentException("Tipo de audio no válido: " + audioType);
        }

        /// <summary>
        /// Obtiene todos los tipos de audio únicos registrados para el usuario actual
        /// </summary>
        public List<string> GetAllAudioTypes()
        {
            var audioTypes = new List<string>();

            try
            {
                using var connection = _connectionSqlite.GetConnection();
                connection.Open();

                string query = @"SELECT DISTINCT audioType 
                               FROM audioReproductionTimes 
                               WHERE CurrentUsername = @currentUsername 
                               ORDER BY audioType";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@currentUsername", UserSession.GetCurrentUsername());
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var audioType = reader["audioType"].ToString();
                    if (!string.IsNullOrEmpty(audioType))
                    {
                        audioTypes.Add(audioType);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo tipos de audio: {ex.Message}");
            }

            return audioTypes;
        }

        /// <summary>
        /// Obtiene el tiempo total de un tipo de audio específico para el usuario actual
        /// </summary>
        public int GetTotalTimeByAudioType(string audioType)
        {
            try
            {
                using var connection = _connectionSqlite.GetConnection();
                connection.Open();

                string normalizedType = NormalizeAudioType(audioType);
                string query = @"SELECT SUM(time) as totalTime 
                               FROM audioReproductionTimes 
                               WHERE audioType = @audioType AND CurrentUsername = @currentUsername";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@audioType", normalizedType);
                command.Parameters.AddWithValue("@currentUsername", UserSession.GetCurrentUsername());

                var result = command.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo tiempo total para {audioType}: {ex.Message}");
                return 0;
            }
        }
    }
}