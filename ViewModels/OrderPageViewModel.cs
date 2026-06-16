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

public partial class OrderPageViewModel : ViewModelBase
{
    private readonly OrderPageService _service;
    private readonly DogovorPageService _dogovorService;
    private readonly ProductPageService _productService;
    private readonly ClientsPageService _clientService;

    [ObservableProperty]
    private ObservableCollection<Order> orders = new();

    [ObservableProperty]
    private ObservableCollection<Dogovor> dogovors = new();

    [ObservableProperty]
    private ObservableCollection<Product> products = new();

    [ObservableProperty]
    private ObservableCollection<Client> clients = new ();

    [ObservableProperty]
    private Order? selectedOrder;

    public OrderPageViewModel()
    {
        _service = new OrderPageService();
        _dogovorService = new DogovorPageService();
        _productService = new ProductPageService();
        _clientService = new ClientsPageService();
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        try
        {
            var list = await _service.GetAsync();
            var dogovorList = await _dogovorService.GetAsync();
            var productList = await _productService.GetAsync();
            var clientList = await _clientService.GetAsync();

            Dogovors = new ObservableCollection<Dogovor>(dogovorList);
            Products = new ObservableCollection<Product>(productList);
            Clients = new ObservableCollection<Client> (clientList);

            // Заполняем вычисляемые свойства для каждого заказа
            foreach (var order in list)
            {
                // Находим договор
                var dogovor = dogovorList.FirstOrDefault(d => d.NumberDogovor == order.NumberDogovor);
                var client = dogovor != null
                    ? clientList.FirstOrDefault(c => c.IdClient == dogovor.IdClient)
                    : null;
                order.DogovorInfo = dogovor != null && client != null
                    ? $"Дог. №{dogovor.NumberDogovor} — {client.NameClient}"
                    : "Неизвестно";

                // Находим продукт
                var product = productList.FirstOrDefault(p => p.IdProduct == order.IdProduct);
                order.ProductName = product?.NameProduct ?? "Неизвестно";
            }

            Orders = new ObservableCollection<Order>(list);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"OrderPage Load error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task Add()
    {
        var dialog = new OrderWindow(new Order(), Dogovors.ToList(), Products.ToList());
        var result = await dialog.ShowDialog<Order?>(MainWindow.Instance!);
        if (result != null)
        {
            await _service.AddAsync(result);
            await LoadAsync();
        }
    }

    [RelayCommand]
    private async Task Edit(object? param)
    {
        if (param is Order order)
        {
            var dialog = new OrderWindow(order, Dogovors.ToList(), Products.ToList());
            var result = await dialog.ShowDialog<Order?>(MainWindow.Instance!);
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
        if (SelectedOrder == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите заказ", ButtonEnum.Ok).ShowAsync();
            return;
        }

        var box = MessageBoxManager.GetMessageBoxStandard("Внимание", $"Удалить заказ №{SelectedOrder.IdOrder}?", ButtonEnum.OkCancel);
        if (await box.ShowAsync() == ButtonResult.Ok)
        {
            await _service.DeleteAsync(SelectedOrder.IdOrder);
            SelectedOrder = null;
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
            Title = "Импорт заказов",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("CSV") { Patterns = new[] { "*.csv" } },
                new FilePickerFileType("Excel") { Patterns = new[] { "*.xlsx" } },
                new FilePickerFileType("JSON") { Patterns = new[] { "*.json" } },
                new FilePickerFileType("XML") { Patterns = new[] { "*.xml" } }
            }
        });

        if (files.Count > 0)
        {
            var path = files[0].Path.AbsolutePath;
            var ext = Path.GetExtension(path).ToLower();

            ILoadInterface<Order>? loader = ext switch
            {
                ".csv" => new LoadCSVService<Order>(_service),
                ".xlsx" => new LoadExcelService<Order>(_service),
                ".json" => new LoadJSONService<Order>(_service),
                ".xml" => new LoadXMLService<Order>(_service),
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
            Title = "Экспорт заказов",
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
                case ".csv": await new LoadCSVService<Order>(_service).UploadAsync(path, list); break;
                case ".xlsx": await new LoadExcelService<Order>(_service).UploadAsync(path, list); break;
                case ".pdf": await new LoadPDFService<Order>(_service).UploadAsync(path, list); break;
            }
        }
    }
}