using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class ServiceRequest
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public string ServiceType { get; set; } = null!;

    public DateTime RequestDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Details { get; set; }

    public virtual Student Student { get; set; } = null!;
}
