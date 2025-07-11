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
        private readonly EmotionLogCRUD _emotionLogCRUD = new EmotionLogCRUD();

        /// <summary>
        /// Obtiene el recuento por cada emoción estándar del usuario, para estadísticas.
        /// </summary>
        public Dictionary<string, int> GetEmotionFrequencies()
        {
            string userId = UserSession.GetCurrentIdUser();

            var result = new Dictionary<string, int>();
            try
            {
                using var conn = _connectionSqlite.GetConnection();
                conn.Open();

                string query = @"
                    SELECT e.Name, COUNT(*) AS count
                    FROM User_Emotion_Log l
                    JOIN Emotion e ON l.Emotion_id = e.Id
                    WHERE l.Id_user = @userId AND l.Emotion_id IS NOT NULL
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
        public Dictionary<string, int> GetPersonalizedEmotionFrequencies()
        {
            string userId = UserSession.GetCurrentIdUser();

            var result = new Dictionary<string, int>();
            try
            {
                using var conn = _connectionSqlite.GetConnection();
                conn.Open();

                string query = @"
                    SELECT pe.Name, COUNT(*) AS count
                    FROM User_Emotion_Log l
                    JOIN Personalized_Emotion pe ON l.Personalized_emotion_id = pe.Id
                    WHERE l.Id_user = @userId AND l.Personalized_emotion_id IS NOT NULL
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
        public (string firstDate, string lastDate)? GetDateRange()
        {
            string userId = UserSession.GetCurrentIdUser();

            try
            {
                using var conn = _connectionSqlite.GetConnection();
                conn.Open();

                string query = @"
                    SELECT MIN(Date) AS firstDate, MAX(Date) AS lastDate
                    FROM User_Emotion_Log
                    WHERE Id_user = @userId";

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
        /// Obtiene el rango de fechas de la semana actual.
        /// </summary>
        private (string StartOfWeek, string EndOfWeek) GetCurrentWeekRange()
        {
            DateTime today = DateTime.Today;
            int delta = DayOfWeek.Monday - today.DayOfWeek;
            DateTime monday = today.AddDays(delta);
            DateTime sunday = monday.AddDays(6);

            return (monday.ToString("yyyy-MM-dd"), sunday.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// Obtiene el recuento de emociones registradas en los últimos 7 días.
        /// </summary>
        public Dictionary<string, int> GetWeeklyEmotionFrequencies()
        {
            string userId = UserSession.GetCurrentIdUser();

            var result = new Dictionary<string, int>();
            try
            {
                using var conn = _connectionSqlite.GetConnection();
                conn.Open();

                var (startOfWeek, endOfWeek) = GetCurrentWeekRange();

                string stdQuery = @"
                    SELECT e.Name, COUNT(*) AS count
                    FROM User_Emotion_Log l
                    JOIN Emotion e ON l.Emotion_id = e.Id
                    WHERE l.Id_user = @userId 
                      AND l.Emotion_id IS NOT NULL
                      AND l.Date BETWEEN @startDate AND @endDate
                    GROUP BY e.Name";

                string persQuery = @"
                    SELECT pe.Name, COUNT(*) AS count
                    FROM User_Emotion_Log l
                    JOIN Personalized_Emotion pe ON l.Personalized_emotion_id = pe.Id
                    WHERE l.Id_user = @userId 
                      AND l.Personalized_emotion_id IS NOT NULL
                      AND l.Date BETWEEN @startDate AND @endDate
                    GROUP BY pe.Name";

                using var stdCmd = new SQLiteCommand(stdQuery, conn);
                stdCmd.Parameters.AddWithValue("@userId", userId);
                stdCmd.Parameters.AddWithValue("@startDate", startOfWeek);
                stdCmd.Parameters.AddWithValue("@endDate", endOfWeek);

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
                persCmd.Parameters.AddWithValue("@startDate", startOfWeek);
                persCmd.Parameters.AddWithValue("@endDate", endOfWeek);

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
        public (string emotionName, int count)? GetMostFrequentEmotion()
        {
            var stdFreq = GetEmotionFrequencies();
            var persFreq = GetPersonalizedEmotionFrequencies();

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

        /// <summary>
        /// Devuelve un resumen formateado con las emociones registradas en la última semana.
        /// </summary>
        public string GetWeeklyEmotionSummary()
        {
            var frequencies = GetWeeklyEmotionFrequencies();

            if (frequencies.Count == 0)
                return "No has registrado emociones en la última semana.";

            var sorted = frequencies.OrderByDescending(kv => kv.Value);

            var summary = new StringBuilder();
            summary.AppendLine("Emociones registradas en la semana actual:");

            foreach (var (emotion, count) in sorted)
            {
                summary.AppendLine($"- {emotion}: {count} ve{(count == 1 ? "z" : "ces")}");
            }

            return summary.ToString().TrimEnd();
        }

        /// <summary>
        /// Devuelve un resumen formateado de la emoción más frecuente.
        /// </summary>
        public string GetMostRegisteredEmotionSummary()
        {
            var result = GetMostFrequentEmotion();

            if (!result.HasValue)
                return "No hay emociones registradas todavía.";

            var (emotionName, count) = result.Value;
            var veces = count == 1 ? "vez" : "veces";

            return $"Te has sentido {emotionName} con mayor frecuencia, la emoción fue registrada {count} {veces}.";
        }

        /// <summary>
        /// Devuelve un resumen formateado de la emoción del día.
        /// </summary>
        public string GetTodayEmotionSummary()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            var log = _emotionLogCRUD.GetEmotionByDate(today);

            if (log == null)
                return "Hoy no has registrado ninguna emoción.";

            string? name = null;

            if (log.Value.idEmotion.HasValue)
                name = _emotionLogCRUD.GetEmotionNameById(log.Value.idEmotion.Value);
            else if (log.Value.idPersonalized.HasValue)
                name = _emotionLogCRUD.GetPersonalizedEmotionNameById(log.Value.idPersonalized.Value);

            if (string.IsNullOrEmpty(name))
                return "Hubo un problema al recuperar la emoción de hoy.";

            var hour = DateTime.Now.ToString("HH:mm");

            return $"Fecha: {DateTime.Now:dd/MM/yyyy}\nEmoción registrada: {name}\nHora: {hour}";
        }

        /// <summary>
        /// Devuelve un resumen formateado de todas las emociones.
        /// </summary>
        public string GetTotalEmotionSummary()
        {
            var stdFreq = GetEmotionFrequencies();
            var persFreq = GetPersonalizedEmotionFrequencies();

            if (stdFreq.Count == 0 && persFreq.Count == 0)
                return "No hay emociones registradas todavía.";

            var sb = new StringBuilder();

            var range = GetDateRange();
            if (range != null)
            {
                sb.AppendLine($"Período: {DateTime.Parse(range.Value.firstDate):MMMM yyyy} - {DateTime.Parse(range.Value.lastDate):MMMM yyyy}");
            }

            sb.AppendLine("Registros por emoción:");

            foreach (var (name, count) in stdFreq)
                sb.AppendLine($"• {name}: {count} registro{(count == 1 ? "" : "s")}");

            foreach (var (name, count) in persFreq)
                sb.AppendLine($"• {name} (personalizada): {count} registro{(count == 1 ? "" : "s")}");

            var most = GetMostFrequentEmotion();
            if (most != null)
            {
                sb.AppendLine($"\nEmoción más registrada: {most.Value.emotionName} ({most.Value.count})");
            }

            return sb.ToString().TrimEnd();
        }

    }

}
