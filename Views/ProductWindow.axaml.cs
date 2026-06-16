using Avalonia.Controls;
using Avalonia.Interactivity;
using Tipografia3.Models;

namespace Tipografia3.Views;

public partial class ProductWindow : Window
{
    public Product Product { get; private set; }

    public ProductWindow(Product product)
    {
        InitializeComponent();
        Product = product;
        DataContext = Product;
    }

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        Close(Product);
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}