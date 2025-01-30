using System.Text.Json;

namespace ConsoleApp.Services.Storage;

public class JsonService
{
    public static List<T> LoadFromJson<T>(string filePath)
    {
        var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Файл {fullPath} не знайдено.");
        }

        var json = File.ReadAllText(fullPath);
        return JsonSerializer.Deserialize<List<T>>(json) ?? [];
    }
}