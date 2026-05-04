using StayEaseApp.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.Interfaces;
public interface IPropertyService
{
    Task<List<PropertyResponseDto>> GetPropertiesAsync();
    Task<PropertyResponseDto?> GetPropertyByIdAsync(Guid propertyId);
}
