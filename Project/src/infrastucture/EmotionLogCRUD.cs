using Project.domain.models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;

namespace Project.infrastucture
{
    public class EmotionLogCRUD
    {
        private readonly connectionSqlite _connection = new();

        public bool RegisterEmotion(string dateId, long? idEmotion, long? idPersonalizedEmotion)
        {
            string UserId = UserSession.GetCurrentIdUser();

            try { 
                using (var connection = _connection.GetConnection())
                {
                    connection.Open();
                    string query = "INSERT INTO User_Emotion_Log (Id_user, Date, Emotion_id, Personalized_emotion_id) VALUES (@Id_user, @Date, @Emotion_id, @Personalized_emotion_id)";
                    using var command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@Id_user", UserId);
                    command.Parameters.AddWithValue("@Date", dateId);
                    command.Parameters.AddWithValue("@Emotion_id", idEmotion.HasValue ? (object)idEmotion.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@Personalized_emotion_id", idPersonalizedEmotion.HasValue ? (object)idPersonalizedEmotion.Value : DBNull.Value);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering emotion: {ex.Message}");
                return false;
            }
        }

        public (long? idEmotion, long? idPersonalized)? GetEmotionByDate(string dateId)
        {
            string UserId = UserSession.GetCurrentIdUser();
            try
            {
                using (var connection = _connection.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT Emotion_id, Personalized_emotion_id FROM User_Emotion_Log WHERE Id_user = @Id_user AND Date = @Date";
                    using var command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@Id_user", UserId);
                    command.Parameters.AddWithValue("@Date", dateId);
                    using var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        long? idEmotion = reader.IsDBNull(0) ? null : reader.GetInt64(0);
                        long? idPersonalizedEmotion = reader.IsDBNull(1) ? null : reader.GetInt64(1);
                        return (idEmotion, idPersonalizedEmotion);
                    }
                    return null;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting emotion by date: {ex.Message}");
                return null;
            }
        }

        public bool DeleteEmotionEntry(string dateId)
        {
            string UserId = UserSession.GetCurrentIdUser();
            try
            {
                using (var connection = _connection.GetConnection())
                {
                    connection.Open();
                    string query = "DELETE FROM User_Emotion_Log WHERE Id_user = @Id_user AND Date = @Date";
                    using var command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@Id_user", UserId);
                    command.Parameters.AddWithValue("@Date", dateId);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting emotion entry: {ex.Message}");
                return false;
            }            
        }

        public long? GetEmotionIdByName(string name)
        {
            try
            {
                using var conn = _connection.GetConnection();
                conn.Open();

                string query = "SELECT Id FROM Emotion WHERE Name = @name";
                using var cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", name);

                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt64(result) : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetEmotionIdByName: {ex.Message}");
                return null;
            }
        }

        public long? GetPersonalizedEmotionIdByName(string name)
        {
            string username = UserSession.GetCurrentUsername();

            try
            {
                using var conn = _connection.GetConnection();
                conn.Open();

                string query = "SELECT Id FROM Personalized_Emotion WHERE Name = @name AND Id_user = @userId";
                using var cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@userId", username);

                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt64(result) : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetPersonalizedEmotionIdByName: {ex.Message}");
                return null;
            }
        }

        public string? GetEmotionNameById(long emotionId)
        {
            try
            {
                using (var connection = _connection.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT Name FROM Emotion WHERE Id = @Emotion_id";
                    using var command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@Emotion_id", emotionId);
                    return command.ExecuteScalar()?.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting emotion name by ID: {ex.Message}");
                return null;
            }
        }

        public string? GetPersonalizedEmotionNameById(long emotionId)
        {
            try
            {
                using (var connection = _connection.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT Name FROM Personalized_Emotion WHERE Id = @Personalized_emotion_id";
                    using var command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@Personalized_emotion_id", emotionId);
                    return command.ExecuteScalar()?.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting personalized emotion name by ID: {ex.Message}");
                return null;
            }
        }

        public string? GetPersonalizedEmotionImagePathByName(string name)
        {
            string userId = UserSession.GetCurrentUsername();

            try
            {
                using var conn = _connection.GetConnection();
                conn.Open();

                string query = "SELECT Path_image FROM Personalized_Emotion WHERE Name = @name AND Id_user = @userId";

                using var cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@userId", userId);

                return cmd.ExecuteScalar()?.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetPersonalizedEmotionImagePathByName: {ex.Message}");
                return null;
            }
        }

        public void SaveNewPersonalizedEmotion(string name, string imagePath)
        {
            string userId = UserSession.GetCurrentUsername();

            try
            {
                using var conn = _connection.GetConnection();
                conn.Open();

                string query = "INSERT INTO Personalized_Emotion (Name, Id_user, Path_image) VALUES (@name, @userId, @imagePath)";

                using var cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@imagePath", imagePath);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en SaveNewPersonalizedEmotion: {ex.Message}");
            }
        }

        public Dictionary<string, string> GetAllPersonalizedEmotionsWithPaths()
        {
            string userId = UserSession.GetCurrentUsername();
            var result = new Dictionary<string, string>();

            try
            {
                using var conn = _connection.GetConnection();
                conn.Open();

                string query = @"
                    SELECT Name, Path_image
                    FROM Personalized_Emotion
                    WHERE Id_user = @userId";

                using var cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var name = reader["Name"]?.ToString() ?? "";
                    var path = reader["Path_image"]?.ToString() ?? null;
                    result[name] = path;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetAllPersonalizedEmotionsWithPaths: {ex.Message}");
            }

            return result;
        }

        public Dictionary<string, string?> GetAllStandardEmotionsWithPaths()
        {
            var result = new Dictionary<string, string?>();

            using var connection = _connection.GetConnection();
            connection.Open();

            string query = "SELECT Name, Image FROM Emotion";
            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string name = reader["Name"].ToString()!;
                string? path = reader["Image"]?.ToString() ?? null;
                result[name] = path;
            }

            return result;
        }

        public bool UpdateEmotion(string dateId, long? idEmotion, long? idPersonalized)
        {
            try
            {
                using var connection = _connection.GetConnection();
                connection.Open();

                string query = @"
                    UPDATE User_Emotion_Log
                    SET Emotion_id = @EmotionId,
                        Personalized_emotion_id = @PersonalizedId
                    WHERE Date = @DateId;
                ";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@DateId", dateId);
                command.Parameters.AddWithValue("@EmotionId", (object?)idEmotion ?? DBNull.Value);
                command.Parameters.AddWithValue("@PersonalizedId", (object?)idPersonalized ?? DBNull.Value);

                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error actualizando emoción: {ex.Message}");
                return false;
            }
        }

    }
}
