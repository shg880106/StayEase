using StayEaseApp.Application.DTOs;
using StayEaseApp.Application.Interfaces;
using StayEaseApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.Services;
public class PropertyService
{
    private readonly IPropertyRepository _propertyRepository;

    public PropertyService(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<List<PropertyResponseDto>> GetPropertiesAsync()
    {
        var properties = await _propertyRepository.GetPropertiesAsync();

        return properties.Select(p => new PropertyResponseDto
        {
            PropertyID = p.PropertyID,
            UserID = p.OwnerID,
            Title = p.Title,
            Description = p.Description,
            PricePerNight = p.PricePerNight,
            Location = p.Location,
            MaxGuests = p.MaxGuests,
            ImageUrl = p.ImageUrl ?? string.Empty
        }).ToList();
    }
}
