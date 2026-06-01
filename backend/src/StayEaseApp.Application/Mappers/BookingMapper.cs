using Riok.Mapperly.Abstractions;
using StayEaseApp.Application.DTOs;
using StayEaseApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.Mappers;
[Mapper]
public partial class BookingMapper
{
    [MapperIgnoreSource(nameof(Booking.Property))]
    [MapperIgnoreSource(nameof(Booking.User))]
    public partial BookingResponseDto BookingToBookingResponseDto(Booking booking);

    private ReviewSummaryDto? MapReview(Review? review) =>
        review is null ? null : new ReviewSummaryDto { Rating = review.Rating, Comment = review.Comment };
}
