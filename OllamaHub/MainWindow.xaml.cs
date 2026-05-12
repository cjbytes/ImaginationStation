using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using OllamaHub.Services;
using OllamaHub.ViewModels;
using OllamaHub.Views;
using Windows.UI;
using Microsoft.UI.Dispatching;
 
namespace OllamaHub;
 
public sealed partial class MainWindow : Window
{
    private readonly MainViewModel _vm;
    private readonly ThemeService  _theme;
    private Button? _activeBtn;
 
    public MainWindow()
    {
        InitializeComponent();
        _vm    = App.Services.GetRequiredService<MainViewModel>();
        _theme = App.Services.GetRequiredService<ThemeService>();
 
        _vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.OllamaOnline))
                UpdateStatusIndicator();
        };
        UpdateStatusIndicator();
        _theme.Apply(this);
 
        ContentFrame.Navigate(typeof(ChatPage));
        SetActiveButton(BtnChat);
    }
 
    private void UpdateStatusIndicator()
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            if (_vm.OllamaOnline)
            {
                StatusDot.Fill   = new SolidColorBrush(Color.FromArgb(255, 52, 199, 89));
                StatusLabel.Text = "Online";
            }
            else
            {
                StatusDot.Fill   = new SolidColorBrush(Color.FromArgb(255, 255, 69, 58));
                StatusLabel.Text = "Offline";
            }
        });
    }
 
    private void NavBtn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        SetActiveButton(btn);
        ContentFrame.Navigate(btn.Tag?.ToString() switch
        {
            "Chat"     => typeof(ChatPage),
            "Models"   => typeof(ModelsPage),
            "History"  => typeof(HistoryPage),
            "Prompts"  => typeof(PromptLibraryPage),
            "CoPilot"  => typeof(CoPilotPage),
            "Sandbox"  => typeof(SandboxPage),
            "Batch"    => typeof(BatchPage),
            "Personas" => typeof(PersonasPage),
            "Chain"    => typeof(ChainPage),
            "Compare"  => typeof(ModelComparePage),
            "Terminal" => typeof(TerminalPage),
            "Settings" => typeof(SettingsPage),
            "Guide"    => typeof(GuidePage),
            _          => typeof(ChatPage)
        });
    }
 
    private void SetActiveButton(Button btn)
    {
        if (_activeBtn != null)
            _activeBtn.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        _activeBtn = btn;
        btn.Background = new SolidColorBrush(Color.FromArgb(40, 138, 99, 255));
    }
}
