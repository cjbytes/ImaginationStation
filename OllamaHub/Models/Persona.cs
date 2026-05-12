namespace OllamaHub.Models;
 
public class Persona
{
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "🤖";
    public string SystemPrompt { get; set; } = "";
    public double Temperature { get; set; } = 0.7;
    public int MaxTokens { get; set; } = 2000;
    public string Description { get; set; } = "";
}
