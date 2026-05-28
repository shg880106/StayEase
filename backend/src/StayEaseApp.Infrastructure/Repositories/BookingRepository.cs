using Microsoft.EntityFrameworkCore;
using StayEaseApp.Application.Interfaces;
using StayEaseApp.Domain.Entities;
using StayEaseApp.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Infrastructure.Repositories;
public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _dbContext;

    public BookingRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Booking booking)
    {
        await _dbContext.Bookings.AddAsync(booking);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Booking?> GetByIdAsync(Guid bookingId)
    {
        return await _dbContext.Bookings
            .Include(b => b.Property)
                .ThenInclude(p => p.Owner)
            .FirstOrDefaultAsync(b => b.BookingID == bookingId);
    }

    public async Task<List<Booking>> GetByPropertyIdAsync(Guid propertyId)
    {
        return await _dbContext.Bookings
            .Where(b => b.PropertyID == propertyId)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetByUserIdAsync(Guid userId)
    {
        return await _dbContext.Bookings
            .Include(b => b.Property)
            .Where(b => b.UserID == userId)
            .OrderByDescending(b => b.StartDate)
            .ToListAsync();
    }

    public async Task UpdateAsync(Booking booking)
    {
        _dbContext.Bookings.Update(booking);
        await _dbContext.SaveChangesAsync();
    }
}
