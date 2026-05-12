using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OllamaHub.Models;
using OllamaHub.Services;

namespace OllamaHub.ViewModels;

public partial class ModelsViewModel : BaseViewModel
{
    private readonly OllamaService _ollama;
    private CancellationTokenSource? _pullCts;

    [ObservableProperty]
    private ObservableCollection<OllamaModelInfo> _localModels = new();

    [ObservableProperty]
    private string _pullModelName = string.Empty;

    [ObservableProperty]
    private string _pullStatus = string.Empty;

    [ObservableProperty]
    private double _pullProgress;

    [ObservableProperty]
    private bool _isPulling;

    // Popular models for quick-pull
    public List<string> PopularModels { get; } = new()
    {
        "llama3.2:3b", "llama3.2:1b", "llama3.1:8b", "llama3.1:70b",
        "mistral:7b", "mistral-nemo", "phi4", "phi3.5",
        "gemma2:2b", "gemma2:9b", "gemma2:27b",
        "qwen2.5:7b", "qwen2.5:14b", "qwen2.5-coder:7b",
        "deepseek-r1:7b", "deepseek-r1:14b",
        "nomic-embed-text", "mxbai-embed-large",
        "codellama:7b", "codellama:13b",
        "neural-chat:7b", "starling-lm:7b"
    };

    public ModelsViewModel(OllamaService ollama)
    {
        _ollama = ollama;
        _ = RefreshModelsAsync();
    }

    [RelayCommand]
    private async Task RefreshModelsAsync()
    {
        IsBusy = true;
        var models = await _ollama.GetModelsAsync();
        LocalModels.Clear();
        foreach (var m in models.OrderBy(m => m.Name))
            LocalModels.Add(m);
        IsBusy = false;
    }

    [RelayCommand]
    private async Task PullModelAsync()
    {
        if (string.IsNullOrWhiteSpace(PullModelName) || IsPulling) return;

        IsPulling = true;
        PullStatus = "Starting download...";
        PullProgress = 0;
        _pullCts = new CancellationTokenSource();

        try
        {
            await foreach (var update in _ollama.PullModelAsync(PullModelName.Trim(), _pullCts.Token))
            {
                PullStatus = update.Status ?? "";
                if (update.Total.HasValue && update.Total > 0 && update.Completed.HasValue)
                    PullProgress = (double)update.Completed.Value / update.Total.Value * 100.0;
            }
            PullStatus = "Download complete!";
            PullModelName = string.Empty;
            await RefreshModelsAsync();
        }
        catch (OperationCanceledException)
        {
            PullStatus = "Cancelled.";
        }
        catch (Exception ex)
        {
            PullStatus = $"Error: {ex.Message}";
        }
        finally
        {
            IsPulling = false;
        }
    }

    [RelayCommand]
    private void CancelPull()
    {
        _pullCts?.Cancel();
    }

    [RelayCommand]
    private async Task DeleteModelAsync(OllamaModelInfo model)
    {
        if (model == null) return;
        var ok = await _ollama.DeleteModelAsync(model.Name);
        if (ok)
        {
            LocalModels.Remove(model);
            PullStatus = $"Deleted {model.Name}";
        }
    }

    [RelayCommand]
    private void QuickPull(string modelName)
    {
        PullModelName = modelName;
    }
}
