using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tipografia3.Models;
using Tipografia3.Services;
using Tipografia3.Views;

namespace Tipografia3.ViewModels;

public partial class ProductPageViewModel : ViewModelBase
{
    private readonly ProductPageService _service;
    private readonly CehPageService _cehService;

    [ObservableProperty]
    private ObservableCollection<Product> products = new();

    [ObservableProperty]
    private ObservableCollection<TipografCeh> cehs = new();

    [ObservableProperty]
    private Product? selectedProduct;

    public ProductPageViewModel()
    {
        _service = new ProductPageService();
        _cehService = new CehPageService();
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        var list = await _service.GetAsync();
        var cehList = await _cehService.GetAsync();
        Cehs = new ObservableCollection<TipografCeh>(cehList);

        foreach (var product in list)
        {
            var ceh = cehList.FirstOrDefault(c => c.NumberCeh == product.NumberCeh);
            product.NameCeh = ceh?.NameCeh ?? "Неизвестно";
        }

        Products = new ObservableCollection<Product>(list);
    }

    [RelayCommand]
    private async Task Add()
    {
        var dialog = new ProductWindow(new Product());
        var result = await dialog.ShowDialog<Product?>(MainWindow.Instance!);
        if (result != null)
        {
            await _service.AddAsync(result);
            await LoadAsync();
        }
    }

    [RelayCommand]
    private async Task Edit(object? param)
    {
        if (param is Product product)
        {
            var dialog = new ProductWindow(product);
            var result = await dialog.ShowDialog<Product?>(MainWindow.Instance!);
            if (result != null)
            {
                await _service.EditAsync(result);
                await LoadAsync();
            }
        }
    }

    [RelayCommand]
    private async Task DeleteSelected()
    {
        if (SelectedProduct == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите продукт", ButtonEnum.Ok).ShowAsync();
            return;
        }

        var box = MessageBoxManager.GetMessageBoxStandard("Внимание", $"Удалить '{SelectedProduct.NameProduct}'?", ButtonEnum.OkCancel);
        if (await box.ShowAsync() == ButtonResult.Ok)
        {
            await _service.DeleteAsync(SelectedProduct.IdProduct);
            SelectedProduct = null;
            await LoadAsync();
        }
    }

    [RelayCommand]
    private async Task Import()
    {
        var topLevel = TopLevel.GetTopLevel(MainWindow.Instance);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Импорт продукции",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("CSV") { Patterns = new[] { "*.csv" } },
                new FilePickerFileType("JSON") { Patterns = new[] { "*.json" } },
                new FilePickerFileType("XML") { Patterns = new[] { "*.xml" } }
            }
        });

        if (files.Count > 0)
        {
            var path = files[0].Path.AbsolutePath;
            var ext = Path.GetExtension(path).ToLower();

            ILoadInterface<Product>? loader = ext switch
            {
                ".csv" => new LoadCSVService<Product>(_service),
                ".json" => new LoadJSONService<Product>(_service),
                ".xml" => new LoadXMLService<Product>(_service),
                _ => null
            };

            if (loader != null)
            {
                await loader.LoadListAsync(path);
                await LoadAsync();
            }
        }
    }

    [RelayCommand]
    private async Task Export()
    {
        var topLevel = TopLevel.GetTopLevel(MainWindow.Instance);
        if (topLevel == null) return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Экспорт продукции",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("CSV") { Patterns = new[] { "*.csv" } },
                new FilePickerFileType("Excel") { Patterns = new[] { "*.xlsx" } },
                new FilePickerFileType("PDF") { Patterns = new[] { "*.pdf" } }
            }
        });

        if (file != null)
        {
            var path = file.Path.AbsolutePath;
            var ext = Path.GetExtension(path).ToLower();
            var list = await _service.GetAsync();

            switch (ext)
            {
                case ".csv": await new LoadCSVService<Product>(_service).UploadAsync(path, list); break;
                case ".xlsx": await new LoadExcelService<Product>(_service).UploadAsync(path, list); break;
                case ".pdf": await new LoadPDFService<Product>(_service).UploadAsync(path, list); break;
            }
        }
    }
}