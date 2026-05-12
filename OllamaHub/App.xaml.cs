using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using OllamaHub.Services;
using OllamaHub.ViewModels;
 
namespace OllamaHub;
 
public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;
    public static new App Current => (App)Application.Current;
    public static DispatcherQueue? UIDispatcher { get; private set; }
 
    private Window? _window;
    public Window? MainWindowHandle => _window;
 
    public App()
    {
        InitializeComponent();
        Services = ConfigureServices();
    }
 
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
 
        // Core Services
        services.AddSingleton<OllamaService>();
        services.AddSingleton<ChatSessionService>();
        services.AddSingleton<SettingsService>();
        services.AddSingleton<ThemeService>();
        services.AddSingleton<PromptLibraryService>();
        services.AddSingleton<ExportService>();
        services.AddSingleton<TokenCounterService>();
        
        // New Services
        services.AddSingleton<PersonaService>();
        services.AddSingleton<ChainService>();
        
        // Proxy Service (if exists)
        if (System.IO.File.Exists("Services/Shadowsocks/ShadowsocksClient.cs"))
        {
            services.AddSingleton<IProxyService, ShadowsocksClient>();
        }
 
        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<ChatViewModel>();
        services.AddTransient<ModelsViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<HistoryViewModel>();
        services.AddTransient<PromptLibraryViewModel>();
        services.AddTransient<ModelCompareViewModel>();
        services.AddTransient<TerminalViewModel>();
 
        return services.BuildServiceProvider();
    }
 
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        UIDispatcher = DispatcherQueue.GetForCurrentThread();
        _window      = new MainWindow();
        _window.Activate();
    }
}
