using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using Tipografia3.Models;

namespace Tipografia3.Services;

public class PdfReportService
{
    private static readonly DeviceRgb HeaderBg = new DeviceRgb(46, 117, 182);
    private static readonly DeviceRgb RowEven = new DeviceRgb(234, 244, 251);
    private static readonly DeviceRgb RowOdd = new DeviceRgb(255, 255, 255);
    private static readonly DeviceRgb TextWhite = new DeviceRgb(255, 255, 255);
    private static readonly DeviceRgb TextDark = new DeviceRgb(28, 40, 51);

    // Шрифты — стандартные встроенные, не требуют файлов
    private static PdfFont GetNormal() => PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
    private static PdfFont GetBold() => PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

    // ─── Отчёт 1: Сводка по цехам ──────────────────────────────────────────
    public void SaveCehReport(string path, IEnumerable<CehReport> data)
    {
        using var writer = new PdfWriter(path);
        using var pdf = new PdfDocument(writer);
        using var doc = new Document(pdf, iText.Kernel.Geom.PageSize.A4.Rotate());

        AddTitle(doc, "Сводка по цехам");
        AddDate(doc);

        var table = new Table(new float[] { 3, 3, 2, 1, 2 })
            .SetWidth(UnitValue.CreatePercentValue(100));
        AddHeader(table, "Название", "Начальник", "Телефон", "Видов продукции", "Сумма заказов (руб.)");

        int row = 0;
        foreach (var r in data)
        {
            var bg = row++ % 2 == 0 ? RowEven : RowOdd;
            AddRow(table, bg, r.NameCeh, r.BossCeh, r.PhoneCeh,
                r.ProductCount.ToString(), $"{r.TotalSum:N2}");
        }

        doc.Add(table);
        AddFooter(doc);
    }

    // ─── Отчёт 2: Сводка по продукции ──────────────────────────────────────
    public void SaveProductReport(string path, IEnumerable<ProductReport> data)
    {
        using var writer = new PdfWriter(path);
        using var pdf = new PdfDocument(writer);
        using var doc = new Document(pdf, iText.Kernel.Geom.PageSize.A4.Rotate());

        AddTitle(doc, "Сводка по продукции");
        AddDate(doc);

        var table = new Table(new float[] { 3, 2, 2, 1, 1, 2 })
            .SetWidth(UnitValue.CreatePercentValue(100));
        AddHeader(table, "Название", "Цех", "Стоимость единицы (руб.)", "Заказов", "Количество", "Выручка (руб.)");

        int row = 0;
        foreach (var r in data)
        {
            var bg = row++ % 2 == 0 ? RowEven : RowOdd;
            AddRow(table, bg, r.NameProduct, r.CehName,
                $"{r.Price:N2}", r.OrderCount.ToString(),
                r.TotalQuantity.ToString(), $"{r.Revenue:N2}");
        }

        doc.Add(table);
        AddFooter(doc);
    }

    // ─── Отчёт 3: Невостребованная продукция ───────────────────────────────
    public void SaveUnsoldReport(string path, IEnumerable<Product> data)
    {
        using var writer = new PdfWriter(path);
        using var pdf = new PdfDocument(writer);
        using var doc = new Document(pdf, iText.Kernel.Geom.PageSize.A4);

        AddTitle(doc, "Невостребованная продукция");
        AddDate(doc);

        var list = data.ToList();
        if (!list.Any())
        {
            doc.Add(new Paragraph("Вся продукция востребована.")
                .SetFont(GetNormal()).SetFontSize(12).SetMarginTop(20));
            AddFooter(doc);
            return;
        }

        var table = new Table(new float[] { 1, 4, 2, 2 })
            .SetWidth(UnitValue.CreatePercentValue(100));
        AddHeader(table, "Код", "Название", "Номер цеха", "Стоимость единицы (руб.)");

        int row = 0;
        foreach (var p in list)
        {
            var bg = row++ % 2 == 0 ? RowEven : RowOdd;
            AddRow(table, bg, p.IdProduct.ToString(),
                p.NameProduct ?? "", p.NumberCeh.ToString(), $"{p.Price1sh:N2}");
        }

        doc.Add(table);
        AddFooter(doc);
    }

    // ─── Отчёт 4: Сводка по договорам ──────────────────────────────────────
    public void SaveDogovorReport(string path, IEnumerable<DogovorReport> data)
    {
        using var writer = new PdfWriter(path);
        using var pdf = new PdfDocument(writer);
        using var doc = new Document(pdf, iText.Kernel.Geom.PageSize.A4.Rotate());

        AddTitle(doc, "Сводка по договорам");
        AddDate(doc);

        var table = new Table(new float[] { 1, 3, 2, 2, 2, 2 })
            .SetWidth(UnitValue.CreatePercentValue(100));
        AddHeader(table, "№ договора", "Заказчик", "Дата оформления", "Дата выполнения", "Сумма (руб.)", "Статус");

        int row = 0;
        foreach (var r in data)
        {
            var bg = row++ % 2 == 0 ? RowEven : RowOdd;
            AddRow(table, bg,
                r.NumberDogovor.ToString(), r.ClientName,
                r.DateOforml, r.DateDue, $"{r.TotalSum:N2}");

            // Статус — отдельная ячейка с цветом текста
            var statusColor = r.Status == "Выполнен"
                ? new DeviceRgb(39, 174, 96)
                : new DeviceRgb(192, 57, 43);
            table.AddCell(new Cell()
                .SetBackgroundColor(bg)
                .SetPadding(5)
                .Add(new Paragraph(r.Status)
                    .SetFont(GetBold())
                    .SetFontSize(10)
                    .SetFontColor(statusColor)));
        }

        doc.Add(table);
        AddFooter(doc);
    }

    // ─── Отчёт 5: Топ-5 продукции ──────────────────────────────────────────
    public void SaveTopProductReport(string path, IEnumerable<TopProductReport> data)
    {
        using var writer = new PdfWriter(path);
        using var pdf = new PdfDocument(writer);
        using var doc = new Document(pdf, iText.Kernel.Geom.PageSize.A4);

        AddTitle(doc, "Топ-5 продукции по выручке");
        AddDate(doc);

        var table = new Table(new float[] { 4, 3, 2 })
            .SetWidth(UnitValue.CreatePercentValue(100));
        AddHeader(table, "Название", "Выручка (руб.)", "Доля (%)");

        int row = 0;
        foreach (var r in data)
        {
            var bg = row++ % 2 == 0 ? RowEven : RowOdd;
            AddRow(table, bg, r.NameProduct, $"{r.Revenue:N2}", $"{r.Share:F2}%");
        }

        doc.Add(table);
        AddFooter(doc);
    }

    // ─── Вспомогательные методы ─────────────────────────────────────────────
    private static void AddTitle(Document doc, string title)
    {
        doc.Add(new Paragraph(title)
            .SetFont(GetBold())
            .SetFontSize(18)
            .SetFontColor(new DeviceRgb(31, 56, 100))
            .SetMarginBottom(4));
    }

    private static void AddDate(Document doc)
    {
        doc.Add(new Paragraph($"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}")
            .SetFont(GetNormal())
            .SetFontSize(9)
            .SetFontColor(new DeviceRgb(127, 140, 141))
            .SetMarginBottom(12));
    }

    private static void AddFooter(Document doc)
    {
        doc.Add(new Paragraph($"Tipografia3 - {DateTime.Now.Year}")
            .SetFont(GetNormal())
            .SetFontSize(8)
            .SetFontColor(new DeviceRgb(127, 140, 141))
            .SetMarginTop(16)
            .SetTextAlignment(TextAlignment.CENTER));
    }

    private static void AddHeader(Table table, params string[] headers)
    {
        var boldFont = GetBold();
        foreach (var h in headers)
        {
            table.AddHeaderCell(new Cell()
                .SetBackgroundColor(HeaderBg)
                .SetPadding(6)
                .Add(new Paragraph(h)
                    .SetFont(boldFont)
                    .SetFontColor(TextWhite)
                    .SetFontSize(10)));
        }
    }

    private static void AddRow(Table table, DeviceRgb bg, params string[] cells)
    {
        var normalFont = GetNormal();
        foreach (var c in cells)
        {
            table.AddCell(new Cell()
                .SetBackgroundColor(bg)
                .SetPadding(5)
                .Add(new Paragraph(c ?? "")
                    .SetFont(normalFont)
                    .SetFontColor(TextDark)
                    .SetFontSize(10)));
        }
    }
}