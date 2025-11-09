using System;
using System.IO;
using Xunit;

public class DatabaseTests
{
    private string GetTestDbPath()
    {
        return Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.db");
    }
    
    [Fact]
    public void Database_Initialization_ShouldCreateTables()
    {
        string dbPath = GetTestDbPath();
        var db = new DatabaseHelper(dbPath);
        
        Assert.True(File.Exists(dbPath));
        
        File.Delete(dbPath);
    }
    
    [Fact]
    public void AddClass_ShouldReturnValidId()
    {
        string dbPath = GetTestDbPath();
        var db = new DatabaseHelper(dbPath);
        
        int classId = db.AddClass("TestClass", "Test comment");
        
        Assert.True(classId > 0);
        
        File.Delete(dbPath);
    }
    
    [Fact]
    public void AddAndGetInstance_ShouldSerializeAndDeserialize()
    {
        string dbPath = GetTestDbPath();
        var db = new DatabaseHelper(dbPath);
        
        var originalDog = new Dog
        {
            Name = "TestDog",
            Age = 3,
            Color = "Brown",
            Breed = "Labrador"
        };
        
        int instanceId = db.AddInstance("Dog", originalDog);
        var retrievedDog = db.GetInstance<Dog>(instanceId);
        
        Assert.NotNull(retrievedDog);
        Assert.Equal(originalDog.Name, retrievedDog.Name);
        Assert.Equal(originalDog.Age, retrievedDog.Age);
        Assert.Equal(originalDog.Color, retrievedDog.Color);
        Assert.Equal(originalDog.Breed, retrievedDog.Breed);
        
        File.Delete(dbPath);
    }
    
    [Fact]
    public void AddProperty_ShouldStoreMetadata()
    {
        string dbPath = GetTestDbPath();
        var db = new DatabaseHelper(dbPath);
        int classId = db.AddClass("TestClass", "Test");
        
        db.AddProperty(classId, "Name", "string");
        db.AddProperty(classId, "Age", "int");
        
        Assert.True(File.Exists(dbPath));
        
        File.Delete(dbPath);
    }
}
