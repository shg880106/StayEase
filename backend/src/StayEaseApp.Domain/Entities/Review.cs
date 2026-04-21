using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Domain.Entities;
public class Review
{
    public Guid ReviewID { get; set; }
    public Guid PropertyID { get; set; }
    public Guid UserID { get; set; }
    public int Rating { get; set; } // 1 to 5
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Property Property { get; set; } = null!;

    private Review() { }

    public Review(Guid propertyId, Guid userId, int rating, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5");

        ReviewID = Guid.NewGuid();
        PropertyID = propertyId;
        UserID = userId;
        Rating = rating;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
    }
}
