using StayEaseApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.Interfaces;
public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid reviewId);
    Task<Review?> GetByBookingIdAsync(Guid bookingId); // Check if booking already has a review
    Task<Review> CreateReviewAsync(Review review);
    Task DeleteReviewAsync(Guid reviewId);
    Task<Review> UpdateReviewAsync(Guid reviewId, Review reviewRequest);
}
