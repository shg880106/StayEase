using StayEaseApp.Application.DTOs;
using StayEaseApp.Application.Interfaces;
using StayEaseApp.Application.Mappers;
using StayEaseApp.Domain.Entities;
using StayEaseApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.Services;
public class ReviewService : IReviewService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly ReviewMapper _mapper = new();

    private const int MaxDaysAfterCheckout = 30;

    public ReviewService(IBookingRepository bookingRepository, IReviewRepository reviewRepository)
    {
        _bookingRepository = bookingRepository;
        _reviewRepository = reviewRepository;
    }

    public async Task<ReviewResponseDto> CreateReviewAsync(CreateReviewRequestDto reviewRequest)
    {
        // Verify that the booking exists
        var booking = await _bookingRepository.GetByIdWithReviewAsync(reviewRequest.BookingID);
        if (booking == null)
        {
            throw new InvalidOperationException($"Booking with ID {reviewRequest.BookingID} not found.");
        }

        // Verify that the user creating the review is the owner of the booking
        if (booking.UserID != reviewRequest.UserID)
        {
            throw new UnauthorizedAccessException("You can only review bookings that you made.");
        }

        // Verify that the booking is in Finished status
        if (booking.BookingStatus != Status.Finished)
        {
            throw new InvalidOperationException("You can only review bookings that have been finished.");
        }

        // Verify that the end date has passed (cannot review before the stay ends)
        if (booking.EndDate >= DateTime.UtcNow)
        {
            throw new InvalidOperationException("You cannot review a booking that hasn't ended yet.");
        }

        // Verify time window (30 days after checkout)
        var daysSinceCheckout = (DateTime.UtcNow - booking.EndDate).Days;
        if (daysSinceCheckout > MaxDaysAfterCheckout)
        {
            throw new InvalidOperationException($"The review period has expired. Reviews must be submitted within {MaxDaysAfterCheckout} days after checkout.");
        }

        // Verify that the booking does NOT have an existing review (1:1 relationship)
        if (booking.Review != null)
        {
            throw new InvalidOperationException("This booking has already been reviewed.");
        }

        // Alternative: verify by direct query
        var existingReview = await _reviewRepository.GetByBookingIdAsync(reviewRequest.BookingID);
        if (existingReview != null)
        {
            throw new InvalidOperationException("This booking has already been reviewed.");
        }

        // Create the review using the constructor with validations
        var newReview = new Review(
            booking.PropertyID,
            reviewRequest.UserID,
            reviewRequest.BookingID,
            reviewRequest.Rating,
            reviewRequest.Comment
        );

        var createdReview = await _reviewRepository.CreateReviewAsync(newReview);
        return _mapper.ReviewToReviewResponseDto(createdReview);
    }

    public async Task<ReviewResponseDto?> GetReviewByIdAsync(Guid reviewId)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);

        return review == null ? null : _mapper.ReviewToReviewResponseDto(review);
    }
}
