using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Tipografia3.ViewModels;

namespace Tipografia3.Views;

public partial class ChartsPageView : UserControl
{
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
}