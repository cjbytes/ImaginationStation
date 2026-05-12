namespace OllamaHub.Services;
 
public class TokenCounterService
{
    // Rough estimate: 1 token ≈ 4 characters for English text
    public int EstimateTokens(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;
        
        // More accurate estimation using word count
        var words = text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        return (int)(words.Length * 1.3); // Average 1.3 tokens per word
    }
    
    public string FormatTokenCount(int tokens)
    {
        if (tokens < 1000)
            return $"{tokens} tokens";
        
        return $"{tokens / 1000.0:F1}K tokens";
    }
}
