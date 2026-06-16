using Avalonia;
using System;
using System.Linq;
using Tipografia3.Models;

namespace Tipografia3;
sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        InitializeDatabase();

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    private static void InitializeDatabase()
    {
        try
        {
            using var db = new TipografiaContext();

            if (!db.Clients.Any(c => c.IdClient == 7))
            {
                db.Clients.AddRange(
                    new Client { IdClient = 7, NameClient = "ООО Издательство Мир", Address = "г. Челябинск, ул. Южная 7" },
                    new Client { IdClient = 8, NameClient = "АО Типография №1", Address = "г. Самара, ул. Заводская 15" },
                    new Client { IdClient = 9, NameClient = "ООО ПромоПринт", Address = "г. Ростов-на-Дону, ул. Донская 9" }
                );
                db.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DB Init error: {ex.Message}");
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}