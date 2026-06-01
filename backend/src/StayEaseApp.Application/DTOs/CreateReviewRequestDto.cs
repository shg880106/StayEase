using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.DTOs;
public class CreateReviewRequestDto
{
    public Guid UserID { get; set; }
    public Guid PropertyID { get; set; }
    public Guid BookingID { get; set; }
    public int Rating { get; set; } // 1 to 5
    public string Comment { get; set; } = string.Empty;

}
