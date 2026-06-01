using StayEaseApp.Application.DTOs;
using StayEaseApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.Interfaces;
public interface IBookingService
{
    Task<BookingResponseDto> CreateBookingAsync(Guid propertyId, Guid userId, DateTime startDate, DateTime endDate);
    Task<List<BookingResponseDto>> GetUserBookingsAsync(Guid userId);
    Task<BookingDetailsDto?> GetBookingDetailsAsync(Guid bookingId, Guid userId);
    Task<BookingDetailsForOwnerDto?> GetBookingDetailsForOwnerAsync(Guid bookingId, Guid ownerId);
    Task<BookingResponseDto> CancelBookingAsync(Guid bookingId, Guid userId);
    Task<BookingResponseDto> ConfirmBookingAsync(Guid bookingId, Guid ownerId);
    Task<BookingResponseDto> FinishBookingAsync(Guid bookingId, Guid ownerId);
    Task<List<BookingResponseDto>> GetPropertyBookingsAsync(Guid propertyId, Guid ownerId);
}
