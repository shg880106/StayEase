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
public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _dbContext;

    public ReviewRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Review> CreateReviewAsync(Review review)
    {
        _dbContext.Reviews.Add(review);
        await _dbContext.SaveChangesAsync();
        return review;
    }

    public Task DeleteReviewAsync(Guid reviewId)
    {
        throw new NotImplementedException();
    }

    public async Task<Review?> GetByBookingIdAsync(Guid bookingId)
    {
        return await _dbContext.Reviews
            .FirstOrDefaultAsync(r => r.BookingID == bookingId);
    }

    public async Task<Review?> GetByIdAsync(Guid reviewId)
    {
        return await _dbContext.Reviews
            .Include(r => r.Property)
            .Include(r => r.Booking)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.ReviewID == reviewId);
    }

    public async Task<Review> UpdateReviewAsync(Guid reviewId, Review reviewRequest)
    {
        _dbContext.Reviews.Update(reviewRequest);
        await _dbContext.SaveChangesAsync();
        return reviewRequest;
    }
}
