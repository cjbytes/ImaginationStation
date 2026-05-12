using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using OllamaHub.Models;
using OllamaHub.ViewModels;

namespace OllamaHub.Views;

public sealed partial class PromptLibraryPage : Page
{
    public PromptLibraryViewModel ViewModel { get; }

    public PromptLibraryPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<PromptLibraryViewModel>();
    }

    private void TemplateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TemplateList.SelectedItem is PromptTemplate t)
        {
            ViewModel.Selected   = t;
            ViewModel.NewName    = t.Name;
            ViewModel.NewCategory= t.Category;
            ViewModel.NewContent = t.Content;
        }
    }

    private void AddNew_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.Selected    = null;
        ViewModel.NewName     = string.Empty;
        ViewModel.NewCategory = "General";
        ViewModel.NewContent  = string.Empty;
        TemplateList.SelectedItem = null;
    }

    private void Save_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (ViewModel.Selected != null)
        {
            ViewModel.Selected.Name     = ViewModel.NewName;
            ViewModel.Selected.Category = ViewModel.NewCategory;
            ViewModel.Selected.Content  = ViewModel.NewContent;
            ViewModel.SaveSelectedCommand.Execute(null);
        }
        else
        {
            ViewModel.AddTemplateCommand.Execute(null);
        }
    }

    private void Delete_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (ViewModel.Selected != null)
            ViewModel.DeleteTemplateCommand.Execute(ViewModel.Selected);
    }

    private void UsePrompt_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // Navigate to Chat with the prompt pre-filled
        if (Frame.Parent is Microsoft.UI.Xaml.Controls.Frame parentFrame)
        {
            // Store prompt for ChatPage to pick up
            ChatPage.PendingPrompt = ViewModel.NewContent;
            parentFrame.Navigate(typeof(ChatPage));
        }
    }
}
