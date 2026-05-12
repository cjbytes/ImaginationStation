using System.Text.Json;
using OllamaHub.Models;

namespace OllamaHub.Services;

public class ChatSessionService
{
    private static readonly string SessionsDir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "OllamaHub", "sessions");

    public List<ChatSession> Sessions { get; } = new();
    public event EventHandler? SessionsChanged;

    public ChatSessionService()
    {
        Directory.CreateDirectory(SessionsDir);
        LoadAll();
    }

    private void LoadAll()
    {
        Sessions.Clear();
        if (!Directory.Exists(SessionsDir)) return;

        foreach (var file in Directory.GetFiles(SessionsDir, "*.json")
                     .OrderByDescending(f => File.GetLastWriteTime(f)))
        {
            try
            {
                var json = File.ReadAllText(file);
                var session = JsonSerializer.Deserialize<ChatSession>(json);
                if (session != null) Sessions.Add(session);
            }
            catch { /* skip corrupt files */ }
        }
    }

    public ChatSession CreateSession(string modelName, string systemPrompt = "")
    {
        var session = new ChatSession
        {
            ModelName = modelName,
            SystemPrompt = systemPrompt,
            Title = $"Chat {DateTime.Now:MMM d, h:mm tt}"
        };
        Sessions.Insert(0, session);
        Save(session);
        SessionsChanged?.Invoke(this, EventArgs.Empty);
        return session;
    }

    public void Save(ChatSession session)
    {
        try
        {
            var json = JsonSerializer.Serialize(session, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path.Combine(SessionsDir, $"{session.Id}.json"), json);
        }
        catch { /* ignore */ }
    }

    public void Delete(ChatSession session)
    {
        Sessions.Remove(session);
        var path = Path.Combine(SessionsDir, $"{session.Id}.json");
        if (File.Exists(path)) File.Delete(path);
        SessionsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateTitle(ChatSession session, string title)
    {
        session.Title = title;
        Save(session);
        SessionsChanged?.Invoke(this, EventArgs.Empty);
    }
}
