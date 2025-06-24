using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Security.Cryptography;
using Project.domain.models;
using Project.infrastucture.utils;


namespace Project.infrastucture
{
    public class UserCRUD
    {
        private readonly connectionSqlite _connectionSqlite = new();

        public bool SignUp(user user)
        {
            using (var connection = _connectionSqlite.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO users (Username, Password, Name, LastName, Age, PathImage) VALUES (@Username, @Password, @Name, @LastName, @Age, @PathImage)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.Parameters.AddWithValue("@LastName", user.LastName);
                    command.Parameters.AddWithValue("@Age", user.Age);
                    command.Parameters.AddWithValue("@PathImage", user.PathImage);
                    command.ExecuteNonQuery();
                }
            }

            return true;
        }

        public bool Login(string username, string password)
        {
            using (var connection = _connectionSqlite.GetConnection())
            {
                connection.Open();
                string query = "SELECT Password FROM users WHERE Username = @Username";
                using var command = new SQLiteCommand(query, connection);
                
                command.Parameters.AddWithValue("@Username", username);
                var storedPassword = command.ExecuteScalar()?.ToString();

                if (storedPassword == null)
                {
                    return false;
                }


                string hashedPassword = passwordHasher.HashPassword(password);
                return hashedPassword == storedPassword;
            }
        }
    }
}
