using System.Collections.Generic;
using Tipografia3.ViewModels;

namespace Tipografia3.Services;

public class NavigationService
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly Dictionary<string, object> _pages = new();

    public NavigationService(MainWindowViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    public void RegisterPage(string key, object pageViewModel)
    {
        _pages[key] = pageViewModel;
    }

    public void NavigateTo(string key)
    {
        if (_pages.TryGetValue(key, out var page))
        {
            _mainViewModel.CurrentPage = page;
        }
    }
}