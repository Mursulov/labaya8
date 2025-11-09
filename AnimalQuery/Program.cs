using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Animal Database - Deserialization from SQLite ===\n");
        
        string dbPath = Path.Combine("..", "database", "animals.db");
        
        if (!File.Exists(dbPath))
        {
            Console.WriteLine($"Error: Database not found at {Path.GetFullPath(dbPath)}");
            Console.WriteLine("Please run AnimalDatabase project first!");
            return;
        }
        
        var db = new DatabaseHelper(dbPath);
        
        Console.WriteLine("=== Retrieving Sample Instances ===\n");
        
        // Get first 3 dogs
        Console.WriteLine("First 3 Dogs:");
        for (int i = 1; i <= 3; i++)
        {
            var dog = db.GetInstance<Dog>(i);
            if (dog != null)
            {
                Console.WriteLine($"  [{i}] {dog}");
                dog.MakeSound();
            }
        }
        
        Console.WriteLine("\nFirst 3 Cats:");
        for (int i = 11; i <= 13; i++)
        {
            var cat = db.GetInstance<Cat>(i);
            if (cat != null)
            {
                Console.WriteLine($"  [{i}] {cat}");
                cat.MakeSound();
            }
        }
        
        // Display all metadata and instances
        db.DisplayClassMetadata();
        db.DisplayAllInstances();
        
        Console.WriteLine("\n✓ Successfully retrieved and deserialized all animals from database!");
    }
}
