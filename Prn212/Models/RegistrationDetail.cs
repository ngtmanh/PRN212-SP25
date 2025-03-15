using System;
using System.Collections.Generic;

namespace Prn212.Models;

public partial class RegistrationDetail
{
    public int RegistrationDetailId { get; set; }

    public string VerifyingIdentity { get; set; } = null!;

    public string VerifyingResidence { get; set; } = null!;

    public int? HouseholdId { get; set; }

    public virtual Household? Household { get; set; }

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
