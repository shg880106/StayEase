using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.DTOs;
public class BookingResponseDto
{
    public Guid BookingID { get; set; }
    public  decimal TotalPrice { get; set; }
}