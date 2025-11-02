using System.ComponentModel.DataAnnotations;

namespace EVOpsPro.Repositories.KhiemNVD.Models;

public partial class VehicleTypeTriNc
{
    [Key]

    public int VehicleTypeTriNcid { get; set; }

    public string Name { get; set; } = null!;

    public string? Country { get; set; }

    public string? Website { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<CustomerVehicleTriNc> CustomerVehicleTriNcs { get; set; } = new List<CustomerVehicleTriNc>();
}
