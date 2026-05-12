using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using OllamaHub.ViewModels;
using Windows.System;

namespace OllamaHub.Views;

public sealed partial class TerminalPage : Page
{
    public TerminalViewModel ViewModel { get; }

    public TerminalPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<TerminalViewModel>();
        ViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(TerminalViewModel.OutputText))
                DispatcherQueue.TryEnqueue(() =>
                {
                    OutputScroll.UpdateLayout();
                    OutputScroll.ScrollToVerticalOffset(OutputScroll.ExtentHeight);
                });
        };
    }

    private void CommandInput_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        switch (e.Key)
        {
            case VirtualKey.Enter:
                ViewModel.RunCommandCommand.Execute(null);
                e.Handled = true;
                break;
            case VirtualKey.Up:
                ViewModel.HistoryUp();
                CommandInput.SelectionStart = CommandInput.Text.Length;
                e.Handled = true;
                break;
            case VirtualKey.Down:
                ViewModel.HistoryDown();
                CommandInput.SelectionStart = CommandInput.Text.Length;
                e.Handled = true;
                break;
        }
    }
}
