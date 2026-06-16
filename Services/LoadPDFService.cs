using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Tipografia3.Models;

namespace Tipografia3.Services;

public class LoadPDFService<T> : ILoadInterface<T> where T : class
{
    private readonly IDBService<T> _service;

    public LoadPDFService(IDBService<T> service)
    {
        _service = service;
    }

    public async Task LoadListAsync(string path)
    {
        throw new NotImplementedException("Загрузка из PDF не поддерживается");
    }

    public async Task UploadAsync(string path, List<T> list)
    {
        await Task.Run(() =>
        {
            using var pdfDoc = new PdfDocument(new PdfWriter(path));
            using var doc = new Document(pdfDoc);

            // Поиск шрифта Arial
            var fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
            if (!File.Exists(fontPath))
                fontPath = @"C:\Windows\Fonts\arial.ttf";

            if (!File.Exists(fontPath))
            {
                doc.Add(new Paragraph("Отчёт"));
            }
            else
            {
                PdfFont cyrillicFont = PdfFontFactory.CreateFont(fontPath, "Identity-H");
                doc.SetFont(cyrillicFont);
            }

            var title = new Paragraph(new Text($"{typeof(T).Name} - Отчёт").SetFontSize(20));
            title.SetTextAlignment(TextAlignment.CENTER);
            doc.Add(title);
            doc.Add(new Paragraph(" "));

            var properties = typeof(T).GetProperties();
            var table = new Table(UnitValue.CreatePercentArray(properties.Length));
            table.SetWidth(UnitValue.CreatePercentValue(100));
            table.SetMarginTop(10);

            // Заголовки
            foreach (var prop in properties)
            {
                var cell = new Cell()
                    .Add(new Paragraph(prop.Name))
                    .SetBackgroundColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY);
                table.AddHeaderCell(cell);
            }

            // Данные
            foreach (var item in list)
            {
                foreach (var prop in properties)
                {
                    var value = prop.GetValue(item)?.ToString() ?? "";
                    table.AddCell(new Cell().Add(new Paragraph(value)));
                }
            }

            doc.Add(table);
        });
    }
}