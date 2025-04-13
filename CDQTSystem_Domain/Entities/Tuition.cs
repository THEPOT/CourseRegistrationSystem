using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class Tuition
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public Guid SemesterId { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal PaidAmount { get; set; }

    public string Status { get; set; } = null!;

    public DateTime DueDate { get; set; }

    public DateTime? LastPaymentDate { get; set; }

    public virtual Semester Semester { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
