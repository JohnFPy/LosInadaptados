using System;
using System.Data.SQLite;
using System.IO;
using System.Diagnostics;

namespace Project.infrastucture
{
    public class connectionSqlite
    {
        private readonly string _connectionString;

        public connectionSqlite()
        {
            string databasePath;

            // Primero intentar usar la ruta del DatabaseInitializer
            if (!string.IsNullOrEmpty(DatabaseInitializer.DatabasePath) && File.Exists(DatabaseInitializer.DatabasePath))
            {
                databasePath = DatabaseInitializer.DatabasePath;
                Debug.WriteLine($"Using AppData database: {databasePath}");
            }
            else
            {
                // Si no está disponible, usar la ruta por defecto
                databasePath = "resources\\emotions.sqlite";
                Debug.WriteLine($"Using default database: {databasePath}");
            }

            _connectionString = $"Data Source={databasePath};Version=3;";
            Debug.WriteLine($"Connection string: {_connectionString}");
        }

        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        // Método para obtener la ruta actual
        public string GetDatabasePath()
        {
            // Extraer la ruta de la cadena de conexión
            var parts = _connectionString.Split(';');
            foreach (var part in parts)
            {
                if (part.StartsWith("Data Source="))
                {
                    return part.Substring("Data Source=".Length);
                }
            }
            return string.Empty;
        }
    }
}