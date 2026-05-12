using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
 
namespace OllamaHub.Services;
 
public class ThemeService
{
    public void Apply(Window window)
    {
        // Apply dark theme by default
        if (window.Content is FrameworkElement root)
        {
            root.RequestedTheme = ElementTheme.Dark;
        }
    }
    
    public void SetAccentColor(Color color)
    {
        // Can be extended to dynamically change accent colors
    }
}
