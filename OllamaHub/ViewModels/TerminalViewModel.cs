using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OllamaHub.ViewModels;

public partial class TerminalViewModel : BaseViewModel
{
    [ObservableProperty] private string _inputCommand = string.Empty;
    [ObservableProperty] private string _outputText   = string.Empty;
    [ObservableProperty] private bool   _isRunning;
    [ObservableProperty] private string _workingDir   = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    private readonly List<string> _history = new();
    private int _historyIdx = -1;

    public ObservableCollection<string> CommandHistory { get; } = new();

    [RelayCommand]
    private async Task RunCommandAsync()
    {
        if (string.IsNullOrWhiteSpace(InputCommand) || IsRunning) return;

        var cmd = InputCommand.Trim();
        CommandHistory.Insert(0, cmd);
        _history.Insert(0, cmd);
        _historyIdx = -1;

        // Handle built-in cd
        if (cmd.StartsWith("cd ", StringComparison.OrdinalIgnoreCase))
        {
            var target = cmd[3..].Trim().Trim('"');
            var newDir = Path.IsPathRooted(target) ? target : Path.Combine(WorkingDir, target);
            if (Directory.Exists(newDir)) WorkingDir = Path.GetFullPath(newDir);
            else AppendOutput($"cd: directory not found: {target}");
            InputCommand = string.Empty;
            return;
        }
        if (cmd.Equals("clear", StringComparison.OrdinalIgnoreCase) ||
            cmd.Equals("cls",   StringComparison.OrdinalIgnoreCase))
        {
            OutputText   = string.Empty;
            InputCommand = string.Empty;
            return;
        }

        AppendOutput($"\n> {cmd}");
        IsRunning    = true;
        InputCommand = string.Empty;

        try
        {
            var psi = new ProcessStartInfo("cmd.exe", $"/c {cmd}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                UseShellExecute        = false,
                CreateNoWindow         = true,
                WorkingDirectory       = WorkingDir
            };
            using var proc = Process.Start(psi)!;
            var stdout = await proc.StandardOutput.ReadToEndAsync();
            var stderr = await proc.StandardError.ReadToEndAsync();
            await proc.WaitForExitAsync();
            if (!string.IsNullOrEmpty(stdout)) AppendOutput(stdout.TrimEnd());
            if (!string.IsNullOrEmpty(stderr)) AppendOutput($"[ERR] {stderr.TrimEnd()}");
        }
        catch (Exception ex) { AppendOutput($"[Error] {ex.Message}"); }
        finally { IsRunning = false; }
    }

    private void AppendOutput(string text)
    {
        var dispatcher = App.UIDispatcher;
        if (dispatcher != null)
            dispatcher.TryEnqueue(() => OutputText += text + "\n");
        else
            OutputText += text + "\n";
    }

    public void HistoryUp()
    {
        if (_history.Count == 0) return;
        _historyIdx = Math.Min(_historyIdx + 1, _history.Count - 1);
        InputCommand = _history[_historyIdx];
    }

    public void HistoryDown()
    {
        _historyIdx = Math.Max(_historyIdx - 1, -1);
        InputCommand = _historyIdx < 0 ? string.Empty : _history[_historyIdx];
    }

    [RelayCommand]
    private void ClearOutput() { OutputText = string.Empty; }
}
