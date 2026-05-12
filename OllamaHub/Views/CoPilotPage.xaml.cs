using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
 
namespace OllamaHub.Views;
 
public sealed partial class CoPilotPage : Page
{
    public CoPilotPage()
    {
        InitializeComponent();
    }
    
    private void ExplainCode_Click(object sender, RoutedEventArgs e)
    {
        OutputText.Text = "Analyzing code structure and explaining functionality...";
    }
    
    private void FindBugs_Click(object sender, RoutedEventArgs e)
    {
        OutputText.Text = "Scanning for potential bugs and security issues...";
    }
    
    private void Optimize_Click(object sender, RoutedEventArgs e)
    {
        OutputText.Text = "Analyzing performance bottlenecks and suggesting optimizations...";
    }
    
    private void AddComments_Click(object sender, RoutedEventArgs e)
    {
        OutputText.Text = "Generating inline comments and documentation...";
    }
    
    private void CopyOutput_Click(object sender, RoutedEventArgs e)
    {
        var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
        dataPackage.SetText(OutputText.Text);
        Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
    }
}
