using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
 
namespace OllamaHub.Views;
 
public sealed partial class SandboxPage : Page
{
    public SandboxPage()
    {
        InitializeComponent();
    }
    
    private void RunPreview_Click(object sender, RoutedEventArgs e)
    {
        // In production, this would inject HTML/CSS/JS into WebView2
        // For now, placeholder functionality
    }
}
