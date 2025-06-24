using System.Data.SQLite;

namespace Project.infrastucture
{
    public class connectionSqlite
    {
        private readonly string _connectionString;

        public connectionSqlite()
        {
            string databasePath = "resources\\emotions.sqlite";
            _connectionString = $"Data Source={databasePath};Version=3;";
        }
        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }
    }
}