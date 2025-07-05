using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Project.infrastucture;

namespace Project.domain.services
{
    internal class Health_Tip_Fetcher
    {
        private readonly connectionSqlite _connectionSqlite = new connectionSqlite();

        /// <summary>
        /// Modelo para representar un tip de salud
        /// </summary>
        public class HealthTip
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }

        /// <summary>
        /// Obtiene todos los tips de salud desde la tabla Health_Tip
        /// </summary>
        public List<HealthTip> GetAllHealthTips()
        {
            var healthTips = new List<HealthTip>();

            try
            {
                using var connection = _connectionSqlite.GetConnection();
                connection.Open();

                string query = "SELECT Id, Name, Description FROM Health_Tip ORDER BY Id";

                using var command = new SQLiteCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var healthTip = new HealthTip
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"]?.ToString() ?? string.Empty,
                        Description = reader["Description"]?.ToString() ?? string.Empty
                    };

                    healthTips.Add(healthTip);
                }

                Console.WriteLine($"Se obtuvieron {healthTips.Count} tips de salud desde la base de datos");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo tips de salud: {ex.Message}");
            }

            return healthTips;
        }

        /// <summary>
        /// Obtiene un tip de salud específico por ID
        /// </summary>
        public HealthTip? GetHealthTipById(int id)
        {
            try
            {
                using var connection = _connectionSqlite.GetConnection();
                connection.Open();

                string query = "SELECT Id, Name, Description FROM Health_Tip WHERE Id = @id";

                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new HealthTip
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"]?.ToString() ?? string.Empty,
                        Description = reader["Description"]?.ToString() ?? string.Empty
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo tip de salud con ID {id}: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Verifica si existen tips de salud en la tabla Health_Tip
        /// </summary>
        public bool HasHealthTips()
        {
            try
            {
                using var connection = _connectionSqlite.GetConnection();
                connection.Open();

                string query = "SELECT COUNT(*) FROM Health_Tip";

                using var command = new SQLiteCommand(query, connection);
                var count = Convert.ToInt32(command.ExecuteScalar());

                return count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verificando existencia de tips de salud: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene el número total de tips de salud
        /// </summary>
        public int GetHealthTipsCount()
        {
            try
            {
                using var connection = _connectionSqlite.GetConnection();
                connection.Open();

                string query = "SELECT COUNT(*) FROM Health_Tip";

                using var command = new SQLiteCommand(query, connection);
                return Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo el conteo de tips de salud: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Obtiene un tip de salud aleatorio
        /// </summary>
        public HealthTip? GetRandomHealthTip()
        {
            try
            {
                using var connection = _connectionSqlite.GetConnection();
                connection.Open();

                string query = "SELECT Id, Name, Description FROM Health_Tip ORDER BY RANDOM() LIMIT 1";

                using var command = new SQLiteCommand(query, connection);
                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new HealthTip
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"]?.ToString() ?? string.Empty,
                        Description = reader["Description"]?.ToString() ?? string.Empty
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo tip de salud aleatorio: {ex.Message}");
            }

            return null;
        }
    }
}