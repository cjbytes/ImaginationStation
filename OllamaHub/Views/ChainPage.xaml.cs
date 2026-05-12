using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
 
namespace OllamaHub.Views;
 
public sealed partial class ChainPage : Page
{
    public ChainPage()
    {
        InitializeComponent();
        LoadChains();
    }
    
    private void LoadChains()
    {
        // Load from ChainService
    }
    
    private void SaveChain_Click(object sender, RoutedEventArgs e)
    {
        // Save current chain
    }
    
    private void ExecuteChain_Click(object sender, RoutedEventArgs e)
    {
        // Execute chain with input
    }
    
    private void AddStep_Click(object sender, RoutedEventArgs e)
    {
        // Add new step to chain
    }
}
