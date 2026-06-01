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
public partial class PropertyMapper
{
    // Ignore unmapped source members to resolve RMG020 diagnostics
    [MapperIgnoreSource(nameof(Property.Owner))]
    [MapperIgnoreSource(nameof(Property.Bookings))]
    // Map nullable ImageUrl to non-nullable target with fallback to empty string to resolve RMG089
    [MapProperty(nameof(Property.ImageUrl), nameof(PropertyResponseDto.ImageUrl))]
    // Custom mapping for Reviews collection
    [MapProperty(nameof(Property.Reviews), nameof(PropertyResponseDto.Reviews), Use = nameof(MapReviews))]
    public partial PropertyResponseDto PropertyToPropertyResponseDto(Property property);

    private string MapImageUrl(string? imageUrl) => imageUrl ?? string.Empty;

    private List<ReviewResponseDto>? MapReviews(ICollection<Review> reviews)
    {
        if (reviews == null || !reviews.Any())
            return null;

        return reviews.Select(r => new ReviewResponseDto
        {
            ReviewID = r.ReviewID,
            PropertyID = r.PropertyID,
            UserID = r.UserID,
            BookingID = r.BookingID,
            Rating = r.Rating,
            Comment = r.Comment
        }).ToList();
    }
}
