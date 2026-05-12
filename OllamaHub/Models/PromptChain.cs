using System.Collections.Generic;
 
namespace OllamaHub.Models;
 
public class PromptChain
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public List<ChainStep> Steps { get; set; } = new();
}
 
public class ChainStep
{
    public string Name { get; set; } = "";
    public string ModelName { get; set; } = "";
    public string Template { get; set; } = "";
    public int Order { get; set; }
}
