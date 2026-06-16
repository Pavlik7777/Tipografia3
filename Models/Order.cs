using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tipografia3.Models;

public partial class Order
{
    public int IdOrder { get; set; }
    public int NumberDogovor { get; set; }
    public int IdProduct { get; set; }
    public int Quantity { get; set; }

    [NotMapped]
    public string? DogovorInfo { get; set; }  // Номер договора + клиент

    [NotMapped]
    public string? ProductName { get; set; }  // Название продукции
}