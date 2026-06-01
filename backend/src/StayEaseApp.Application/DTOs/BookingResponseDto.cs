using StayEaseApp.Domain.Entities;
using StayEaseApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.DTOs;
public class BookingResponseDto
{
    public Guid BookingID { get; set; }
    public Guid PropertyID { get; set; }
    public Guid UserID { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public Status BookingStatus { get; set; }
    public bool CanBeReviewed { get; set; }
    public ReviewSummaryDto? Review { get; set; }
}

public class ReviewSummaryDto
{
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}