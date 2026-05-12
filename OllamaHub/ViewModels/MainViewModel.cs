using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OllamaHub.Services;

namespace OllamaHub.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly OllamaService _ollama;
    private readonly SettingsService _settings;

    [ObservableProperty]
    private bool _ollamaOnline;

    [ObservableProperty]
    private string _selectedSection = "Chat";

    public MainViewModel(OllamaService ollama, SettingsService settings)
    {
        _ollama = ollama;
        _settings = settings;
        _ = CheckOllamaAsync();
    }

    [RelayCommand]
    private async Task CheckOllamaAsync()
    {
        OllamaOnline = await _ollama.IsOnlineAsync();
        _ = Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(5000);
                var online = await _ollama.IsOnlineAsync();
                App.UIDispatcher?.TryEnqueue(() => OllamaOnline = online);
            }
        });
    }

    [RelayCommand]
    private void NavigateTo(string section) => SelectedSection = section;
}