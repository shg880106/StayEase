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
}
