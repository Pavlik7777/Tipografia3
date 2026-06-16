using Avalonia.Controls;

namespace Tipografia3.Views;

public partial class MainWindow : Window
{
    public static MainWindow? Instance { get; private set; }

    public MainWindow()
    {
        InitializeComponent();
        Instance = this;
    }
}