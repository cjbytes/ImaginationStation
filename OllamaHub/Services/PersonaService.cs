using OllamaHub.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
 
namespace OllamaHub.Services;
 
public class PersonaService
{
    private readonly string _personasPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "OllamaHub", "personas.json");
    
    public async Task<List<Persona>> LoadPersonasAsync()
    {
        if (!File.Exists(_personasPath))
            return GetDefaultPersonas();
        
        var json = await File.ReadAllTextAsync(_personasPath);
        return JsonSerializer.Deserialize<List<Persona>>(json) ?? GetDefaultPersonas();
    }
    
    public async Task SavePersonasAsync(List<Persona> personas)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_personasPath)!);
        var json = JsonSerializer.Serialize(personas, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_personasPath, json);
    }
    
    private List<Persona> GetDefaultPersonas()
    {
        return new List<Persona>
        {
            new Persona
            {
                Name = "Code Assistant",
                Icon = "👨‍💻",
                SystemPrompt = "You are an expert software engineer. Provide clean, efficient code with explanations. Focus on best practices and maintainable solutions.",
                Temperature = 0.3,
                MaxTokens = 4000
            },
            new Persona
            {
                Name = "Creative Writer",
                Icon = "✍️",
                SystemPrompt = "You are a creative writer. Craft engaging narratives, vivid descriptions, and compelling dialogue. Be imaginative and expressive.",
                Temperature = 0.9,
                MaxTokens = 2000
            },
            new Persona
            {
                Name = "Data Analyst",
                Icon = "📊",
                SystemPrompt = "You are a data analyst. Analyze data, identify patterns, and provide insights. Be precise and evidence-based in your conclusions.",
                Temperature = 0.2,
                MaxTokens = 3000
            },
            new Persona
            {
                Name = "Teacher",
                Icon = "👨‍🏫",
                SystemPrompt = "You are a patient teacher. Explain concepts clearly with examples. Break down complex topics into digestible parts.",
                Temperature = 0.5,
                MaxTokens = 2500
            }
        };
    }
}
