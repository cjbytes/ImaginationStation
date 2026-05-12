using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using OllamaHub.Models;
using OllamaHub.Services;
using OllamaHub.ViewModels;
using Windows.System;

namespace OllamaHub.Views;

public sealed partial class ChatPage : Page
{
    public ChatViewModel ViewModel { get; }
    private readonly ExportService       _export;
    private readonly TokenCounterService _tokens;

    /// <summary>Set this before navigating to pre-fill the input box.</summary>
    public static string? PendingPrompt { get; set; }

    public ChatPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<ChatViewModel>();
        _export   = App.Services.GetRequiredService<ExportService>();
        _tokens   = App.Services.GetRequiredService<TokenCounterService>();

        ViewModel.Messages.CollectionChanged += (_, _) => UpdateTokenCount();
        ModelComboBox.SelectionChanged += ModelComboBox_SelectionChanged;

        Loaded += ChatPage_Loaded;
    }

    private void ChatPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(PendingPrompt))
        {
            ViewModel.InputText = PendingPrompt;
            PendingPrompt = null;
            InputBox.Focus(FocusState.Programmatic);
        }
        else
        {
            InputBox.Focus(FocusState.Programmatic);
        }
    }

    private void UpdateTokenCount()
    {
        var count = _tokens.EstimateMessages(ViewModel.Messages);
        DispatcherQueue.TryEnqueue(() =>
        {
            if (TokenCountLabel != null)
                TokenCountLabel.Text = _tokens.Format(count);
        });
    }

    private void ModelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ModelComboBox.SelectedItem is OllamaModelInfo m)
            ViewModel.SelectedModel = m.Name;
    }

    private void NewChat_Click(object sender, RoutedEventArgs e)
        => ViewModel.NewChatCommand.Execute(null);

    private void SessionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SessionList.SelectedItem is ChatSession s)
            ViewModel.OpenSessionCommand.Execute(s);
    }

    private void InputBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            var ctrl  = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control);
            var shift = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift);
            bool ctrlDown  = ctrl.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);
            bool shiftDown = shift.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

            if (!ctrlDown && !shiftDown)
            {
                SendButton_Click(sender, e);
                e.Handled = true;
            }
        }
    }

    private void SendButton_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.IsGenerating)
            ViewModel.StopGenerationCommand.Execute(null);
        else
            ViewModel.SendMessageCommand.Execute(null);
    }

    private async void ExportMd_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.CurrentSession == null) return;
        var md = _export.ToMarkdown(ViewModel.CurrentSession);
        await _export.SaveToFileAsync(md,
            $"{ViewModel.CurrentSession.Title}.md",
            "Markdown:*.md");
    }

    private async void ExportHtml_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.CurrentSession == null) return;
        var html = _export.ToHtml(ViewModel.CurrentSession);
        await _export.SaveToFileAsync(html,
            $"{ViewModel.CurrentSession.Title}.html",
            "HTML File:*.html");
    }

    private async void ExportTxt_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.CurrentSession == null) return;
        var txt = _export.ToPlainText(ViewModel.CurrentSession);
        await _export.SaveToFileAsync(txt,
            $"{ViewModel.CurrentSession.Title}.txt",
            "Text File:*.txt");
    }
}
