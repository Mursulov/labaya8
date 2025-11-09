using System;
using Microsoft.Data.Sqlite;
using System.Text.Json;

public class DatabaseHelper
{
    private string connectionString;
    
    public DatabaseHelper(string dbPath)
    {
        connectionString = $"Data Source={dbPath}";
        InitializeDatabase();
    }
    
    private void InitializeDatabase()
    {
        using (var connection = new SqliteConnection(connectionString))
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
            
            using (var command = connection.CreateCommand())
            {
                command.CommandText = createTables;
                command.ExecuteNonQuery();
            }
        }
    }
    
    public int AddClass(string name, string comment)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Classes (Name, Comment) VALUES ($name, $comment); SELECT last_insert_rowid();";
                command.Parameters.AddWithValue("$name", name);
                command.Parameters.AddWithValue("$comment", comment ?? "");
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
    
    public void AddProperty(int classId, string name, string type)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Properties (ClassId, Name, Type) VALUES ($classId, $name, $type)";
                command.Parameters.AddWithValue("$classId", classId);
                command.Parameters.AddWithValue("$name", name);
                command.Parameters.AddWithValue("$type", type);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public void AddMethod(int classId, string name, string signature)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Methods (ClassId, Name, Signature) VALUES ($classId, $name, $signature)";
                command.Parameters.AddWithValue("$classId", classId);
                command.Parameters.AddWithValue("$name", name);
                command.Parameters.AddWithValue("$signature", signature ?? "");
                command.ExecuteNonQuery();
            }
        }
    }
    
    public int AddInstance(string className, object instance)
    {
        string jsonData = JsonSerializer.Serialize(instance);
        
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Instances (ClassName, JsonData) VALUES ($className, $jsonData); SELECT last_insert_rowid();";
                command.Parameters.AddWithValue("$className", className);
                command.Parameters.AddWithValue("$jsonData", jsonData);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
    
    public T GetInstance<T>(int id)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT JsonData FROM Instances WHERE Id = $id";
                command.Parameters.AddWithValue("$id", id);
                var result = command.ExecuteScalar();
                string jsonData = result?.ToString();
                
                if (string.IsNullOrEmpty(jsonData))
                    return default(T);
                
                return JsonSerializer.Deserialize<T>(jsonData);
            }
        }
    }
    
    public void DisplayAllInstances()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Id, ClassName, JsonData FROM Instances";
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
    }
    
    public void DisplayClassMetadata()
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            
            Console.WriteLine("\n=== Classes Metadata ===");
            
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Classes";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int classId = Convert.ToInt32(reader["Id"]);
                        Console.WriteLine($"\n[Class {classId}] {reader["Name"]}");
                        Console.WriteLine($"  Comment: {reader["Comment"]}");
                    }
                }
            }
            
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT c.Name as ClassName, p.Name, p.Type FROM Properties p JOIN Classes c ON p.ClassId = c.Id";
                using (var reader = cmd.ExecuteReader())
                {
                    string currentClass = "";
                    while (reader.Read())
                    {
                        string className = reader["ClassName"].ToString();
                        if (className != currentClass)
                        {
                            if (currentClass != "") Console.WriteLine();
                            Console.WriteLine($"  {className} Properties:");
                            currentClass = className;
                        }
                        Console.WriteLine($"    - {reader["Name"]} ({reader["Type"]})");
                    }
                }
            }
            
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT c.Name as ClassName, m.Name, m.Signature FROM Methods m JOIN Classes c ON m.ClassId = c.Id";
                using (var reader = cmd.ExecuteReader())
                {
                    string currentClass = "";
                    while (reader.Read())
                    {
                        string className = reader["ClassName"].ToString();
                        if (className != currentClass)
                        {
                            if (currentClass != "") Console.WriteLine();
                            Console.WriteLine($"  {className} Methods:");
                            currentClass = className;
                        }
                        Console.WriteLine($"    - {reader["Name"]}{reader["Signature"]}");
                    }
                }
            }
        }
    }
}
