using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Tipografia3.Models;

namespace Tipografia3.Services;

public class LoadJSONService<T> : ILoadInterface<T> where T : class
{
    private readonly IDBService<T> _service;

    public LoadJSONService(IDBService<T> service)
    {
        _service = service;
    }

    public async Task LoadListAsync(string path)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        using var fs = new FileStream(path, FileMode.Open);

        if (typeof(T) == typeof(TipografCeh))
        {
            var data = await JsonSerializer.DeserializeAsync<Dictionary<string, List<TipografCeh>>>(fs, options);
            if (data != null && data.ContainsKey("цеха"))
            {
                foreach (var item in data["цеха"])
                    await _service.AddAsync((T)(object)item);
            }
        }
        else if (typeof(T) == typeof(Product))
        {
            var data = await JsonSerializer.DeserializeAsync<Dictionary<string, List<Product>>>(fs, options);
            if (data != null && data.ContainsKey("продукция"))
            {
                foreach (var item in data["продукция"])
                    await _service.AddAsync((T)(object)item);
            }
        }
        else if (typeof(T) == typeof(Dogovor))
        {
            // Десериализуем договоры с полями заказчика из задания
            var data = await JsonSerializer.DeserializeAsync<Dictionary<string, List<DogovorImport>>>(fs, options);
            if (data != null && data.ContainsKey("договоры"))
            {
                var clientService = new ClientsPageService();
                int clientId = 1;

                // Получаем максимальный существующий IdClient чтобы не дублировать
                var existingClients = await clientService.GetAsync();
                if (existingClients.Count > 0)
                    clientId = existingClients.Max(c => c.IdClient) + 1;

                var dogovorService = new DogovorPageService();
                var existingDogovors = await dogovorService.GetAsync();
                int dogovorId = existingDogovors.Count > 0 ? existingDogovors.Max(d => d.IdDogovor) + 1 : 1;

                foreach (var item in data["договоры"])
                {
                    // Создаём клиента автоматически
                    var client = new Client
                    {
                        IdClient = clientId,
                        NameClient = item.НазваниеЗаказчика ?? item.NameClient ?? "Неизвестно",
                        Address = item.АдресЗаказчика ?? item.Address ?? ""
                    };
                    await clientService.AddAsync(client);

                    // Создаём договор со ссылкой на клиента
                    var dogovor = new Dogovor
                    {
                        IdDogovor = dogovorId,
                        IdClient = clientId,
                        NumberDogovor = item.НомерДоговора ?? item.NumberDogovor,
                        DateOforml = item.ДатаОформления ?? item.DateOforml ?? "",
                        DateDue = item.ДатаВыполнения ?? item.DateDue ?? ""
                    };
                    await _service.AddAsync((T)(object)dogovor);

                    clientId++;
                    dogovorId++;
                }
            }
        }
        else if (typeof(T) == typeof(Order))
        {
            var data = await JsonSerializer.DeserializeAsync<Dictionary<string, List<OrderImport>>>(fs, options);
            if (data != null && data.ContainsKey("заказы"))
            {
                var orderService = new OrderPageService();
                var existingOrders = await orderService.GetAsync();
                int orderId = existingOrders.Count > 0 ? existingOrders.Max(o => o.IdOrder) + 1 : 1;

                foreach (var item in data["заказы"])
                {
                    var order = new Order
                    {
                        IdOrder = orderId,
                        NumberDogovor = item.НомерДоговора ?? item.NumberDogovor,
                        IdProduct = item.КодПродукции ?? item.IdProduct,
                        Quantity = item.Количество ?? item.Quantity
                    };
                    await _service.AddAsync((T)(object)order);
                    orderId++;
                }
            }
        }
        else
        {
            var records = await JsonSerializer.DeserializeAsync<List<T>>(fs, options);
            if (records != null)
            {
                foreach (var record in records)
                    await _service.AddAsync(record);
            }
        }
    }

    public async Task UploadAsync(string path, List<T> list)
    {
        using var fs = new FileStream(path, FileMode.Create);
        await JsonSerializer.SerializeAsync(fs, list, new JsonSerializerOptions { WriteIndented = true });
    }
}

// Вспомогательные классы для импорта с русскими полями из задания
public class DogovorImport
{
    // Русские поля (формат из задания)
    public int? НомерДоговора { get; set; }
    public string? НазваниеЗаказчика { get; set; }
    public string? АдресЗаказчика { get; set; }
    public string? ДатаОформления { get; set; }
    public string? ДатаВыполнения { get; set; }

    // Английские поля (запасной вариант)
    public int NumberDogovor { get; set; }
    public string? NameClient { get; set; }
    public string? Address { get; set; }
    public string? DateOforml { get; set; }
    public string? DateDue { get; set; }
}

public class OrderImport
{
    // Русские поля (формат из задания)
    public int? НомерДоговора { get; set; }
    public int? КодПродукции { get; set; }
    public int? Количество { get; set; }

    // Английские поля (запасной вариант)
    public int NumberDogovor { get; set; }
    public int IdProduct { get; set; }
    public int Quantity { get; set; }
}
