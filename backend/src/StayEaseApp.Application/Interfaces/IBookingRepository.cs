using StayEaseApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.Interfaces;
public interface IBookingRepository
{
    Task<List<Booking>> GetByPropertyIdAsync(Guid propertyId);
    Task AddAsync(Booking booking);
}
