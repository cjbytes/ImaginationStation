using OllamaHub.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
 
namespace OllamaHub.Services;
 
public class ChainService
{
    private readonly string _chainsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "OllamaHub", "chains.json");
    
    public async Task<List<PromptChain>> LoadChainsAsync()
    {
        if (!File.Exists(_chainsPath))
            return new List<PromptChain>();
        
        var json = await File.ReadAllTextAsync(_chainsPath);
        return JsonSerializer.Deserialize<List<PromptChain>>(json) ?? new List<PromptChain>();
    }
    
    public async Task SaveChainsAsync(List<PromptChain> chains)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_chainsPath)!);
        var json = JsonSerializer.Serialize(chains, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_chainsPath, json);
    }
    
    public async Task<string> ExecuteChainAsync(PromptChain chain, OllamaService ollama, string initialInput)
    {
        var currentInput = initialInput;
        
        foreach (var step in chain.Steps)
        {
            var prompt = step.Template.Replace("{input}", currentInput);
            
            // Execute step with specified model
            var response = await ollama.GenerateAsync(step.ModelName, prompt);
            currentInput = response;
        }
        
        return currentInput;
    }
}
