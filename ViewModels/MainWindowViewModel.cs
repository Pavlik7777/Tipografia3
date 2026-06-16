using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tipografia3.Services;

namespace Tipografia3.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;

    [ObservableProperty]
    private object? currentPage;

    [ObservableProperty]
    private string title = "Типография";

    public MainWindowViewModel()
    {
        _navigationService = new NavigationService(this);

        _navigationService.RegisterPage("Home", new HomePageViewModel());
        _navigationService.RegisterPage("Ceh", new CehPageViewModel());
        _navigationService.RegisterPage("Product", new ProductPageViewModel());
        _navigationService.RegisterPage("Dogovor", new DogovorPageViewModel());
        _navigationService.RegisterPage("Order", new OrderPageViewModel());
        _navigationService.RegisterPage("Reports", new ReportsPageViewModel());
        _navigationService.RegisterPage("Charts", new ChartsPageViewModel());

        _navigationService.NavigateTo("Home");
    }

    [RelayCommand]
    public void Navigate(string pageKey)
    {
        _navigationService.NavigateTo(pageKey);
    }
}