using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.DTOs;
public class PropertyResponseDto
{
    public Guid PropertyID { get; set; }
    public Guid UserID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public string Location { get; set; } = string.Empty;
    public int MaxGuests { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}