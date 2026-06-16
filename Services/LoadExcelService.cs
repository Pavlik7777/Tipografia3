using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tipografia3.Models;

namespace Tipografia3.Services;

public class LoadExcelService<T> : ILoadInterface<T> where T : class
{
    private readonly IDBService<T> _service;

    public LoadExcelService(IDBService<T> service)
    {
        _service = service;
        ExcelPackage.License.SetNonCommercialPersonal("KBK");
    }

    public async Task LoadListAsync(string path)
    {
        await Task.Run(async () =>
        {
            var fileInfo = new FileInfo(path);
            using var package = new ExcelPackage(fileInfo);
            var worksheet = package.Workbook.Worksheets["Заказы"] ?? package.Workbook.Worksheets[0];

            if (typeof(T) == typeof(Order))
            {
                await LoadOrders(worksheet);
            }
            else
            {
                // Общая загрузка по заголовкам
                await LoadGeneric(worksheet);
            }
        });
    }

    private async Task LoadOrders(ExcelWorksheet ws)
    {
        int rowCount = ws.Dimension.Rows;
        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                var order = new Order
                {
                    IdOrder = int.Parse(ws.Cells[row, 1].Text),
                    NumberDogovor = int.Parse(ws.Cells[row, 2].Text),
                    IdProduct = int.Parse(ws.Cells[row, 3].Text),
                    Quantity = int.Parse(ws.Cells[row, 4].Text)
                };
                await _service.AddAsync((T)(object)order);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка строки {row}: {ex.Message}");
            }
        }
    }

    private async Task LoadGeneric(ExcelWorksheet ws)
    {
        // Реализация для других типов при необходимости
        throw new NotImplementedException("Загрузка данного типа из Excel не реализована");
    }

    public async Task UploadAsync(string path, List<T> list)
    {
        await Task.Run(() =>
        {
            var fileInfo = new FileInfo(path);
            using var package = new ExcelPackage(fileInfo);
            var worksheet = package.Workbook.Worksheets.Add(typeof(T).Name);

            var properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
                worksheet.Cells[1, i + 1].Value = properties[i].Name;

            for (int row = 0; row < list.Count; row++)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    var value = properties[col].GetValue(list[row]);
                    worksheet.Cells[row + 2, col + 1].Value = value;
                }
            }

            worksheet.Cells.AutoFitColumns();
            package.Save();
        });
    }
}