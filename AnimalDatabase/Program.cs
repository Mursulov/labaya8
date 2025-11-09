using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Animal Database - Serialization to SQLite ===\n");
        
        string dbPath = Path.Combine("..", "database", "animals.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath));
        
        if (File.Exists(dbPath))
        {
            File.Delete(dbPath);
            Console.WriteLine("Old database deleted.\n");
        }
        
        var db = new DatabaseHelper(dbPath);
        
        int dogClassId = db.AddClass("Dog", "Dog class inherited from Animal");
        db.AddProperty(dogClassId, "Name", "string");
        db.AddProperty(dogClassId, "Age", "int");
        db.AddProperty(dogClassId, "Color", "string");
        db.AddProperty(dogClassId, "Breed", "string");
        db.AddMethod(dogClassId, "MakeSound", "()");
        db.AddMethod(dogClassId, "ToString", "() : string");
        
        int catClassId = db.AddClass("Cat", "Cat class inherited from Animal");
        db.AddProperty(catClassId, "Name", "string");
        db.AddProperty(catClassId, "Age", "int");
        db.AddProperty(catClassId, "Color", "string");
        db.AddProperty(catClassId, "IsIndoor", "bool");
        db.AddMethod(catClassId, "MakeSound", "()");
        db.AddMethod(catClassId, "ToString", "() : string");
        
        Console.WriteLine("=== Adding 20 Animal Instances ===\n");
        
        string[] dogNames = { "Buddy", "Max", "Charlie", "Cooper", "Rocky", "Duke", "Bear", "Zeus", "Jack", "Oliver" };
        string[] dogBreeds = { "Golden Retriever", "Labrador", "German Shepherd", "Bulldog", "Beagle", "Poodle", "Rottweiler", "Husky", "Boxer", "Dachshund" };
        string[] dogColors = { "Golden", "Black", "Brown", "White", "Tan", "Gray", "Spotted", "Cream", "Red", "Brindle" };
        
        for (int i = 0; i < 10; i++)
        {
            var dog = new Dog
            {
                Name = dogNames[i],
                Age = (i % 12) + 1,
                Color = dogColors[i],
                Breed = dogBreeds[i]
            };
            
            int id = db.AddInstance("Dog", dog);
            Console.WriteLine($"✓ [{id}] Dog added: {dog.Name} ({dog.Breed})");
        }
        
        Console.WriteLine();
        
        string[] catNames = { "Whiskers", "Luna", "Simba", "Bella", "Oliver", "Milo", "Lucy", "Chloe", "Leo", "Shadow" };
        string[] catColors = { "White", "Black", "Orange", "Gray", "Calico", "Tabby", "Siamese", "Tuxedo", "Brown", "Cream" };
        bool[] indoorStatus = { true, true, false, true, false, true, true, false, true, false };
        
        for (int i = 0; i < 10; i++)
        {
            var cat = new Cat
            {
                Name = catNames[i],
                Age = (i % 15) + 1,
                Color = catColors[i],
                IsIndoor = indoorStatus[i]
            };
            
            int id = db.AddInstance("Cat", cat);
            Console.WriteLine($"✓ [{id}] Cat added: {cat.Name} ({cat.Color}, Indoor: {cat.IsIndoor})");
        }
        
        Console.WriteLine("\n=== Database Statistics ===");
        Console.WriteLine($"Total Classes: 2 (Dog, Cat)");
        Console.WriteLine($"Total Properties: 8");
        Console.WriteLine($"Total Methods: 4");
        Console.WriteLine($"Total Instances: 20 (10 Dogs + 10 Cats)");
        
        db.DisplayClassMetadata();
        db.DisplayAllInstances();
        
        Console.WriteLine($"\n✓ Database saved to: {Path.GetFullPath(dbPath)}");
    }
}
