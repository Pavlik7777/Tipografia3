using Avalonia.Animation.Easings;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tipografia3.Models;
using Tmds.DBus.Protocol;

namespace Tipografia3.Services;

public class LoadCSVService<T> : ILoadInterface<T> where T : class
{
    private readonly IDBService<T> _service;

    public LoadCSVService(IDBService<T> service)
    {
        _service = service;
    }

    public async Task LoadListAsync(string path)
    {
        path = Uri.UnescapeDataString(path);
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        if (typeof(T) == typeof(TipografCeh))
            await LoadCehCSV(csv);
        else if (typeof(T) == typeof(Product))
            await LoadProductCSV(csv);
        else if (typeof(T) == typeof(Client))
            await LoadClientCSV(csv);
        else if (typeof(T) == typeof(Dogovor))
            await LoadDogovorCSV(csv);
        else if (typeof(T) == typeof(Order))
            await LoadOrderCSV(csv);
        else
            await LoadStandardCSV(csv);
    }

    private async Task LoadCehCSV(CsvReader csv)
    {
        csv.Read();
        csv.ReadHeader();

        while (csv.Read())
        {
            try
            {
                var ceh = new TipografCeh
                {
                    IdTipograf = csv.GetField<int>("Id_Tipograf"),
                    NumberCeh = csv.GetField<int>("NumberCeh"),
                    NameCeh = csv.GetField("NameCeh"),
                    BossCeh = csv.GetField("BossCeh"),
                    PhoneCeh = csv.GetField("PhoneCeh")
                };
                await _service.AddAsync((T)(object)ceh);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка импорта цеха: {ex.Message}");
            }
        }
    }
    private async Task LoadProductCSV(CsvReader csv)
    {
        csv.Read();
        csv.ReadHeader();

        while (csv.Read())
        {
            try
            {
                var product = new Product
                {
                    IdProduct = csv.GetField<int>("Id_Product"),
                    NameProduct = csv.GetField("NameProduct"),
                    NumberCeh = csv.GetField<int>("NumberCeh"),
                    Price1sh = csv.GetField<double>("Price_1sh")
                };
                await _service.AddAsync((T)(object)product);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка импорта продукции: {ex.Message}");
            }
        }
    }

    private async Task LoadClientCSV(CsvReader csv)
    {
        csv.Read();
        csv.ReadHeader();

        while (csv.Read())
        {
            try
            {
                var client = new Client
                {
                    IdClient = csv.GetField<int>("Id_Client"),
                    NameClient = csv.GetField("NameClient"),
                    Address = csv.GetField("Address")
                };
                await _service.AddAsync((T)(object)client);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка импорта клиента: {ex.Message}");
            }
        }
    }

    private async Task LoadDogovorCSV(CsvReader csv)
    {
        csv.Read();
        csv.ReadHeader();

        while (csv.Read())
        {
            try
            {
                var dogovor = new Dogovor
                {
                    IdDogovor = csv.GetField<int>("Id_Dogovor"),
                    IdClient = csv.GetField<int>("Id_Client"),
                    NumberDogovor = csv.GetField<int>("NumberDogovor"),
                    DateOforml = csv.GetField("Date_Oforml"),
                    DateDue = csv.GetField("Date_Due")
                };
                await _service.AddAsync((T)(object)dogovor);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка импорта договора: {ex.Message}");
            }
        }
    }

    private async Task LoadOrderCSV(CsvReader csv)
    {
        csv.Read();
        csv.ReadHeader();

        while (csv.Read())
        {
            try
            {
                var order = new Order
                {
                    IdOrder = csv.GetField<int>("Id_Order"),
                    NumberDogovor = csv.GetField<int>("NumberDogovor"),
                    IdProduct = csv.GetField<int>("Id_Product"),
                    Quantity = csv.GetField<int>("Quantity")
                };
                await _service.AddAsync((T)(object)order);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка импорта заказа: {ex.Message}");
            }
        }
    }

    private async Task LoadStandardCSV(CsvReader csv)
    {
        var records = csv.GetRecords<T>();
        foreach (var record in records)
        {
            await _service.AddAsync(record);
        }
    }

    public async Task UploadAsync(string path, List<T> list)
    {
        using var writer = new StreamWriter(path);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(list);
    }
}