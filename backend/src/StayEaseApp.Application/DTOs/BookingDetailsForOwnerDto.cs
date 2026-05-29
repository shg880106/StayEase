using StayEaseApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.DTOs;
public class BookingDetailsForOwnerDto
{
    public Guid BookingID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public Status BookingStatus { get; set; }
    public PropertyDetailsDto Property { get; set; } = null!;
    public GuestDetailsDto Guest { get; set; } = null!;
}

public class GuestDetailsDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
