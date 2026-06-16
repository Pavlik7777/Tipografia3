using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using Tipografia3.Models;

namespace Tipografia3.Views;
public partial class DogovorWindow : Window
{
    public Dogovor Dogovor { get; private set; }
    private readonly List<Client> _clients;
    public List<Client> Clients => _clients;

    public DogovorWindow(Dogovor dogovor, List<Client> clients)
    {
        InitializeComponent();
        Dogovor = dogovor;
        _clients = clients;
        DataContext = this;
        ClientComboBox.ItemsSource = _clients;
        if (DateTime.TryParse(Dogovor.DateOforml, out var dateOforml))
            DateOformlPicker.SelectedDate = dateOforml;
        if (DateTime.TryParse(Dogovor.DateDue, out var dateDue))
            DateDuePicker.SelectedDate = dateDue;
    }

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        if (ClientComboBox.SelectedItem is Client client)
            Dogovor.IdClient = client.IdClient;
        Dogovor.NumberDogovor = Dogovor.NumberDogovor;
        Dogovor.DateOforml = DateOformlPicker.SelectedDate?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd");
        Dogovor.DateDue = DateDuePicker.SelectedDate?.ToString("yyyy-MM-dd") ?? DateTime.Now.AddDays(30).ToString("yyyy-MM-dd");

        Close(Dogovor);
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}