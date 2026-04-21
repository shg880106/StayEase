using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.DTOs;
public class CreateBookingRequestDto
{
    public Guid PropertyID { get; set; }
    public Guid UserID { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
