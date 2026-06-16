using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tipografia3.Services;

namespace Tipografia3.ViewModels;

public partial class ChartsPageViewModel : ViewModelBase
{
    private readonly ChartService _chartService;

    [ObservableProperty]
    private ISeries[] ordersByCehSeries = Array.Empty<ISeries>();

    [ObservableProperty]
    private Axis[] ordersByCehXAxes = Array.Empty<Axis>();

    [ObservableProperty]
    private ISeries[] ordersByProductSeries = Array.Empty<ISeries>();

    [ObservableProperty]
    private ISeries[] ordersByMonthSeries = Array.Empty<ISeries>();

    [ObservableProperty]
    private Axis[] ordersByMonthXAxes = Array.Empty<Axis>();

    [ObservableProperty]
    private ISeries[] priceDistributionSeries = Array.Empty<ISeries>();

    [ObservableProperty]
    private Axis[] priceDistributionXAxes = Array.Empty<Axis>();

    public ChartsPageViewModel()
    {
        _chartService = new ChartService();

        // Инициализация осей (чтобы графики не были совсем пустыми до загрузки)
        OrdersByCehXAxes = new Axis[] { new Axis { TextSize = 12 } };
        OrdersByMonthXAxes = new Axis[] { new Axis { TextSize = 12 } };
        PriceDistributionXAxes = new Axis[] { new Axis { TextSize = 12 } };
    }

    // Публичный метод для вызова из View при загрузке
    public async Task InitializeAsync()
    {
        await LoadChartsData();
    }

    // Команда для кнопки "Обновить"
    [RelayCommand]
    private async Task LoadCharts()
    {
        await LoadChartsData();
    }

    // Общий метод загрузки данных
    private async Task LoadChartsData()
    {
        await LoadOrdersByCeh();
        await LoadOrdersByProduct();
        await LoadOrdersByMonth();
        await LoadPriceDistribution();
    }

    private async Task LoadOrdersByCeh()
    {
        var data = await _chartService.GetOrdersByCehAsync();
        var labels = data.Keys.ToArray();
        var values = data.Values.ToArray();

        OrdersByCehSeries = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Values = values,
                Name = "Заказы по цехам",
                Fill = new SolidColorPaint(SKColors.CornflowerBlue)
            }
        };

        OrdersByCehXAxes = new Axis[]
        {
            new Axis
            {
                Labels = labels,
                TextSize = 12
            }
        };
    }

    private async Task LoadOrdersByProduct()
    {
        var data = await _chartService.GetOrdersByProductAsync();

        OrdersByProductSeries = data.Select(x => new PieSeries<double>
        {
            Values = new[] { x.Value },
            Name = x.Key,
            DataLabelsPaint = new SolidColorPaint(SKColors.Black),
            DataLabelsSize = 12,
            DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer
        }).ToArray<ISeries>();
    }

    private async Task LoadOrdersByMonth()
    {
        var data = await _chartService.GetOrdersByMonthAsync();
        var labels = data.Keys.ToArray();
        var values = data.Values.ToArray();

        OrdersByMonthSeries = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = values,
                Name = "Динамика заказов",
                GeometrySize = 15,
                Stroke = new SolidColorPaint(SKColors.CornflowerBlue, 3),
                Fill = null
            }
        };

        OrdersByMonthXAxes = new Axis[]
        {
            new Axis
            {
                Labels = labels,
                TextSize = 12
            }
        };
    }

    private async Task LoadPriceDistribution()
    {
        var prices = await _chartService.GetPriceDistributionAsync();

        var ranges = new Dictionary<string, int>
        {
            ["0-50"] = 0,
            ["50-100"] = 0,
            ["100-200"] = 0,
            ["200-350"] = 0
        };

        foreach (var price in prices)
        {
            if (price <= 50) ranges["0-50"]++;
            else if (price <= 100) ranges["50-100"]++;
            else if (price <= 200) ranges["100-200"]++;
            else ranges["200-350"]++;
        }

        var labels = ranges.Keys.ToArray();
        var values = ranges.Values.Select(x => (double)x).ToArray();

        PriceDistributionSeries = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Values = values,
                Name = "Количество видов",
                Fill = new SolidColorPaint(SKColors.MediumSeaGreen)
            }
        };

        PriceDistributionXAxes = new Axis[]
        {
            new Axis
            {
                Labels = labels,
                TextSize = 12
            }
        };
    }
}