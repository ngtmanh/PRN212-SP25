using System;
using System.Collections.Generic;

namespace Prn212.Models;

public partial class RegistrationDetail
{
    public int RegistrationDetailId { get; set; }

    public string Description { get; set; } = null!;

    public string? VerifyingIdentity { get; set; }

    public string? VerifyingResidence { get; set; }

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
