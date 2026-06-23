using Avalonia.Controls;
using Avalonia.Platform.Storage;
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
    private readonly PdfReportService _pdfService;

    [ObservableProperty] private ObservableCollection<CehReport> cehReports = new();
    [ObservableProperty] private ObservableCollection<ProductReport> productReports = new();
    [ObservableProperty] private ObservableCollection<Product> unsoldProducts = new();
    [ObservableProperty] private ObservableCollection<DogovorReport> dogovorReports = new();
    [ObservableProperty] private ObservableCollection<TopProductReport> topProducts = new();
    [ObservableProperty] private int selectedReportIndex;
    [ObservableProperty] private bool hasUnsoldProducts;

    public ReportsPageViewModel()
    {
        _reportService = new ReportService();
        _pdfService = new PdfReportService();
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
        HasUnsoldProducts = unsold.Any();

        var dogovorList = await _reportService.GetDogovorReportAsync();
        DogovorReports = new ObservableCollection<DogovorReport>(dogovorList);

        var topList = await _reportService.GetTopProductsAsync();
        TopProducts = new ObservableCollection<TopProductReport>(topList);
    }

    // Сохранить текущий отчёт в PDF
    [RelayCommand]
    private async Task SavePdf()
    {
        // Получаем окно для показа диалога
        var window = Avalonia.Application.Current?.ApplicationLifetime is
            Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

        if (window == null) return;

        // Определяем имя файла по текущей вкладке
        string defaultName = SelectedReportIndex switch
        {
            0 => "Сводка_по_цехам.pdf",
            1 => "Сводка_по_продукции.pdf",
            2 => "Невостребованная_продукция.pdf",
            3 => "Сводка_по_договорам.pdf",
            4 => "Топ-5_продукции.pdf",
            _ => "Отчёт.pdf"
        };

        // Диалог сохранения файла
        var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Сохранить отчёт в PDF",
            SuggestedFileName = defaultName,
            FileTypeChoices = new[]
            {
                new FilePickerFileType("PDF документ") { Patterns = new[] { "*.pdf" } }
            }
        });

        if (file == null) return;

        var path = file.Path.LocalPath;

        // Вызываем нужный метод в зависимости от вкладки
        switch (SelectedReportIndex)
        {
            case 0: _pdfService.SaveCehReport(path, CehReports); break;
            case 1: _pdfService.SaveProductReport(path, ProductReports); break;
            case 2: _pdfService.SaveUnsoldReport(path, UnsoldProducts); break;
            case 3: _pdfService.SaveDogovorReport(path, DogovorReports); break;
            case 4: _pdfService.SaveTopProductReport(path, TopProducts); break;
        }
    }
}
