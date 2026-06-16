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

public partial class DogovorPageViewModel : ViewModelBase
{
    private readonly DogovorPageService _service;
    private readonly ClientsPageService _clientService;

    [ObservableProperty]
    private ObservableCollection<Dogovor> dogovors = new();

    [ObservableProperty]
    private ObservableCollection<Client> clients = new();

    [ObservableProperty]
    private Dogovor? selectedDogovor;

    public DogovorPageViewModel()
    {
        _service = new DogovorPageService();
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
            var clientList = await _clientService.GetAsync();
            Clients = new ObservableCollection<Client> (clientList);

            // Заполняем имя клиента для каждого договора
            foreach (var dogovor in list)
            {
                var client = clientList.FirstOrDefault(c => c.IdClient == dogovor.IdClient);
                dogovor.ClientName = client?.NameClient ?? "Неизвестно";
            }

            Dogovors = new ObservableCollection<Dogovor>(list);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DogovorPage Load error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task Add()
    {
        var dialog = new DogovorWindow(new Dogovor(), Clients.ToList());
        var result = await dialog.ShowDialog<Dogovor?>(MainWindow.Instance!);
        if (result != null)
        {
            await _service.AddAsync(result);
            await LoadAsync();
        }
    }

    [RelayCommand]
    private async Task Edit(object? param)
    {
        if (param is Dogovor dogovor)
        {
            var dialog = new DogovorWindow(dogovor, Clients.ToList());
            var result = await dialog.ShowDialog<Dogovor?>(MainWindow.Instance!);
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
        if (SelectedDogovor == null)
        {
            await MessageBoxManager.GetMessageBoxStandard(
                "Ошибка",
                "Выберите договор для удаления",
                ButtonEnum.Ok).ShowAsync();
            return;
        }

        var box = MessageBoxManager.GetMessageBoxStandard(
            "Внимание",
            $"Удалить договор №{SelectedDogovor.NumberDogovor}?",
            ButtonEnum.OkCancel);

        if (await box.ShowAsync() == ButtonResult.Ok)
        {
            await _service.DeleteAsync(SelectedDogovor.IdDogovor);
            SelectedDogovor = null;
            await LoadAsync();
        }
    }

    [RelayCommand]
    private async Task Import()
    {
        var topLevel = TopLevel.GetTopLevel(MainWindow.Instance);
        if (topLevel != null)
        {
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Импорт договоров",
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

                ILoadInterface<Dogovor>? loader = ext switch
                {
                    ".csv" => new LoadCSVService<Dogovor>(_service),
                    ".json" => new LoadJSONService<Dogovor>(_service),
                    ".xml" => new LoadXMLService<Dogovor>(_service),
                    _ => null
                };

                if (loader != null)
                {
                    await loader.LoadListAsync(path);
                    await LoadAsync();
                }
            }
        }
    }

    [RelayCommand]
    private async Task Export()
    {
        var topLevel = TopLevel.GetTopLevel(MainWindow.Instance);
        if (topLevel != null)
        {
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Экспорт договоров",
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
                    case ".csv":
                        await new LoadCSVService<Dogovor>(_service).UploadAsync(path, list);
                        break;
                    case ".xlsx":
                        await new LoadExcelService<Dogovor>(_service).UploadAsync(path, list);
                        break;
                    case ".pdf":
                        await new LoadPDFService<Dogovor>(_service).UploadAsync(path, list);
                        break;
                }
            }
        }
    }
}