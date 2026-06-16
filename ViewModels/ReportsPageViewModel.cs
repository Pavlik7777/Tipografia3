using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Tipografia3.Models;
using Tipografia3.Services;

namespace Tipografia3.ViewModels;

public partial class ReportsPageViewModel : ViewModelBase
{
    private readonly ReportService _reportService;

    [ObservableProperty]
    private ObservableCollection<CehReport> cehReports = new();

    [ObservableProperty]
    private ObservableCollection<ProductReport> productReports = new();

    [ObservableProperty]
    private ObservableCollection<Product> unsoldProducts = new();

    [ObservableProperty]
    private ObservableCollection<DogovorReport> dogovorReports = new();

    [ObservableProperty]
    private ObservableCollection<TopProductReport> topProducts = new();

    [ObservableProperty]
    private int selectedReportIndex;

    [ObservableProperty]
    private bool hasUnsoldProducts;

    public ReportsPageViewModel()
    {
        _reportService = new ReportService();
        LoadAllReports();
    }

    [RelayCommand]
    private async Task LoadAllReports()
    {
        var cehList = await _reportService.GetCehReportAsync();
        CehReports = new ObservableCollection<CehReport>(cehList);

        var productList = await _reportService.GetProductReportAsync();
        ProductReports = new ObservableCollection<ProductReport>(productList);

        var unsold = await _reportService.GetUnsoldProductsAsync();
        UnsoldProducts = new ObservableCollection<Product>(unsold);
        HasUnsoldProducts = unsold.Any();  // ← добавь

        var dogovorList = await _reportService.GetDogovorReportAsync();
        DogovorReports = new ObservableCollection<DogovorReport>(dogovorList);

        var topList = await _reportService.GetTopProductsAsync();
        TopProducts = new ObservableCollection<TopProductReport>(topList);
    }
}