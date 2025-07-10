using Project.domain.models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

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
                    string query = "INSERT INTO Calendar (Id_user, Date, Id_emotion, Id_personalized_emotion) VALUES (@Id_user, @Date, @Id_emotion, @Id_personalized_emotion)";
                    using var command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@Id_user", UserId);
                    command.Parameters.AddWithValue("@Date", dateId);
                    command.Parameters.AddWithValue("@Id_emotion", idEmotion.HasValue ? (object)idEmotion.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@Id_personalized_emotion", idPersonalizedEmotion.HasValue ? (object)idPersonalizedEmotion.Value : DBNull.Value);

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
                    string query = "SELECT Id_emotion, Id_personalized_emotion FROM Calendar WHERE Id_user = @Id_user AND Date = @Date";
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
                    string query = "DELETE FROM Calendar WHERE Id_user = @Id_user AND Date = @Date";
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

        public string? GetEmotionNameById(long emotionId)
        {
            try
            {
                using (var connection = _connection.GetConnection())
                {
                    connection.Open();
                    string query = "SELECT Name FROM Emotion WHERE Id = @Id_emotion";
                    using var command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@Id_emotion", emotionId);
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
                    string query = "SELECT Name FROM Personalized_Emotion WHERE Id = @Id_personalized_emotion";
                    using var command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@Id_personalized_emotion", emotionId);
                    return command.ExecuteScalar()?.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting personalized emotion name by ID: {ex.Message}");
                return null;
            }
        }

        public Dictionary<string, int> GetEmotionFrequencies()
        {
            string username = UserSession.GetCurrentUsername();

            return null;
        }

        public Dictionary<string, int> GetPersonalizedEmotionFrequencies()
        {
            string username = UserSession.GetCurrentUsername();

            return null;
        }

        public List<personalizedEmotion> GetAllPersonalizedEmotionsByUser()
        {
            string username = UserSession.GetCurrentUsername();

            return null;
        }

    }
}
