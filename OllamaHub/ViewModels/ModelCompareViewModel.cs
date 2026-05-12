using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OllamaHub.Models;
using OllamaHub.Services;

namespace OllamaHub.ViewModels;

public partial class ModelCompareViewModel : BaseViewModel
{
    private readonly OllamaService _ollama;

    [ObservableProperty] private ObservableCollection<OllamaModelInfo> _availableModels = new();
    [ObservableProperty] private string _modelA = string.Empty;
    [ObservableProperty] private string _modelB = string.Empty;
    [ObservableProperty] private string _prompt = string.Empty;
    [ObservableProperty] private string _responseA = string.Empty;
    [ObservableProperty] private string _responseB = string.Empty;
    [ObservableProperty] private bool _isRunning;
    [ObservableProperty] private double _msA;
    [ObservableProperty] private double _msB;
    [ObservableProperty] private int _tokensA;
    [ObservableProperty] private int _tokensB;

    public ModelCompareViewModel(OllamaService ollama)
    {
        _ollama = ollama;
        _ = LoadModelsAsync();
    }

    private async Task LoadModelsAsync()
    {
        var models = await _ollama.GetModelsAsync();
        AvailableModels.Clear();
        foreach (var m in models) AvailableModels.Add(m);
        if (models.Count > 0) ModelA = models[0].Name;
        if (models.Count > 1) ModelB = models[1].Name;
    }

    [RelayCommand]
    private async Task RunCompareAsync()
    {
        if (string.IsNullOrWhiteSpace(Prompt) || string.IsNullOrWhiteSpace(ModelA) || string.IsNullOrWhiteSpace(ModelB)) return;
        IsRunning  = true;
        ResponseA  = string.Empty;
        ResponseB  = string.Empty;
        MsA = MsB = 0;
        TokensA = TokensB = 0;

        var taskA = RunModelAsync(ModelA, Prompt, (t) => ResponseA += t, (ms, tok) => { MsA = ms; TokensA = tok; });
        var taskB = RunModelAsync(ModelB, Prompt, (t) => ResponseB += t, (ms, tok) => { MsB = ms; TokensB = tok; });
        await Task.WhenAll(taskA, taskB);
        IsRunning = false;
    }

    private async Task RunModelAsync(string model, string prompt,
        Action<string> onToken, Action<double, int> onDone)
    {
        var start = DateTime.UtcNow;
        int tokens = 0;
        var req = new OllamaChatRequest
        {
            Model    = model,
            Messages = new List<OllamaChatMessage> { new() { Role = "user", Content = prompt } },
            Stream   = true
        };
        try
        {
            await foreach (var tok in _ollama.ChatStreamAsync(req))
            {
                onToken(tok);
                tokens += (int)Math.Ceiling(tok.Length / 4.0);
            }
        }
        catch (Exception ex) { onToken($"\n\n[Error: {ex.Message}]"); }
        onDone((DateTime.UtcNow - start).TotalMilliseconds, tokens);
    }

    [RelayCommand]
    private void ClearAll()
    {
        Prompt = ResponseA = ResponseB = string.Empty;
        MsA = MsB = 0;
        TokensA = TokensB = 0;
    }
}
