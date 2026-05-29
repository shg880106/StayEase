using Microsoft.EntityFrameworkCore;
using StayEaseApp.Application.DTOs;
using StayEaseApp.Application.Interfaces;
using StayEaseApp.Domain.Entities;
using StayEaseApp.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Infrastructure.Repositories;
public class PropertyRepository : IPropertyRepository
{
    private readonly AppDbContext _dbContext;

    public PropertyRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Property?> GetByIdAsync(Guid propertyId)
    {
        return await _dbContext.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PropertyID == propertyId);
    }

    public async Task<List<Property>> GetPropertiesAsync()
    {
        return await _dbContext.Properties
            .Where(p => !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<Property> CreatePropertyAsync(Property property)
    {
        _dbContext.Properties.Add(property);
        await _dbContext.SaveChangesAsync();
        return property;
    }

    public async Task DeletePropertyAsync(Guid propertyId)
    {
        var property = await _dbContext.Properties.FindAsync(propertyId);
        if (property != null)
        {
            // Soft delete instead of hard delete
            property.IsDeleted = true;
            property.DeletedAt = DateTime.UtcNow;
            _dbContext.Properties.Update(property);
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            throw new Exception($"Property with ID {propertyId} not found.");

        }
    }

    public async Task<Property> UpdatePropertyAsync(Guid propertyId, Property propertyRequest)
    {
        _dbContext.Properties.Update(propertyRequest);
        await _dbContext.SaveChangesAsync();
        return propertyRequest;
    }

    public async Task<List<Property>> GetPropertiesByOwnerIdAsync(Guid ownerId)
    {
        return await _dbContext.Properties
            .AsNoTracking()
            .Where(p => p.OwnerID == ownerId && !p.IsDeleted)
            .ToListAsync();
    }
}
