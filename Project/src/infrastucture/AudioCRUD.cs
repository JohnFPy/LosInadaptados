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

                string columnName = GetColumnName(audioType);

                string insertQuery = "INSERT OR IGNORE INTO audioReproductionTimes (dateId, ethnoTime, japanTime, pianoTime) VALUES (@dateId, 0, 0, 0)";

                using (var insertCommand = new SQLiteCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@dateId", dateId);
                    insertCommand.ExecuteNonQuery();
                }

                string updateQuery = string.Format("UPDATE audioReproductionTimes SET {0} = {0} + @secondsToAdd WHERE dateId = @dateId", columnName);

                using var updateCommand = new SQLiteCommand(updateQuery, connection);
                updateCommand.Parameters.AddWithValue("@secondsToAdd", secondsToAdd);
                updateCommand.Parameters.AddWithValue("@dateId", dateId);

                return updateCommand.ExecuteNonQuery() > 0;
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

                string query = "SELECT ethnoTime, japanTime, pianoTime FROM audioReproductionTimes WHERE dateId = @dateId";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@dateId", dateId);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return (
                        Convert.ToInt32(reader["ethnoTime"]),
                        Convert.ToInt32(reader["japanTime"]),
                        Convert.ToInt32(reader["pianoTime"])
                    );
                }
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

        private string GetColumnName(string audioType)
        {
            var audioTypeLower = audioType.ToLowerInvariant();
            
            if (audioTypeLower == "japon" || audioTypeLower == "japan")
                return "japanTime";
            if (audioTypeLower == "ethno" || audioTypeLower == "ethnic")
                return "ethnoTime";
            if (audioTypeLower == "piano" || audioTypeLower == "relaxing")
                return "pianoTime";
            
            throw new ArgumentException("Tipo de audio no valido: " + audioType);
        }
    }
}