using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Tipografia3.Models;

namespace Tipografia3.Services;

public class LoadXMLService<T> : ILoadInterface<T> where T : class
{
    private readonly IDBService<T> _service;

    public LoadXMLService(IDBService<T> service)
    {
        _service = service;
    }

    public async Task LoadListAsync(string path)
    {
        await Task.Run(() =>
        {
            using var fs = new FileStream(path, FileMode.Open);

            if (typeof(T) == typeof(TipografCeh))
            {
                var serializer = new XmlSerializer(typeof(List<TipografCeh>));
                var records = serializer.Deserialize(fs) as List<TipografCeh>;
                if (records != null)
                    foreach (var item in records)
                        _service.AddAsync((T)(object)item).Wait();
            }
            else if (typeof(T) == typeof(Product))
            {
                var serializer = new XmlSerializer(typeof(List<Product>));
                var records = serializer.Deserialize(fs) as List<Product>;
                if (records != null)
                    foreach (var item in records)
                        _service.AddAsync((T)(object)item).Wait();
            }
            else if (typeof(T) == typeof(Client))
            {
                var serializer = new XmlSerializer(typeof(List<Client>));
                var records = serializer.Deserialize(fs) as List<Client>;
                if (records != null)
                    foreach (var item in records)
                        _service.AddAsync((T)(object)item).Wait();
            }
            else if (typeof(T) == typeof(Dogovor))
            {
                var serializer = new XmlSerializer(typeof(List<Dogovor>));
                var records = serializer.Deserialize(fs) as List<Dogovor>;
                if (records != null)
                    foreach (var item in records)
                        _service.AddAsync((T)(object)item).Wait();
            }
            else if (typeof(T) == typeof(Order))
            {
                var serializer = new XmlSerializer(typeof(List<Order>));
                var records = serializer.Deserialize(fs) as List<Order>;
                if (records != null)
                    foreach (var item in records)
                        _service.AddAsync((T)(object)item).Wait();
            }
            else
            {
                var serializer = new XmlSerializer(typeof(List<T>));
                var records = serializer.Deserialize(fs) as List<T>;
                if (records != null)
                    foreach (var record in records)
                        _service.AddAsync(record).Wait();
            }
        });
    }

    public async Task UploadAsync(string path, List<T> list)
    {
        await Task.Run(() =>
        {
            var serializer = new XmlSerializer(typeof(List<T>));
            using var fs = new FileStream(path, FileMode.Create);
            serializer.Serialize(fs, list);
        });
    }
}
