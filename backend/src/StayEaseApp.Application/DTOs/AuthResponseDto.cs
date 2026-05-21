using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.DTOs;
public class AuthResponseDto
{
    public required string Token { get; set; }
    public Guid UserID { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
}
