using System.Collections.ObjectModel;
using System.Text.Json;
using OllamaHub.Models;

namespace OllamaHub.Services;

public class PromptLibraryService
{
    private readonly string _path;
    public ObservableCollection<PromptTemplate> Templates { get; } = new();

    public PromptLibraryService()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "OllamaHub");
        Directory.CreateDirectory(dir);
        _path = Path.Combine(dir, "prompts.json");
        Load();
        if (Templates.Count == 0) SeedDefaults();
    }

    private void Load()
    {
        if (!File.Exists(_path)) return;
        try
        {
            var json = File.ReadAllText(_path);
            var list = JsonSerializer.Deserialize<List<PromptTemplate>>(json);
            if (list != null) foreach (var t in list) Templates.Add(t);
        }
        catch { }
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(Templates.ToList(),
            new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_path, json);
    }

    public void Add(PromptTemplate t)    { Templates.Add(t); Save(); }
    public void Delete(PromptTemplate t) { Templates.Remove(t); Save(); }
    public void Update(PromptTemplate t) { Save(); }

    private void SeedDefaults()
    {
        var defaults = new[]
        {
            new PromptTemplate { Name="Explain Like I'm 5",   Category="Learning",   Content="Explain the following in simple terms a 5-year-old could understand:\n\n",           Icon="\uE82D" },
            new PromptTemplate { Name="Code Review",          Category="Coding",     Content="Review this code for bugs, performance issues, and best practices:\n\n```\n\n```",   Icon="\uE943" },
            new PromptTemplate { Name="Write Unit Tests",     Category="Coding",     Content="Write comprehensive unit tests for the following code:\n\n```\n\n```",               Icon="\uE8C4" },
            new PromptTemplate { Name="Summarize",            Category="Writing",    Content="Summarize the following text concisely:\n\n",                                        Icon="\uE8D2" },
            new PromptTemplate { Name="Fix Grammar",          Category="Writing",    Content="Fix grammar, spelling, and clarity in the following text:\n\n",                     Icon="\uE8AB" },
            new PromptTemplate { Name="Translate to English", Category="Language",   Content="Translate the following to English:\n\n",                                           Icon="\uE8C1" },
            new PromptTemplate { Name="SQL Query Helper",     Category="Coding",     Content="Write a SQL query to:\n\n",                                                         Icon="\uE8D7" },
            new PromptTemplate { Name="Brainstorm Ideas",     Category="Creative",   Content="Brainstorm 10 creative ideas for:\n\n",                                             Icon="\uE90F" },
            new PromptTemplate { Name="Debug This Error",     Category="Coding",     Content="Help me debug this error:\n\nError:\n\nCode:\n\n```\n\n```",                        Icon="\uEBE8" },
            new PromptTemplate { Name="Write a Blog Post",    Category="Writing",    Content="Write an engaging blog post about:\n\n",                                            Icon="\uE8F4" },
        };
        foreach (var d in defaults) Templates.Add(d);
        Save();
    }
}
