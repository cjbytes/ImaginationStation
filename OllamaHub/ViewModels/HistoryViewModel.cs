using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OllamaHub.Models;
using OllamaHub.Services;

namespace OllamaHub.ViewModels;

public partial class HistoryViewModel : ObservableObject
{
    private readonly ChatSessionService _sessions;

    [ObservableProperty]
    private ObservableCollection<ChatSession> _filteredSessions = new();

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    public HistoryViewModel(ChatSessionService sessions)
    {
        _sessions = sessions;
        _sessions.SessionsChanged += (_, _) =>
            App.UIDispatcher?.TryEnqueue(Refresh);
        Refresh();
    }

    partial void OnSearchQueryChanged(string value) => Refresh();

    public void Refresh()
    {
        FilteredSessions.Clear();
        var query = _sessions.Sessions.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(SearchQuery))
            query = query.Where(s =>
                s.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                s.Messages.Any(m => m.Content.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)));
        foreach (var s in query.OrderByDescending(s => s.LastMessageAt))
            FilteredSessions.Add(s);
    }

    [RelayCommand]
    private void DeleteChat(ChatSession session)
    {
        _sessions.Delete(session);
        FilteredSessions.Remove(session);
    }
}
