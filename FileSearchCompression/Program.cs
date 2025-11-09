using System;
using System.IO;
using System.IO.Compression;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== File Search and Compression Tool ===\n");
        
        Console.Write("Enter directory path to search: ");
        string searchPath = Console.ReadLine();
        
        if (!Directory.Exists(searchPath))
        {
            Console.WriteLine("Directory not found!");
            return;
        }
        
        Console.Write("Enter filename to search for (e.g., *.db, *.txt): ");
        string searchFileName = Console.ReadLine();
        
        Console.WriteLine($"\nSearching for '{searchFileName}' in '{searchPath}'...\n");
        SearchFiles(searchPath, searchFileName);
    }
    
    static void SearchFiles(string path, string fileName)
    {
        try
        {
            string[] files = Directory.GetFiles(path, fileName);
            
            foreach (string file in files)
            {
                Console.WriteLine($"✓ Found: {file}");
                ProcessFile(file);
            }
            
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                SearchFiles(directory, fileName);
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"Access denied: {path}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
    
    static void ProcessFile(string filePath)
    {
        Console.WriteLine("\n--- File Content (FileStream) ---");
        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(fs))
            {
                string content = reader.ReadToEnd();
                Console.WriteLine(content.Length > 500 
                    ? content.Substring(0, 500) + "..." 
                    : content);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Cannot read as text: {ex.Message}");
            
            // Show binary file info
            FileInfo fi = new FileInfo(filePath);
            Console.WriteLine($"Binary file - Size: {fi.Length} bytes");
        }
        
        Console.Write("\nCompress this file? (y/n): ");
        string response = Console.ReadLine()?.ToLower();
        
        if (response == "y")
        {
            CompressFile(filePath);
        }
        
        Console.WriteLine();
    }
    
    static void CompressFile(string filePath)
    {
        string compressedPath = filePath + ".gz";
        
        try
        {
            using (FileStream sourceStream = new FileStream(filePath, FileMode.Open))
            using (FileStream targetStream = new FileStream(compressedPath, FileMode.Create))
            using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
            {
                sourceStream.CopyTo(compressionStream);
            }
            
            FileInfo originalFile = new FileInfo(filePath);
            FileInfo compressedFile = new FileInfo(compressedPath);
            
            Console.WriteLine($"✓ File compressed successfully!");
            Console.WriteLine($"  Original size: {originalFile.Length} bytes");
            Console.WriteLine($"  Compressed size: {compressedFile.Length} bytes");
            Console.WriteLine($"  Compression ratio: {100 - (compressedFile.Length * 100 / originalFile.Length)}%");
            Console.WriteLine($"  Saved to: {compressedPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Compression failed: {ex.Message}");
        }
    }
}
