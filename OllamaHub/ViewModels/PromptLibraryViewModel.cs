using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OllamaHub.Models;
using OllamaHub.Services;

namespace OllamaHub.ViewModels;

public partial class PromptLibraryViewModel : BaseViewModel
{
    private readonly PromptLibraryService _svc;

    public ObservableCollection<PromptTemplate> Templates => _svc.Templates;

    [ObservableProperty] private string _newName     = string.Empty;
    [ObservableProperty] private string _newCategory = "General";
    [ObservableProperty] private string _newContent  = string.Empty;
    [ObservableProperty] private PromptTemplate? _selected;
    [ObservableProperty] private string _searchText  = string.Empty;
    [ObservableProperty] private ObservableCollection<PromptTemplate> _filtered = new();

    public List<string> Categories { get; } = new()
        { "General", "Coding", "Writing", "Learning", "Language", "Creative", "Other" };

    public PromptLibraryViewModel(PromptLibraryService svc)
    {
        _svc = svc;
        RefreshFiltered();
    }

    partial void OnSearchTextChanged(string value) => RefreshFiltered();

    public void RefreshFiltered()
    {
        Filtered.Clear();
        var q = SearchText.Trim().ToLowerInvariant();
        foreach (var t in Templates)
            if (string.IsNullOrEmpty(q) ||
                t.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                t.Category.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                t.Content.Contains(q, StringComparison.OrdinalIgnoreCase))
                Filtered.Add(t);
    }

    [RelayCommand]
    private void AddTemplate()
    {
        if (string.IsNullOrWhiteSpace(NewName) || string.IsNullOrWhiteSpace(NewContent)) return;
        var t = new PromptTemplate
        {
            Name     = NewName.Trim(),
            Category = NewCategory,
            Content  = NewContent.Trim()
        };
        _svc.Add(t);
        NewName    = string.Empty;
        NewContent = string.Empty;
        RefreshFiltered();
    }

    [RelayCommand]
    private void DeleteTemplate(PromptTemplate t)
    {
        _svc.Delete(t);
        RefreshFiltered();
    }

    [RelayCommand]
    private void SaveSelected()
    {
        if (Selected == null) return;
        _svc.Update(Selected);
        RefreshFiltered();
    }
}
