using System;
using System.Collections.Generic;

namespace Prn212.Models;

public partial class RegistrationDetail
{
    public int RegistrationDetailId { get; set; }

    public string Description { get; set; } = null!;

    public string? VerifyingIdentity { get; set; }

    public string? ResidenceFileName { get; set; }

    public string? ResidenceFileType { get; set; }

    public byte[]? ResidenceFileData { get; set; }

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
