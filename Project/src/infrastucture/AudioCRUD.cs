using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Project.infrastucture
{
    public class AudioCRUD
    {
        private readonly connectionSqlite _connectionSqlite = new connectionSqlite();

        public bool UpdateAudioTime(string dateId, string audioType, int secondsToAdd)
        {
            try
            {
                using var connection = _connectionSqlite.GetConnection();
                connection.Open();

                string normalizedAudioType = NormalizeAudioType(audioType);

                // Usar INSERT OR REPLACE para manejar la clave primaria compuesta
                string upsertQuery = @"
                    INSERT INTO audioReproductionTimes (dateId, audioType, time) 
                    VALUES (@dateId, @audioType, @secondsToAdd)
                    ON CONFLICT(dateId, audioType) 
                    DO UPDATE SET time = time + @secondsToAdd";

                using var command = new SQLiteCommand(upsertQuery, connection);
                command.Parameters.AddWithValue("@dateId", dateId);
                command.Parameters.AddWithValue("@audioType", normalizedAudioType);
                command.Parameters.AddWithValue("@secondsToAdd", secondsToAdd);

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

                string query = @"
                    SELECT audioType, time 
                    FROM audioReproductionTimes 
                    WHERE dateId = @dateId";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@dateId", dateId);

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
        /// Obtiene todos los tipos de audio únicos registrados
        /// </summary>
        public List<string> GetAllAudioTypes()
        {
            var audioTypes = new List<string>();

            try
            {
                using var connection = _connectionSqlite.GetConnection();
                connection.Open();

                string query = "SELECT DISTINCT audioType FROM audioReproductionTimes ORDER BY audioType";

                using var command = new SQLiteCommand(query, connection);
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
        /// Obtiene el tiempo total de un tipo de audio específico
        /// </summary>
        public int GetTotalTimeByAudioType(string audioType)
        {
            try
            {
                using var connection = _connectionSqlite.GetConnection();
                connection.Open();

                string normalizedType = NormalizeAudioType(audioType);
                string query = "SELECT SUM(time) as totalTime FROM audioReproductionTimes WHERE audioType = @audioType";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@audioType", normalizedType);

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