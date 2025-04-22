using System;
using System.Collections.Generic;

namespace WebStore.Entities;

public class Carrier
{
    public int CarrierId { get; set; }
    public string CarrierName { get; set; } = null!;
    public string? ContactUrl { get; set; }
    public string? ContactPhone { get; set; }

    // Navigation back to orders
    public ICollection<Order>? Orders { get; set; }
} 