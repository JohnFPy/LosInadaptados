using System;
using System.Data.SQLite;
using System.IO;

namespace Project.infrastucture
{
    public class connectionSqlite
    {
        private readonly string _connectionString;
        private static string _databasePath = "resources\\emotions.sqlite";

        public connectionSqlite()
        {
            // Si DatabaseInitializer.DatabasePath está disponible, úsalo
            if (!string.IsNullOrEmpty(DatabaseInitializer.DatabasePath) && File.Exists(DatabaseInitializer.DatabasePath))
            {
                _databasePath = DatabaseInitializer.DatabasePath;
            }
            
            _connectionString = $"Data Source={_databasePath};Version=3;";
        }
        
        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }
        
        // Método para actualizar la ruta de la base de datos
        public static void SetDatabasePath(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                _databasePath = path;
            }
        }
        
        // Método para obtener la ruta actual
        public static string GetDatabasePath()
        {
            return _databasePath;
        }
    }
}