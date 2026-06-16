using System;
using System.Collections.Generic;

namespace Tipografia3.Models;

public partial class Dogovor
{
    public int IdDogovor { get; set; }

    public int IdClient { get; set; }

    public int NumberDogovor { get; set; }

    public string DateOforml { get; set; } = null!;

    public string? DateDue { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string? ClientName { get; set; }
}
