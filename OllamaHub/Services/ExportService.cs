using System.Text;
using OllamaHub.Models;

namespace OllamaHub.Services;

public class ExportService
{
    public string ToMarkdown(ChatSession session)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# {session.Title}");
        sb.AppendLine($"> Model: {session.ModelName}  |  Created: {session.CreatedAt:yyyy-MM-dd HH:mm}");
        sb.AppendLine();
        foreach (var msg in session.Messages)
        {
            if (msg.Role == "system") continue;
            sb.AppendLine(msg.Role == "user" ? "## You" : $"## {msg.ModelName ?? "Assistant"}");
            sb.AppendLine(msg.Content);
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public string ToPlainText(ChatSession session)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"=== {session.Title} ===");
        sb.AppendLine($"Model: {session.ModelName}  |  {session.CreatedAt:yyyy-MM-dd HH:mm}");
        sb.AppendLine(new string('-', 60));
        foreach (var msg in session.Messages)
        {
            if (msg.Role == "system") continue;
            var label = msg.Role == "user" ? "YOU" : "ASSISTANT";
            sb.AppendLine($"[{label}]");
            sb.AppendLine(msg.Content);
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public string ToHtml(ChatSession session)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'>");
        sb.AppendLine($"<title>{HtmlEncode(session.Title)}</title>");
        sb.AppendLine(@"<style>
body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
       max-width: 800px; margin: 0 auto; padding: 2rem; background: #0f0f0f; color: #e0e0e0; }
h1   { color: #7c7cff; }
.meta { color: #888; font-size: 0.85rem; margin-bottom: 2rem; }
.msg  { margin: 1rem 0; padding: 1rem 1.25rem; border-radius: 12px; }
.user { background: #1a1a4a; border-left: 3px solid #7c7cff; }
.assistant { background: #1a2a1a; border-left: 3px solid #4caf50; }
.role { font-size: 0.75rem; font-weight: 600; text-transform: uppercase;
        letter-spacing: 1px; margin-bottom: 0.5rem; color: #aaa; }
pre  { white-space: pre-wrap; margin: 0; font-size: 0.95rem; line-height: 1.6; }
</style></head><body>");
        sb.AppendLine($"<h1>{HtmlEncode(session.Title)}</h1>");
        sb.AppendLine($"<div class='meta'>Model: {session.ModelName} &nbsp;|&nbsp; {session.CreatedAt:yyyy-MM-dd HH:mm}</div>");
        foreach (var msg in session.Messages)
        {
            if (msg.Role == "system") continue;
            var cls   = msg.Role == "user" ? "user" : "assistant";
            var label = msg.Role == "user" ? "You" : (msg.ModelName ?? "Assistant");
            sb.AppendLine($"<div class='msg {cls}'><div class='role'>{HtmlEncode(label)}</div>");
            sb.AppendLine($"<pre>{HtmlEncode(msg.Content)}</pre></div>");
        }
        sb.AppendLine("</body></html>");
        return sb.ToString();
    }

    /// <summary>Minimal HTML encoder â€” avoids System.Web dependency.</summary>
    private static string HtmlEncode(string s) => s
        .Replace("&",  "&amp;")
        .Replace("<",  "&lt;")
        .Replace(">",  "&gt;")
        .Replace("\"", "&quot;")
        .Replace("'",  "&#39;");

    public async Task SaveToFileAsync(string content, string defaultName, string filter)
    {
        var picker = new Windows.Storage.Pickers.FileSavePicker();
        picker.SuggestedFileName = defaultName;

        // Get HWND from the active MainWindow
        var window = App.Current.MainWindowHandle
                     ?? throw new InvalidOperationException("No main window");
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        foreach (var f in filter.Split('|'))
        {
            var parts = f.Split(':');
            if (parts.Length == 2)
                picker.FileTypeChoices.Add(parts[0], new[] { parts[1] });
        }

        var file = await picker.PickSaveFileAsync();
        if (file != null)
            await Windows.Storage.FileIO.WriteTextAsync(file, content);
    }
}
