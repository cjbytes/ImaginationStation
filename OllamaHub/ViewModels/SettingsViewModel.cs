using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OllamaHub.Services;

namespace OllamaHub.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly SettingsService _settings;

    [ObservableProperty] private string _ollamaUrl = string.Empty;
    [ObservableProperty] private string _defaultModel = string.Empty;
    [ObservableProperty] private string _defaultSystemPrompt = string.Empty;
    [ObservableProperty] private float _temperature;
    [ObservableProperty] private int _maxTokens;
    [ObservableProperty] private float _topP;
    [ObservableProperty] private int _topK;
    [ObservableProperty] private string _theme = "Dark";
    [ObservableProperty] private bool _streamResponses;
    [ObservableProperty] private bool _sendWithEnter;
    [ObservableProperty] private bool _showTimestamps;
    [ObservableProperty] private bool _showTokenStats;

    public List<string> Themes { get; } = new() { "Dark", "Light", "System Default" };

    public SettingsViewModel(SettingsService settings)
    {
        _settings = settings;
        LoadFromSettings();
    }

    private void LoadFromSettings()
    {
        var s = _settings.Current;
        OllamaUrl = s.OllamaBaseUrl;
        DefaultModel = s.DefaultModel;
        DefaultSystemPrompt = s.DefaultSystemPrompt;
        Temperature = s.Temperature;
        MaxTokens = s.MaxTokens;
        TopP = s.TopP;
        TopK = s.TopK;
        Theme = s.Theme;
        StreamResponses = s.StreamResponses;
        SendWithEnter = s.SendWithEnter;
        ShowTimestamps = s.ShowTimestamps;
        ShowTokenStats = s.ShowTokenStats;
    }

    [RelayCommand]
    private void Save()
    {
        _settings.Update(s =>
        {
            s.OllamaBaseUrl = OllamaUrl;
            s.DefaultModel = DefaultModel;
            s.DefaultSystemPrompt = DefaultSystemPrompt;
            s.Temperature = Temperature;
            s.MaxTokens = MaxTokens;
            s.TopP = TopP;
            s.TopK = TopK;
            s.Theme = Theme;
            s.StreamResponses = StreamResponses;
            s.SendWithEnter = SendWithEnter;
            s.ShowTimestamps = ShowTimestamps;
            s.ShowTokenStats = ShowTokenStats;
        });
        StatusMessage = "Settings saved!";
    }

    [RelayCommand]
    private void Reset()
    {
        _settings.Update(_ => { });
        LoadFromSettings();
        StatusMessage = "Settings reset to defaults.";
    }
}
