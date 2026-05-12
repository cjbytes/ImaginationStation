using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
 
namespace OllamaHub.Views;
 
public sealed partial class PersonasPage : Page
{
    public PersonasPage()
    {
        InitializeComponent();
        LoadPersonas();
    }
    
    private void LoadPersonas()
    {
        // Load from PersonaService
    }
    
    private void CreatePersona_Click(object sender, RoutedEventArgs e)
    {
        // Show create persona dialog
    }
    
    private void PersonaCard_Click(object sender, ItemClickEventArgs e)
    {
        // Navigate to chat with this persona
    }
    
    private void EditPersona_Click(object sender, RoutedEventArgs e)
    {
        // Show edit persona dialog
    }
}
