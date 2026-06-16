using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using Tipografia3.Models;

namespace Tipografia3.Views;
public partial class OrderWindow : Window
{
    public Order Order { get; private set; }
    private readonly List<Dogovor> _dogovors;
    private readonly List<Product> _products;
    public OrderWindow(Order order, List<Dogovor> dogovors, List<Product> products)
    {
        InitializeComponent();
        Order = order;
        _dogovors = dogovors;
        _products = products;
        DataContext = this;
        DogovorComboBox.ItemsSource = _dogovors;
        ProductComboBox.ItemsSource = _products;
        ProductComboBox.SelectionChanged += (s, e) => CalculateSum();
        QuantityNumeric.ValueChanged += (s, e) => CalculateSum();
    }

    private void CalculateSum()
    {
        if (ProductComboBox.SelectedItem is Product product && QuantityNumeric.Value is decimal qty)
        {
            var sum = product.Price1sh * (int)qty;
            SumTextBlock.Text = $"Сумма: {sum:C}";
        }
    }

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        if (DogovorComboBox.SelectedItem is Dogovor dogovor)
            Order.NumberDogovor = dogovor.NumberDogovor;
        if (ProductComboBox.SelectedItem is Product product)
            Order.IdProduct = product.IdProduct;
        Order.Quantity = (int)(QuantityNumeric.Value ?? 1);

        Close(Order);
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}