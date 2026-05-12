using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OllamaHub.Models;
using OllamaHub.Services;

namespace OllamaHub.ViewModels;

public partial class ChatViewModel : BaseViewModel
{
    private readonly OllamaService _ollama;
    private readonly ChatSessionService _sessions;
    private readonly SettingsService _settings;

    private CancellationTokenSource? _cts;

    [ObservableProperty]
    private ObservableCollection<ChatSession> _chatSessions = new();

    [ObservableProperty]
    private ChatSession? _currentSession;

    [ObservableProperty]
    private ObservableCollection<ChatMessage> _messages = new();

    [ObservableProperty]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<OllamaModelInfo> _availableModels = new();

    [ObservableProperty]
    private string _selectedModel = string.Empty;

    [ObservableProperty]
    private bool _isGenerating;

    [ObservableProperty]
    private string _statusText = "Ready";

    public ChatViewModel(OllamaService ollama, ChatSessionService sessions, SettingsService settings)
    {
        _ollama = ollama;
        _sessions = sessions;
        _settings = settings;

        foreach (var s in _sessions.Sessions)
            ChatSessions.Add(s);

        SelectedModel = _settings.Current.DefaultModel;
        _ = LoadModelsAsync();
    }

    [RelayCommand]
    private async Task LoadModelsAsync()
    {
        var models = await _ollama.GetModelsAsync();
        AvailableModels.Clear();
        foreach (var m in models) AvailableModels.Add(m);

        if (string.IsNullOrEmpty(SelectedModel) && models.Count > 0)
            SelectedModel = models[0].Name;
    }

    [RelayCommand]
    private void NewChat()
    {
        if (string.IsNullOrEmpty(SelectedModel)) return;
        var session = _sessions.CreateSession(SelectedModel, _settings.Current.DefaultSystemPrompt);
        ChatSessions.Insert(0, session);
        OpenSession(session);
    }

    [RelayCommand]
    private void OpenSession(ChatSession session)
    {
        CurrentSession = session;
        Messages.Clear();
        foreach (var msg in session.Messages)
            Messages.Add(msg);
        SelectedModel = session.ModelName;
    }

    [RelayCommand]
    private void DeleteSession(ChatSession session)
    {
        _sessions.Delete(session);
        ChatSessions.Remove(session);
        if (CurrentSession == session)
        {
            CurrentSession = null;
            Messages.Clear();
        }
    }

    [RelayCommand]
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(InputText) || IsGenerating) return;
        if (string.IsNullOrEmpty(SelectedModel)) return;

        // Create session if needed
        if (CurrentSession == null)
        {
            var session = _sessions.CreateSession(SelectedModel, _settings.Current.DefaultSystemPrompt);
            ChatSessions.Insert(0, session);
            OpenSession(session);
        }

        var userMsg = new ChatMessage { Role = "user", Content = InputText.Trim() };
        Messages.Add(userMsg);
        CurrentSession!.Messages.Add(userMsg);

        // Auto-title from first message
        if (CurrentSession.Messages.Count == 1)
        {
            var title = InputText.Length > 40 ? InputText[..40] + "…" : InputText;
            _sessions.UpdateTitle(CurrentSession, title);
        }

        InputText = string.Empty;
        IsGenerating = true;
        StatusText = $"Generating with {SelectedModel}...";

        var assistantMsg = new ChatMessage { Role = "assistant", Content = "", IsStreaming = true, ModelName = SelectedModel };
        Messages.Add(assistantMsg);
        CurrentSession.Messages.Add(assistantMsg);

        _cts = new CancellationTokenSource();
        var start = DateTime.UtcNow;

        try
        {
            // Build message history for context
            var history = CurrentSession.Messages
                .Where(m => !m.IsStreaming || m == assistantMsg)
                .Where(m => m != assistantMsg)
                .Select(m => new OllamaChatMessage { Role = m.Role, Content = m.Content })
                .ToList();

            // Add system prompt if present
            if (!string.IsNullOrEmpty(CurrentSession.SystemPrompt))
                history.Insert(0, new OllamaChatMessage { Role = "system", Content = CurrentSession.SystemPrompt });

            var request = new OllamaChatRequest
            {
                Model = SelectedModel,
                Messages = history,
                Stream = _settings.Current.StreamResponses,
                Options = new OllamaOptions
                {
                    Temperature = _settings.Current.Temperature,
                    NumPredict = _settings.Current.MaxTokens,
                    TopP = _settings.Current.TopP,
                    TopK = _settings.Current.TopK
                }
            };

            await foreach (var token in _ollama.ChatStreamAsync(request, _cts.Token))
            {
                assistantMsg.Content += token;
                // Notify UI of content change
                var idx = Messages.IndexOf(assistantMsg);
                if (idx >= 0)
                {
                    Messages.RemoveAt(idx);
                    Messages.Insert(idx, assistantMsg);
                }
            }
        }
        catch (OperationCanceledException)
        {
            assistantMsg.Content += "\n\n[Stopped]";
        }
        catch (Exception ex)
        {
            assistantMsg.Content = $"Error: {ex.Message}";
        }
        finally
        {
            assistantMsg.IsStreaming = false;
            assistantMsg.GenerationMs = (DateTime.UtcNow - start).TotalMilliseconds;

            // Refresh last message
            var idx2 = Messages.IndexOf(assistantMsg);
            if (idx2 >= 0) { Messages.RemoveAt(idx2); Messages.Insert(idx2, assistantMsg); }

            _sessions.Save(CurrentSession!);
            IsGenerating = false;
            StatusText = $"Done in {assistantMsg.GenerationMs:F0}ms";
        }
    }

    [RelayCommand]
    private void StopGeneration()
    {
        _cts?.Cancel();
    }

    [RelayCommand]
    private void ClearCurrentChat()
    {
        if (CurrentSession == null) return;
        CurrentSession.Messages.Clear();
        Messages.Clear();
        _sessions.Save(CurrentSession);
    }

    partial void OnSelectedModelChanged(string value)
    {
        if (CurrentSession != null)
            CurrentSession.ModelName = value;
    }
}
