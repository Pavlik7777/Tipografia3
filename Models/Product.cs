using System;
using System.Collections.Generic;

namespace Tipografia3.Models;

public partial class Product
{
    public int IdProduct { get; set; }
    public string? NameProduct { get; set; }
    public int NumberCeh { get; set; }
    public double Price1sh { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string? NameCeh { get; set; }

}