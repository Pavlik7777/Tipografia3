using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using System.Threading.Tasks;
using Tipografia3.ViewModels;

namespace Tipografia3.Views;

public partial class ChartsPageView : UserControl
{
    private async Task SaveChart(Control chart, string fileName)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel == null)
            return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(
            new FilePickerSaveOptions
            {
                SuggestedFileName = fileName,
                DefaultExtension = "png"
            });

        if (file == null)
            return;

        var bitmap = new RenderTargetBitmap(
            new PixelSize(
                (int)chart.Bounds.Width,
                (int)chart.Bounds.Height));

        bitmap.Render(chart);

        await using var stream = await file.OpenWriteAsync();

        bitmap.Save(stream);
    }
    public ChartsPageView()
    {
        InitializeComponent();

        this.AttachedToVisualTree += OnAttachedToVisualTree;
    }

    private async void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is ChartsPageViewModel vm)
        {
            await vm.InitializeAsync();
        }
    }
    private async void SaveOrdersByCehChart_Click(object? sender, RoutedEventArgs e)
    {
        var chart = this.FindControl<Control>("OrdersByCehChart");

        if (chart != null)
            await SaveChart(chart, "OrdersByCeh.png");
    }

    private async void SaveOrdersByProductChart_Click(object? sender, RoutedEventArgs e)
    {
        var chart = this.FindControl<Control>("OrdersByProductChart");

        if (chart != null)
            await SaveChart(chart, "OrdersByProduct.png");
    }

    private async void SaveOrdersByMonthChart_Click(object? sender, RoutedEventArgs e)
    {
        var chart = this.FindControl<Control>("OrdersByMonthChart");

        if (chart != null)
            await SaveChart(chart, "OrdersByMonth.png");
    }

    private async void SavePriceDistributionChart_Click(object? sender, RoutedEventArgs e)
    {
        var chart = this.FindControl<Control>("PriceDistributionChart");

        if (chart != null)
            await SaveChart(chart, "PriceDistribution.png");
    }
}
