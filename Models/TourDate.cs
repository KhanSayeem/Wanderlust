using System;
using System.Collections.Generic;

namespace TourismApp.Models;

public partial class TourDate
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public int Capacity { get; set; }

    public int TourPackageId { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual TourPackage TourPackage { get; set; } = null!;
}
