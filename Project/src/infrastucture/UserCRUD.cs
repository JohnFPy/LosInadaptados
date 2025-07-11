using Project.domain.models;
using Project.infrastucture.utils;
using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading.Tasks;

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

                    // Si el login es exitoso, establecer la sesión del usuario
                    if (loginSuccess)
                    {
                        var currentUser = GetUserByUsername(username);
                        if (currentUser != null)
                        {
                            UserSession.SetCurrentUser(currentUser);
                            Debug.WriteLine($"User session established for: {currentUser.Name}");
                        }
                    }

                    return loginSuccess;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in Login: {ex.Message}");
                return false;
            }
        }

        public user? GetUserByUsername(string username)
        {
            try
            {
                using (var connection = _connectionSqlite.GetConnection())
                {
                    connection.Open();
                    Debug.WriteLine($"Getting user data for username: {username}");

                    string query = "SELECT Id, Username, Password, Name, LastName, Age, PathImage FROM User WHERE Username = @Username";
                    using var command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@Username", username);

                    using var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        return new user
                        {
                            Id = Convert.ToInt64(reader["Id"]),
                            Username = reader["Username"].ToString()!,
                            Password = reader["Password"].ToString()!,
                            Name = reader["Name"].ToString()!,
                            LastName = reader["LastName"].ToString()!,
                            Age = Convert.ToInt32(reader["Age"]),
                            PathImage = reader["PathImage"].ToString() ?? ""
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting user by username: {ex.Message}");
            }
            return null;
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
        public bool updateUser(user updatedUser)
        {
            try
            {
                using (var connection = _connectionSqlite.GetConnection())
                {
                    connection.Open();
                    Debug.WriteLine($"Database connection opened for updateUser: {connection.DataSource}");
                    string query = "UPDATE User SET Username = @Username, Password = @Password, Name = @Name, LastName = @LastName, WHERE Id = @Id";
                    using var command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@Username", updatedUser.Username);
                    command.Parameters.AddWithValue("@Password", updatedUser.Password);
                    command.Parameters.AddWithValue("@Name", updatedUser.Name);
                    command.Parameters.AddWithValue("@LastName", updatedUser.LastName);
                    command.Parameters.AddWithValue("@Id", updatedUser.Id);
                    int rowsAffected = command.ExecuteNonQuery();
                    Debug.WriteLine($"User updated successfully. Rows affected: {rowsAffected}");
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating user: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> UpdateUsername(string oldUsername, string newUsername)
        {
            try
            {
                using (var connection = _connectionSqlite.GetConnection())
                {
                    connection.Open();
                    string query = "UPDATE User SET Username = @NewUsername WHERE Username = @OldUsername";
                    using var command = new SQLiteCommand(query, connection);
                    command.Parameters.AddWithValue("@NewUsername", newUsername);
                    command.Parameters.AddWithValue("@OldUsername", oldUsername);

                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        var updatedUser = GetUserByUsername(newUsername);
                        if (updatedUser != null)
                        {
                            UserSession.SetCurrentUser(updatedUser);
                        }
                    }

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating username: {ex.Message}");
                return false;
            }
        }


    }
}