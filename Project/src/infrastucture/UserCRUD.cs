using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Project.domain.models;
using Project.infrastucture.utils;

namespace Project.infrastucture
{
    public class UserCRUD
    {
        private readonly connectionSqlite _connectionSqlite = new();

        public bool SignUp(user user)
        {
            try
            {
                using (var connection = _connectionSqlite.GetConnection())
                {
                    connection.Open();
                    Debug.WriteLine($"Database connection opened for SignUp: {connection.DataSource}");

                    string query = "INSERT INTO User (Username, Password, Name, LastName, Age, PathImage) VALUES (@Username, @Password, @Name, @LastName, @Age, @PathImage)";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", user.Username);
                        command.Parameters.AddWithValue("@Password", user.Password);
                        command.Parameters.AddWithValue("@Name", user.Name);
                        command.Parameters.AddWithValue("@LastName", user.LastName);
                        command.Parameters.AddWithValue("@Age", user.Age);
                        command.Parameters.AddWithValue("@PathImage", user.PathImage);

                        int rowsAffected = command.ExecuteNonQuery();
                        Debug.WriteLine($"User registered successfully. Rows affected: {rowsAffected}");
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in SignUp: {ex.Message}");
                return false;
            }
        }

        public bool Login(string username, string password)
        {
            try
            {
                using (var connection = _connectionSqlite.GetConnection())
                {
                    connection.Open();
                    Debug.WriteLine($"Database connection opened for Login: {connection.DataSource}");

                    // Primero verificar si el usuario existe
                    string checkUserQuery = "SELECT COUNT(*) FROM User WHERE Username = @Username";
                    using var checkCommand = new SQLiteCommand(checkUserQuery, connection);
                    checkCommand.Parameters.AddWithValue("@Username", username);

                    long userCount = (long)checkCommand.ExecuteScalar();
                    Debug.WriteLine($"Users found with username '{username}': {userCount}");

                    if (userCount == 0)
                    {
                        Debug.WriteLine("User not found");
                        return false;
                    }

                    // Obtener la contraseña almacenada
                    string query = "SELECT Password FROM User WHERE Username = @Username";
                    using var command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@Username", username);

                    var storedPassword = command.ExecuteScalar()?.ToString();
                    Debug.WriteLine($"Stored password hash: {storedPassword}");

                    if (storedPassword == null)
                    {
                        Debug.WriteLine("No password found for user");
                        return false;
                    }

                    // Hash de la contraseña ingresada
                    string hashedPassword = passwordHasher.HashPassword(password);
                    Debug.WriteLine($"Input password hash: {hashedPassword}");

                    bool loginSuccess = hashedPassword == storedPassword;
                    Debug.WriteLine($"Login result: {loginSuccess}");

                    return loginSuccess;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Login: {ex.Message}");
                return false;
            }
        }

        // Método auxiliar para listar todos los usuarios (para debugging)
        public void ListAllUsers()
        {
            try
            {
                using (var connection = _connectionSqlite.GetConnection())
                {
                    connection.Open();
                    Debug.WriteLine($"Listing users from database: {connection.DataSource}");

                    string query = "SELECT Username, Name, LastName FROM User";
                    using var command = new SQLiteCommand(query, connection);
                    using var reader = command.ExecuteReader();

                    Debug.WriteLine("Existing users:");
                    while (reader.Read())
                    {
                        Debug.WriteLine($"- Username: {reader["Username"]}, Name: {reader["Name"]} {reader["LastName"]}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error listing users: {ex.Message}");
            }
        }
    }
}