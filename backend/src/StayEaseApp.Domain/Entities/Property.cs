using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Domain.Entities;
public class Property
{   
    public Guid PropertyID { get; set; }
    public Guid OwnerID { get; set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public decimal PricePerNight { get; set; }
    public string Location { get; private set; }
    public int MaxGuests { get; set; }
    public string? ImageUrl { get; set; }

    // Navigation properties
    public User Owner { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    private Property() { }

    public Property(string title, string description, decimal pricePerNight, string location, int maxGuests, string imageUrl, Guid ownerID)
    {
        PropertyID = Guid.NewGuid();
        Title = title;
        Description = description;
        PricePerNight = pricePerNight;
        Location = location;
        MaxGuests = maxGuests;
        ImageUrl = imageUrl;
        OwnerID = ownerID;
    }
}
