using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using Project.infrastucture;

namespace Project.domain.services
{
    internal class DatabaseEmotionFetcher
    {
        private readonly connectionSqlite _connectionSqlite = new connectionSqlite();

        /// <summary>
        /// Obtiene la emoción (estándar o personalizada) registrada en una fecha para el usuario actual.
        /// </summary>
        public (long? emotionId, long? personalizedEmotionId)? GetEmotionLogForDate(string dateId)
        {
            try
            {
                using var conn = _connectionSqlite.GetConnection();
                conn.Open();

                string query = @"
                    SELECT Emotion_id, Personalized_emotion_id
                    FROM User_Emotion_Log
                    WHERE date = @date AND IdUser = @userId";

                using var cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@date", dateId);
                cmd.Parameters.AddWithValue("@userId", UserSession.GetCurrentUsername());

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var std = reader["Emotion_id"] != DBNull.Value ? (long?)reader.GetInt64(0) : null;
                    var pers = reader["Personalized_emotion_id"] != DBNull.Value ? (long?)reader.GetInt64(1) : null;
                    return (std, pers);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetEmotionLogForDate: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Obtiene el nombre de una emoción estándar dado su Id.
        /// </summary>
        public string? GetEmotionNameById(long id)
        {
            try
            {
                using var conn = _connectionSqlite.GetConnection();
                conn.Open();

                string query = "SELECT Name FROM Emotion WHERE Id = @id";

                using var cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                return cmd.ExecuteScalar()?.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetEmotionNameById: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Obtiene el nombre de una emoción personalizada dado su Id y usuario.
        /// </summary>
        public string? GetPersonalizedEmotionNameById(long id, string userId)
        {
            try
            {
                using var conn = _connectionSqlite.GetConnection();
                conn.Open();

                string query = @"
                    SELECT Name
                    FROM Personalized_Emotion
                    WHERE Id = @id AND Id_user = @userId";

                using var cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@userId", userId);

                return cmd.ExecuteScalar()?.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetPersonalizedEmotionNameById: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Obtiene el recuento por cada emoción estándar del usuario, para estadísticas.
        /// </summary>
        public Dictionary<string, int> GetEmotionFrequencies(string userId)
        {
            var result = new Dictionary<string, int>();
            try
            {
                using var conn = _connectionSqlite.GetConnection();
                conn.Open();

                string query = @"
                    SELECT e.Name, COUNT(*) AS count
                    FROM User_Emotion_Log l
                    JOIN Emotion e ON l.Emotion_id = e.Id
                    WHERE l.IdUser = @userId AND l.Emotion_id IS NOT NULL
                    GROUP BY e.Name";

                using var cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var name = reader["Name"]?.ToString() ?? "";
                    var count = Convert.ToInt32(reader["count"]);
                    result[name] = count;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetEmotionFrequencies: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Obtiene el recuento por cada emoción personalizada del usuario, para estadísticas.
        /// </summary>
        public Dictionary<string, int> GetPersonalizedEmotionFrequencies(string userId)
        {
            var result = new Dictionary<string, int>();
            try
            {
                using var conn = _connectionSqlite.GetConnection();
                conn.Open();

                string query = @"
                    SELECT pe.Name, COUNT(*) AS count
                    FROM User_Emotion_Log l
                    JOIN Personalized_Emotion pe ON l.Personalized_emotion_id = pe.Id
                    WHERE l.IdUser = @userId AND l.Personalized_emotion_id IS NOT NULL
                    GROUP BY pe.Name";

                using var cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var name = reader["Name"]?.ToString() ?? "";
                    var count = Convert.ToInt32(reader["count"]);
                    result[name] = count;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetPersonalizedEmotionFrequencies: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Obtiene el rango de fechas con al menos un registro de emoción para el usuario.
        /// </summary>
        public (string firstDate, string lastDate)? GetDateRange(string userId)
        {
            try
            {
                using var conn = _connectionSqlite.GetConnection();
                conn.Open();

                string query = @"
                    SELECT MIN(date) AS firstDate, MAX(date) AS lastDate
                    FROM User_Emotion_Log
                    WHERE IdUser = @userId";

                using var cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var first = reader["firstDate"]?.ToString();
                    var last = reader["lastDate"]?.ToString();

                    if (!string.IsNullOrEmpty(first) && !string.IsNullOrEmpty(last))
                        return (first, last);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetDateRange: {ex.Message}");
            }
            return null;
        }

        /// <summary>
        /// Obtiene el recuento de emociones registradas en los últimos 7 días.
        /// </summary>
        public Dictionary<string, int> GetWeeklyEmotionFrequencies(string userId)
        {
            var result = new Dictionary<string, int>();
            try
            {
                using var conn = _connectionSqlite.GetConnection();
                conn.Open();

                var oneWeekAgo = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");

                string stdQuery = @"
            SELECT e.Name, COUNT(*) AS count
            FROM User_Emotion_Log l
            JOIN Emotion e ON l.Emotion_id = e.Id
            WHERE l.IdUser = @userId 
              AND l.Emotion_id IS NOT NULL
              AND l.date >= @startDate
            GROUP BY e.Name";

                string persQuery = @"
            SELECT pe.Name, COUNT(*) AS count
            FROM User_Emotion_Log l
            JOIN Personalized_Emotion pe ON l.Personalized_emotion_id = pe.Id
            WHERE l.IdUser = @userId 
              AND l.Personalized_emotion_id IS NOT NULL
              AND l.date >= @startDate
            GROUP BY pe.Name";

                using var stdCmd = new SQLiteCommand(stdQuery, conn);
                stdCmd.Parameters.AddWithValue("@userId", userId);
                stdCmd.Parameters.AddWithValue("@startDate", oneWeekAgo);

                using var stdReader = stdCmd.ExecuteReader();
                while (stdReader.Read())
                {
                    var name = stdReader["Name"].ToString() ?? "";
                    var count = Convert.ToInt32(stdReader["count"]);
                    result[name] = count;
                }
                stdReader.Close();

                using var persCmd = new SQLiteCommand(persQuery, conn);
                persCmd.Parameters.AddWithValue("@userId", userId);
                persCmd.Parameters.AddWithValue("@startDate", oneWeekAgo);

                using var persReader = persCmd.ExecuteReader();
                while (persReader.Read())
                {
                    var name = persReader["Name"].ToString() ?? "";
                    var count = Convert.ToInt32(persReader["count"]);
                    if (result.ContainsKey(name))
                        result[name] += count;
                    else
                        result[name] = count;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetWeeklyEmotionFrequencies: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Devuelve la emoción (estándar o personalizada) más frecuente en total del usuario.
        /// </summary>
        public (string emotionName, int count)? GetMostFrequentEmotion(string userId)
        {
            var stdFreq = GetEmotionFrequencies(userId);
            var persFreq = GetPersonalizedEmotionFrequencies(userId);

            var allFrequencies = new Dictionary<string, int>(stdFreq);

            foreach (var kv in persFreq)
            {
                if (allFrequencies.ContainsKey(kv.Key))
                    allFrequencies[kv.Key] += kv.Value;
                else
                    allFrequencies[kv.Key] = kv.Value;
            }

            if (allFrequencies.Count == 0)
                return null;

            var mostFrequent = allFrequencies.OrderByDescending(kv => kv.Value).First();
            return (mostFrequent.Key, mostFrequent.Value);
        }

    }
}
