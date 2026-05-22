using StayEaseApp.Application.DTOs;
using StayEaseApp.Application.Interfaces;
using StayEaseApp.Application.Mappers;
using StayEaseApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.Services;
public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly PropertyMapper _mapper = new();

    public PropertyService(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }


    public async Task<List<PropertyResponseDto>> GetPropertiesAsync()
    {
        var properties = await _propertyRepository.GetPropertiesAsync();

        return properties.Select(p => _mapper.PropertyToPropertyResponseDto(p)).ToList();
    }

    public async Task<PropertyResponseDto?> GetPropertyByIdAsync(Guid propertyId)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId);

        return property == null ? null : _mapper.PropertyToPropertyResponseDto(property);
    }
    public async Task<PropertyResponseDto> CreatePropertyAsync(CreatePropertyRequestDto propertyRequest)
    {
        var newProperty = new Property
        {
            PropertyID = Guid.NewGuid(),
            OwnerID = propertyRequest.OwnerID,
            Title = propertyRequest.Title,
            Description = propertyRequest.Description,
            PricePerNight = propertyRequest.PricePerNight,
            Location = propertyRequest.Location,
            MaxGuests = propertyRequest.MaxGuests,
            ImageUrl = propertyRequest.ImageUrl
        };

        var createdProperty = await _propertyRepository.CreatePropertyAsync(newProperty);
        return _mapper.PropertyToPropertyResponseDto(createdProperty);
    }

    public async Task DeletePropertyAsync(Guid propertyId)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId);
        if (property == null)
        {
            throw new Exception($"Property with ID {propertyId} not found.");
        }
        await _propertyRepository.DeletePropertyAsync(propertyId);
    }

    public async Task<PropertyResponseDto> UpdatePropertyAsync(Guid propertyId, UpdatePropertyRequestDto propertyRequest)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId);
        if (property == null)
        {
            throw new Exception($"Property with ID {propertyId} not found.");
        }
        property.Title = propertyRequest.Title;
        property.Description = propertyRequest.Description;
        property.PricePerNight = propertyRequest.PricePerNight;
        property.Location = propertyRequest.Location;
        property.MaxGuests = propertyRequest.MaxGuests;
        property.ImageUrl = propertyRequest.ImageUrl;

        await _propertyRepository.UpdatePropertyAsync(propertyId, property);

        return _mapper.PropertyToPropertyResponseDto(property);
    }

    public async Task<List<PropertyResponseDto>> GetPropertiesSearchFiltersAsync(PropertySearchFiltersDto filters)
    {
        var properties = await _propertyRepository.GetPropertiesAsync();

        // Apply filters
        if (!string.IsNullOrEmpty(filters.Location))
        {
            properties = properties.Where(p => p.Location.Contains(filters.Location, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        if (filters.MinPrice.HasValue)
        {
            properties = properties.Where(p => p.PricePerNight >= filters.MinPrice.Value).ToList();
        }
        if (filters.MaxPrice.HasValue)
        {
            properties = properties.Where(p => p.PricePerNight <= filters.MaxPrice.Value).ToList();
        }
        if (filters.MinGuests.HasValue)
        {
            properties = properties.Where(p => p.MaxGuests >= filters.MinGuests.Value).ToList();
        }
        if (filters.MaxGuests.HasValue)
        {
            properties = properties.Where(p => p.MaxGuests <= filters.MaxGuests.Value).ToList();
        }

        return properties.Select(p => _mapper.PropertyToPropertyResponseDto(p)).ToList();
    }

    public async Task<List<PropertyResponseDto>> GetPropertiesByOwnerIdAsync(Guid ownerId)
    {
        var properties = await _propertyRepository.GetPropertiesByOwnerIdAsync(ownerId);
        return properties.Select(p => _mapper.PropertyToPropertyResponseDto(p)).ToList();
    }
}