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
}
