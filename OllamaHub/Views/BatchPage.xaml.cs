using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
 
namespace OllamaHub.Views;
 
public sealed partial class BatchPage : Page
{
    public BatchPage()
    {
        InitializeComponent();
    }
    
    private void RunBatch_Click(object sender, RoutedEventArgs e)
    {
        // Run prompt across all available models
    }
    
    private void CompareResults_Click(object sender, RoutedEventArgs e)
    {
        // Show side-by-side comparison
    }
    
    private void ExportReport_Click(object sender, RoutedEventArgs e)
    {
        // Export results as CSV or JSON
    }
}
