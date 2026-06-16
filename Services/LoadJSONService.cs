using System;
using System.Collections.Generic;
using System.IO;
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
        using var fs = new FileStream(path, FileMode.Open);

        if (typeof(T) == typeof(TipografCeh))
        {
            var data = await JsonSerializer.DeserializeAsync<Dictionary<string, List<TipografCeh>>>(fs);
            if (data != null && data.ContainsKey("цеха"))
            {
                foreach (var item in data["цеха"])
                {
                    await _service.AddAsync((T)(object)item);
                }
            }
        }
        else if (typeof(T) == typeof(Product))
        {
            var data = await JsonSerializer.DeserializeAsync<Dictionary<string, List<Product>>>(fs);
            if (data != null && data.ContainsKey("продукция"))
            {
                foreach (var item in data["продукция"])
                {
                    await _service.AddAsync((T)(object)item);
                }
            }
        }
        else if (typeof(T) == typeof(Client))
        {
            var data = await JsonSerializer.DeserializeAsync<Dictionary<string, List<Client>>>(fs);
            if (data != null && data.ContainsKey("клиенты"))
            {
                foreach (var item in data["клиенты"])
                {
                    await _service.AddAsync((T)(object)item);
                }
            }
        }
        else if (typeof(T) == typeof(Dogovor))
        {
            var data = await JsonSerializer.DeserializeAsync<Dictionary<string, List<Dogovor>>>(fs);
            if (data != null && data.ContainsKey("договоры"))
            {
                foreach (var item in data["договоры"])
                {
                    await _service.AddAsync((T)(object)item);
                }
            }
        }
        else if (typeof(T) == typeof(Order))
        {
            var data = await JsonSerializer.DeserializeAsync<Dictionary<string, List<Order>>>(fs);
            if (data != null && data.ContainsKey("заказы"))
            {
                foreach (var item in data["заказы"])
                {
                    await _service.AddAsync((T)(object)item);
                }
            }
        }
        else
        {
            var records = await JsonSerializer.DeserializeAsync<List<T>>(fs);
            if (records != null)
            {
                foreach (var record in records)
                {
                    await _service.AddAsync(record);
                }
            }
        }
    }

    public async Task UploadAsync(string path, List<T> list)
    {
        using var fs = new FileStream(path, FileMode.Create);
        await JsonSerializer.SerializeAsync(fs, list, new JsonSerializerOptions { WriteIndented = true });
    }
}