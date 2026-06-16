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
            var formatter = new XmlSerializer(typeof(List<T>));
            using var fs = new FileStream(path, FileMode.Open);
            var records = formatter.Deserialize(fs) as List<T>;
            if (records != null)
            {
                foreach (var record in records)
                {
                    _service.AddAsync(record).Wait();
                }
            }
        });
    }

    public async Task UploadAsync(string path, List<T> list)
    {
        await Task.Run(() =>
        {
            var formatter = new XmlSerializer(typeof(List<T>));
            using var fs = new FileStream(path, FileMode.Create);
            formatter.Serialize(fs, list);
        });
    }
}