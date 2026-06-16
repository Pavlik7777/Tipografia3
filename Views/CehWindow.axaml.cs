using Avalonia.Controls;
using Avalonia.Interactivity;
using Tipografia3.Models;

namespace Tipografia3.Views;

public partial class CehWindow : Window
{
    public TipografCeh Ceh { get; private set; }

    public CehWindow(TipografCeh ceh)
    {
        InitializeComponent();
        Ceh = ceh;
        DataContext = Ceh;
    }

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        Close(Ceh);
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
    private void Add_Click(object? sender, RoutedEventArgs e)
    {
        Ceh = new TipografCeh();
        DataContext = Ceh;
    }
}