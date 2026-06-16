using System;
using System.Collections.Generic;

namespace Tipografia3.Models;

public partial class Client
{
    public int IdClient { get; set; }

    public string NameClient { get; set; } = null!;

    public string Address { get; set; } = null!;
}
