using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.DTOs;
public class CreateBookingRequestDto
{
    [Required(ErrorMessage = "Property ID is required")]
    public Guid PropertyID { get; set; }
    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }
    [Required(ErrorMessage = "End date is required")]
    public DateTime EndDate { get; set; }
}
