using System;

namespace TourismApp.Models;

public partial class Booking
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public int TourDateId { get; set; }

    public int Participants { get; set; }

    public BookingStatus Status { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Feedback? Feedback { get; set; }

    public virtual TourDate TourDate { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;
}
