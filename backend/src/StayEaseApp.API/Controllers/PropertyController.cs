using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayEaseApp.Application.DTOs;
using StayEaseApp.Application.Interfaces;
using StayEaseApp.Application.Services;
using System.Security.Claims;

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
            return BadRequest(new { message = ex.Message });
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
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Create a new property (requires authentication)
    /// </summary>
    /// <returns>
    /// Returns one of the following HTTP status codes:
    /// <list type="bullet">
    ///   <item><description>201 Created - Property created successfully</description></item>
    ///   <item><description>400 Bad Request - Invalid input data or property validation failed</description></item>
    ///   <item><description>401 Unauthorized - User not authenticated</description></item>
    /// </list>
    /// </returns>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(PropertyResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateProperty([FromBody] CreatePropertyRequestDto propertyRequest)
    {
        try
        {
            // Extract UserID from JWT token claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid or missing user authentication" });
            }

            // Override the OwnerID with the authenticated user's ID
            propertyRequest.OwnerID = userId;

            var createdProperty = await _propertyService.CreatePropertyAsync(propertyRequest);

            return CreatedAtAction(nameof(GetPropertyById), new { propertyId = createdProperty.PropertyID }, createdProperty);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a property (requires authentication and ownership)
    /// </summary>
    /// <returns>
    /// Returns one of the following HTTP status codes:
    /// <list type="bullet">
    ///   <item><description>200 Ok - Property deleted successfully</description></item>
    ///   <item><description>400 Bad Request - Invalid input data or property validation failed</description></item>
    ///   <item><description>401 Unauthorized - User not authenticated</description></item>
    ///   <item><description>403 Forbidden - User does not own this property</description></item>
    ///   <item><description>404 Not Found - Property not found</description></item>
    /// </list>
    /// </returns>
    [Authorize]
    [HttpDelete("{propertyId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProperty(Guid propertyId)
    {
        try
        {
            var property = await _propertyService.GetPropertyByIdAsync(propertyId);
            if (property == null)
            {
                return NotFound(new { message = $"Property with ID {propertyId} not found." });
            }

            // Optional: Verify the authenticated user owns the property
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid or missing user authentication" });
            }

            if (property.OwnerID != userId)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to delete this property. Only the property owner can delete it." });
            }

            await _propertyService.DeletePropertyAsync(propertyId);
            return Ok(new { message = "Property deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
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
            return BadRequest(new { message = ex.Message });
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
            return BadRequest(new { message = ex.Message });
        }
    }
}