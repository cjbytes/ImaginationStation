using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using OllamaHub.Models;
using OllamaHub.ViewModels;

namespace OllamaHub.Views;

public sealed partial class ModelComparePage : Page
{
    public ModelCompareViewModel ViewModel { get; }

    public ModelComparePage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<ModelCompareViewModel>();
        Loaded += (_, _) =>
        {
            ModelABox.SelectionChanged += (s, e) =>
            {
                if (ModelABox.SelectedItem is OllamaModelInfo m) ViewModel.ModelA = m.Name;
            };
            ModelBBox.SelectionChanged += (s, e) =>
            {
                if (ModelBBox.SelectedItem is OllamaModelInfo m) ViewModel.ModelB = m.Name;
            };
        };
    }
}
