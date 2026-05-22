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
    [MapperIgnoreSource(nameof(Property.Reviews))]
    // Map nullable ImageUrl to non-nullable target with fallback to empty string to resolve RMG089
    [MapProperty(nameof(Property.ImageUrl), nameof(PropertyResponseDto.ImageUrl))]
    public partial PropertyResponseDto PropertyToPropertyResponseDto(Property property);

    private string MapImageUrl(string? imageUrl) => imageUrl ?? string.Empty;
}
