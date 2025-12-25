using System;
using Microsoft.EntityFrameworkCore;

namespace Domain;

public class ActivityAttendee
{
    public string UserId { get; set; } = null!;

    public User User { get; set; } = null!;

    public string ActivityId { get; set; } = null!;

    public Activity Activity { get; set; } = null!;

    public bool IsHost { get; set; }

    public DateTime DateJoined { get; set; } = DateTime.UtcNow;
}
