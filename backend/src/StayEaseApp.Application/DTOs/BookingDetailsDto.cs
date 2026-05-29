using StayEaseApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.DTOs;
public class BookingDetailsDto
{
    public Guid BookingID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public Status BookingStatus { get; set; }
    public PropertyDetailsDto Property { get; set; } = null!;
    public OwnerDetailsDto Owner { get; set; } = null!;
}

public class PropertyDetailsDto
{
    public Guid PropertyID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public string? ImageUrl { get; set; }
}

public class OwnerDetailsDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
