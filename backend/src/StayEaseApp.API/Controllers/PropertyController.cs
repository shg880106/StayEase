using Microsoft.AspNetCore.Mvc;
using StayEaseApp.Application.DTOs;
using StayEaseApp.Application.Interfaces;
using StayEaseApp.Application.Services;

namespace StayEaseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertyController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertyController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    /// <summary>
    /// Get a list with all properties
    /// </summary>
    /// <returns>
    /// Returns one of the following HTTP status codes:
    /// <list type="bullet">
    ///   <item><description>200 Ok - List with all properties details in response body</description></item>
    ///   <item><description>400 Bad Request - Invalid input data or property validation failed</description></item>
    /// </list>
    /// </returns>
    [HttpGet]
    [ProducesResponseType(typeof(PropertyResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllProperties()
    {
        try
        {
            var properties = await _propertyService.GetPropertiesAsync();
            return Ok(properties);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get the property details by the property id
    /// </summary>
    /// <returns>
    /// Returns one of the following HTTP status codes:
    /// <list type="bullet">
    ///   <item><description>200 Ok - Property details in response body</description></item>
    ///   <item><description>404 Not Found - Property not found</description></item>
    /// </list>
    /// </returns>
    [HttpGet("{propertyId}")]
    [ProducesResponseType(typeof(PropertyResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPropertyById(Guid propertyId)
    {
        try
        {
            var property = await _propertyService.GetPropertyByIdAsync(propertyId);
            if (property == null)
            {
                return NotFound($"Property with ID {propertyId} not found.");
            }
            return Ok(property);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Create a new property with the provided details
    /// </summary>
    /// <returns>
    /// Returns one of the following HTTP status codes:
    /// <list type="bullet">
    ///   <item><description>201 Created - Property created successfully</description></item>
    ///   <item><description>400 Bad Request - Invalid input data or property validation failed</description></item>
    /// </list>
    /// </returns>
    [HttpPost]
    [ProducesResponseType(typeof(PropertyResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProperty([FromBody] CreatePropertyRequestDto propertyRequest)
    {
        try
        {
            var createdProperty = await _propertyService.CreatePropertyAsync(propertyRequest);

            return CreatedAtAction(nameof(GetPropertyById), new { propertyId = createdProperty.PropertyID }, createdProperty);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete a property with the provided details
    /// </summary>
    /// <returns>
    /// Returns one of the following HTTP status codes:
    /// <list type="bullet">
    ///   <item><description>200 Ok - Property deleted successfully</description></item>
    ///   <item><description>400 Bad Request - Invalid input data or property validation failed</description></item>
    /// </list>
    /// </returns>
    [HttpDelete("{propertyId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteProperty(Guid propertyId)
    {
        try
        {
            var property = await _propertyService.GetPropertyByIdAsync(propertyId);
            if (property == null)
            {
                return NotFound($"Property with ID {propertyId} not found.");
            }

            await _propertyService.DeletePropertyAsync(propertyId);
            return Ok($"Property with ID {propertyId} deleted successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update a property with the provided details
    /// </summary>
    /// <returns>
    /// Returns one of the following HTTP status codes:
    /// <list type="bullet">
    ///   <item><description>200 Ok - Property updated successfully</description></item>
    ///   <item><description>400 Bad Request - Invalid input data or property validation failed</description></item>
    /// </list>
    /// </returns>
    [HttpPut("{propertyId}")]
    [ProducesResponseType(typeof(PropertyResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProperty(Guid propertyId, [FromBody] UpdatePropertyRequestDto propertyRequest)
    {
        try
        {
            var updatedProperty = await _propertyService.UpdatePropertyAsync(propertyId, propertyRequest);
            return Ok(updatedProperty);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get a list with all properties that match the provided search filters
    /// </summary>
    /// <param name="filters">The search filters to apply when retrieving properties</param>
    /// <returns>
    /// Returns one of the following HTTP status codes:
    /// <list type="bullet">
    ///   <item><description>200 Ok - Properties retrieved successfully</description></item>
    ///   <item><description>400 Bad Request - Invalid input data or property validation failed</description></item>
    /// </list>
    /// </returns>
    [HttpGet("search/filter")]
    [ProducesResponseType(typeof(List<PropertyResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProperties([FromQuery] PropertySearchFiltersDto filters)
    {
        try
        {
            var properties = await _propertyService.GetPropertiesSearchFiltersAsync(filters);
            return Ok(properties);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}