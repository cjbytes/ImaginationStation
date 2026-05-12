using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace OllamaHub.Models;

public class PromptTemplate
{
    public string Id       { get; set; } = Guid.NewGuid().ToString();
    public string Name     { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public string Content  { get; set; } = string.Empty;
    public string Icon     { get; set; } = "\uE8BD";
    public DateTime Created { get; set; } = DateTime.UtcNow;
}

public class PromptLibrary
{
    public ObservableCollection<PromptTemplate> Templates { get; set; } = new();
}
