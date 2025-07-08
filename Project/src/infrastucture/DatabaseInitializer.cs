using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Project.infrastucture
{
    public static class DatabaseInitializer
    {
        // Ruta de la base de datos persistente en AppData
        private static readonly string AppDataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Project"
        );

        // Nombre del archivo de base de datos
        private static readonly string DbFileName = "emotions.sqlite";

        // Ruta completa a la base de datos persistente
        private static readonly string PersistentDbPath = Path.Combine(AppDataDir, DbFileName);

        // Para acceso externo a la ruta de la base de datos
        public static string DatabasePath { get; private set; } = string.Empty;

        public static void Initialize()
        {
            try
            {
                // Directorio base de la aplicación
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string resourcesDir = Path.Combine(baseDir, "resources");
                string templateDbFile = Path.Combine(resourcesDir, DbFileName);
                string sourceDbFile = Path.Combine(baseDir, DbFileName);

                Debug.WriteLine($"Inicializando base de datos persistente en: {PersistentDbPath}");

                // Crear directorio de AppData si no existe
                if (!Directory.Exists(AppDataDir))
                {
                    Debug.WriteLine($"Creando directorio de datos de la aplicación: {AppDataDir}");
                    Directory.CreateDirectory(AppDataDir);
                }

                // Crear el directorio resources si no existe
                if (!Directory.Exists(resourcesDir))
                {
                    Debug.WriteLine($"Creando directorio de recursos: {resourcesDir}");
                    Directory.CreateDirectory(resourcesDir);
                }

                // Si la base de datos persistente no existe, inicializarla desde la plantilla
                if (!File.Exists(PersistentDbPath))
                {
                    Debug.WriteLine($"Inicializando nueva base de datos persistente...");

                    // Buscar una base de datos plantilla, primero en resources, luego en raíz
                    if (File.Exists(templateDbFile))
                    {
                        Debug.WriteLine($"Copiando base de datos desde resources...");
                        File.Copy(templateDbFile, PersistentDbPath);
                        Debug.WriteLine($"Base de datos copiada con éxito a: {PersistentDbPath}");
                    }
                    else if (File.Exists(sourceDbFile))
                    {
                        Debug.WriteLine($"Copiando base de datos desde raíz...");
                        File.Copy(sourceDbFile, PersistentDbPath);
                        Debug.WriteLine($"Base de datos copiada con éxito a: {PersistentDbPath}");
                    }
                    else
                    {
                        Debug.WriteLine($"No se encontró plantilla de base de datos, extrayendo desde recursos embebidos...");
                        ExtractEmbeddedDatabase(PersistentDbPath);
                    }
                }
                else
                {
                    Debug.WriteLine($"Base de datos persistente encontrada en: {PersistentDbPath}");
                }

                // También copiamos la base de datos persistente a la ubicación esperada por connectionSqlite
                // (esto es un parche para no modificar connectionSqlite.cs)
                if (File.Exists(PersistentDbPath))
                {
                    if (!Directory.Exists(resourcesDir))
                    {
                        Directory.CreateDirectory(resourcesDir);
                    }

                    Debug.WriteLine($"Copiando base de datos persistente a ubicación de trabajo...");
                    File.Copy(PersistentDbPath, templateDbFile, true);
                }

                // IMPORTANTE: Guardar la ruta a la base de datos para uso externo
                DatabasePath = PersistentDbPath;
                Debug.WriteLine($"DatabasePath configurado como: {DatabasePath}");

                // *** NUEVA FUNCIONALIDAD: Aplicar migraciones de esquema ***
                ApplySchemaUpdates();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error inicializando base de datos: {ex.Message}");
                // En caso de error, establecer una ruta por defecto
                DatabasePath = "resources\\emotions.sqlite";
            }
        }

        /// <summary>
        /// Aplica actualizaciones de esquema a la base de datos existente
        /// </summary>
        private static void ApplySchemaUpdates()
        {
            try
            {
                using var connection = new SQLiteConnection($"Data Source={DatabasePath};Version=3;");
                connection.Open();

                // Verificar si la columna CurrentUsername ya existe
                if (!ColumnExists(connection, "audioReproductionTimes", "CurrentUsername"))
                {
                    Debug.WriteLine("Agregando columna CurrentUsername a tabla audioReproductionTimes...");

                    var addColumnCommand = new SQLiteCommand(
                        "ALTER TABLE audioReproductionTimes ADD COLUMN CurrentUsername TEXT;",
                        connection);
                    addColumnCommand.ExecuteNonQuery();

                    Debug.WriteLine("Columna CurrentUsername agregada exitosamente.");
                }
                else
                {
                    Debug.WriteLine("La columna CurrentUsername ya existe en la tabla audioReproductionTimes.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error aplicando actualizaciones de esquema: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica si una columna existe en una tabla específica
        /// </summary>
        private static bool ColumnExists(SQLiteConnection connection, string tableName, string columnName)
        {
            try
            {
                var command = new SQLiteCommand($"PRAGMA table_info({tableName});", connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var existingColumnName = reader["name"].ToString();
                    if (string.Equals(existingColumnName, columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error verificando existencia de columna: {ex.Message}");
                return false;
            }
        }

        private static void ExtractEmbeddedDatabase(string outputPath)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Buscar el recurso emotions.sqlite
            var resourceNames = assembly.GetManifestResourceNames();
            var dbResourceName = Array.Find(resourceNames, name => name.EndsWith(DbFileName, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(dbResourceName))
            {
                Debug.WriteLine("No se encontró la base de datos como recurso embebido");
                foreach (var resource in resourceNames)
                {
                    Debug.WriteLine($"Recurso disponible: {resource}");
                }
                return;
            }

            Debug.WriteLine($"Extrayendo base de datos desde recurso: {dbResourceName}");
            using (var stream = assembly.GetManifestResourceStream(dbResourceName))
            {
                if (stream != null)
                {
                    using (var fileStream = new FileStream(outputPath, FileMode.Create))
                    {
                        stream.CopyTo(fileStream);
                    }
                    Debug.WriteLine($"Base de datos extraída con éxito a: {outputPath}");
                }
            }
        }

        // Métodos de utilidad para copia de seguridad
        public static bool BackupDatabase(string backupPath)
        {
            try
            {
                File.Copy(PersistentDbPath, backupPath, true);
                Debug.WriteLine($"Copia de seguridad creada en: {backupPath}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creando copia de seguridad: {ex.Message}");
                return false;
            }
        }

        public static bool RestoreDatabase(string backupPath)
        {
            try
            {
                if (File.Exists(backupPath))
                {
                    File.Copy(backupPath, PersistentDbPath, true);
                    Debug.WriteLine($"Base de datos restaurada desde: {backupPath}");
                    return true;
                }
                else
                {
                    Debug.WriteLine($"El archivo de respaldo no existe: {backupPath}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error restaurando base de datos: {ex.Message}");
                return false;
            }
        }
    }
}