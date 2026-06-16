using System;
using System.Collections.Generic;

namespace Tipografia3.Models;

public partial class TipografCeh
{
    public int IdTipograf { get; set; }

    public int NumberCeh { get; set; }

    public string NameCeh { get; set; } = null!;

    public string BossCeh { get; set; } = null!;

    public string PhoneCeh { get; set; } = null!;
}
