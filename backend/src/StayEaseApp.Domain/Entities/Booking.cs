using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StayEaseApp.Domain.Entities;
using StayEaseApp.Domain.Enums;

namespace StayEaseApp.Domain.Entities;
public class Booking
{
    private Status bookingStatus = Status.Pending;

    public Guid BookingID { get; set; }
    public Guid PropertyID { get; set; }
    public Guid UserID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public Status BookingStatus { get; set; } = Status.Pending;    

    // Navigation properties
    public User User { get; set; } = null!;
    public Property Property { get; set; } = null!;
    public Review? Review { get; set; } // 1:1

    // Computed property
    public bool CanBeReviewed =>
        BookingStatus == Status.Finished
        && Review == null
        && EndDate < DateTime.UtcNow;

    private Booking() { }

    public Booking(Guid propertyId, Guid userId, DateTime startDate, DateTime endDate, decimal pricePerNight)
    {
        if (startDate >= endDate)
            throw new ArgumentException("Invalid date range");

        PropertyID = propertyId;
        UserID = userId;
        StartDate = startDate;
        EndDate = endDate;

        var nights = (endDate - startDate).Days;
        TotalPrice = nights * pricePerNight;

        BookingID = Guid.NewGuid();
    }

    public bool Overlaps(DateTime startDate, DateTime endDate)
    {
        return StartDate < endDate && EndDate > startDate;
    }
    
    public bool IsFinishedAndPast()
    {
        return BookingStatus == Status.Finished && EndDate < DateTime.UtcNow;
    }
}
