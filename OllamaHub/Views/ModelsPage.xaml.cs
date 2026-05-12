using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using OllamaHub.ViewModels;

namespace OllamaHub.Views;

public sealed partial class ModelsPage : Page
{
    public ModelsViewModel ViewModel { get; }

    public ModelsPage()
    {
        ViewModel = App.Services.GetRequiredService<ModelsViewModel>();
        InitializeComponent();
    }
}