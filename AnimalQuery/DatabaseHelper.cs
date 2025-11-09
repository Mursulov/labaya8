using System;
using System.Data.SQLite;
using System.Text.Json;

public class DatabaseHelper
{
    private string connectionString;
    
    public DatabaseHelper(string dbPath)
    {
        connectionString = $"Data Source={dbPath};Version=3;";
        InitializeDatabase();
    }
    
    private void InitializeDatabase()
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string createTables = @"
                CREATE TABLE IF NOT EXISTS Classes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                    Name TEXT NOT NULL, 
                    Comment TEXT
                );
                
                CREATE TABLE IF NOT EXISTS Properties (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                    ClassId INTEGER NOT NULL, 
                    Name TEXT NOT NULL, 
                    Type TEXT NOT NULL,
                    FOREIGN KEY (ClassId) REFERENCES Classes(Id)
                );
                
                CREATE TABLE IF NOT EXISTS Methods (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                    ClassId INTEGER NOT NULL, 
                    Name TEXT NOT NULL, 
                    Signature TEXT,
                    FOREIGN KEY (ClassId) REFERENCES Classes(Id)
                );
                
                CREATE TABLE IF NOT EXISTS Attributes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                    ClassId INTEGER NOT NULL, 
                    Type TEXT NOT NULL, 
                    Data TEXT,
                    FOREIGN KEY (ClassId) REFERENCES Classes(Id)
                );
                
                CREATE TABLE IF NOT EXISTS Instances (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                    ClassName TEXT NOT NULL, 
                    JsonData TEXT NOT NULL
                );
            ";
            
            using (var command = new SQLiteCommand(createTables, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
    
    public int AddClass(string name, string comment)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string sql = "INSERT INTO Classes (Name, Comment) VALUES (@name, @comment); SELECT last_insert_rowid();";
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@comment", comment ?? "");
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
    
    public void AddProperty(int classId, string name, string type)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string sql = "INSERT INTO Properties (ClassId, Name, Type) VALUES (@classId, @name, @type)";
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@classId", classId);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@type", type);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public void AddMethod(int classId, string name, string signature)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string sql = "INSERT INTO Methods (ClassId, Name, Signature) VALUES (@classId, @name, @signature)";
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@classId", classId);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@signature", signature ?? "");
                command.ExecuteNonQuery();
            }
        }
    }
    
    public int AddInstance(string className, object instance)
    {
        string jsonData = JsonSerializer.Serialize(instance);
        
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string sql = "INSERT INTO Instances (ClassName, JsonData) VALUES (@className, @jsonData); SELECT last_insert_rowid();";
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@className", className);
                command.Parameters.AddWithValue("@jsonData", jsonData);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
    
    public T GetInstance<T>(int id)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string sql = "SELECT JsonData FROM Instances WHERE Id = @id";
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                string jsonData = command.ExecuteScalar()?.ToString();
                
                if (string.IsNullOrEmpty(jsonData))
                    return default(T);
                
                return JsonSerializer.Deserialize<T>(jsonData);
            }
        }
    }
    
    public void DisplayAllInstances()
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string sql = "SELECT Id, ClassName, JsonData FROM Instances";
            using (var command = new SQLiteCommand(sql, connection))
            using (var reader = command.ExecuteReader())
            {
                Console.WriteLine("\n=== All Instances ===");
                while (reader.Read())
                {
                    Console.WriteLine($"\nID: {reader["Id"]}");
                    Console.WriteLine($"Class: {reader["ClassName"]}");
                    Console.WriteLine($"Data: {reader["JsonData"]}");
                }
            }
        }
    }
    
    public void DisplayClassMetadata()
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            
            Console.WriteLine("\n=== Classes Metadata ===");
            
            string classSql = "SELECT * FROM Classes";
            using (var cmd = new SQLiteCommand(classSql, connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int classId = Convert.ToInt32(reader["Id"]);
                    Console.WriteLine($"\n[Class {classId}] {reader["Name"]}");
                    Console.WriteLine($"  Comment: {reader["Comment"]}");
                    
                    var propSql = "SELECT * FROM Properties WHERE ClassId = @classId";
                    using (var propCmd = new SQLiteCommand(propSql, connection))
                    {
                        propCmd.Parameters.AddWithValue("@classId", classId);
                        using (var propReader = propCmd.ExecuteReader())
                        {
                            Console.WriteLine("  Properties:");
                            while (propReader.Read())
                            {
                                Console.WriteLine($"    - {propReader["Name"]} ({propReader["Type"]})");
                            }
                        }
                    }
                    
                    var methodSql = "SELECT * FROM Methods WHERE ClassId = @classId";
                    using (var methodCmd = new SQLiteCommand(methodSql, connection))
                    {
                        methodCmd.Parameters.AddWithValue("@classId", classId);
                        using (var methodReader = methodCmd.ExecuteReader())
                        {
                            Console.WriteLine("  Methods:");
                            while (methodReader.Read())
                            {
                                Console.WriteLine($"    - {methodReader["Name"]}{methodReader["Signature"]}");
                            }
                        }
                    }
                }
            }
        }
    }
}
