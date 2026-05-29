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
    Task<List<Booking>> GetByUserIdAsync(Guid userId);
    Task<Booking?> GetByIdAsync(Guid bookingId);
    Task<Booking?> GetByIdWithReviewAsync(Guid bookingId); 
    Task AddAsync(Booking booking);
    Task UpdateAsync(Booking booking);
}
