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

public partial class CehPageViewModel : ViewModelBase
{
    private readonly CehPageService _service;

    [ObservableProperty]
    private ObservableCollection<TipografCeh> cehs = new();

    [ObservableProperty]
    private TipografCeh? selectedCeh;

    public CehPageViewModel()
    {
        _service = new CehPageService();
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() => await LoadAsync();

    private async Task LoadAsync()
    {
        try
        {
            var list = await _service.GetAsync();
            Cehs = new ObservableCollection<TipografCeh>(list);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CehPage Load error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task Add()
    {
        var dialog = new CehWindow(new TipografCeh());
        var result = await dialog.ShowDialog<TipografCeh?>(MainWindow.Instance!);
        if (result != null)
        {
            await _service.AddAsync(result);
            await LoadAsync();
        }
    }

    [RelayCommand]
    private async Task Edit()
    {
        if (SelectedCeh == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите цех", ButtonEnum.Ok).ShowAsync();
            return;
        }

        var dialog = new CehWindow(SelectedCeh);
        var result = await dialog.ShowDialog<TipografCeh?>(MainWindow.Instance!);
        if (result != null)
        {
            await _service.EditAsync(result);
            await LoadAsync();
        }
    }

    [RelayCommand]
    private async Task DeleteSelected()
    {
        if (SelectedCeh == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("Ошибка", "Выберите цех", ButtonEnum.Ok).ShowAsync();
            return;
        }

        var box = MessageBoxManager.GetMessageBoxStandard("Внимание", $"Удалить '{SelectedCeh.NameCeh}'?", ButtonEnum.OkCancel);
        if (await box.ShowAsync() == ButtonResult.Ok)
        {
            await _service.DeleteAsync(SelectedCeh.IdTipograf);
            SelectedCeh = null;
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
            Title = "Импорт цехов",
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

            ILoadInterface<TipografCeh>? loader = ext switch
            {
                ".csv" => new LoadCSVService<TipografCeh>(_service),
                ".json" => new LoadJSONService<TipografCeh>(_service),
                ".xml" => new LoadXMLService<TipografCeh>(_service),
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
            Title = "Экспорт цехов",
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
                case ".csv": await new LoadCSVService<TipografCeh>(_service).UploadAsync(path, list); break;
                case ".xlsx": await new LoadExcelService<TipografCeh>(_service).UploadAsync(path, list); break;
                case ".pdf": await new LoadPDFService<TipografCeh>(_service).UploadAsync(path, list); break;
            }
        }
    }
}