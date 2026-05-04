using StayEaseApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.Interfaces;
public interface IPropertyRepository
{
    Task<Property?> GetByIdAsync(Guid propertyId);
    Task<List<Property>> GetPropertiesAsync();
    Task<Property> CreatePropertyAsync(Property property);
    Task DeletePropertyAsync(Guid propertyId);
    Task<Property> UpdatePropertyAsync(Guid propertyId, Property propertyRequest);
}
