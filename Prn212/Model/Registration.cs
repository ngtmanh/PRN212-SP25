using System;
using System.Collections.Generic;

namespace Prn212.Model;

public partial class Registration
{
    public int RegistrationId { get; set; }

    public int? UserId { get; set; }

    public string RegistrationType { get; set; } = null!;

    public int? RegistrationDetailId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Status { get; set; }

    public int? ApprovedBy { get; set; }

    public string? Comments { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual RegistrationDetail? RegistrationDetail { get; set; }

    public virtual User? User { get; set; }
}
