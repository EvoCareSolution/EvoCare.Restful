using System.ComponentModel.DataAnnotations;

namespace EVOpsPro.Repositories.KhiemNVD.Models;

public partial class ReminderKhiemNvd
{
    public int ReminderKhiemNvdid { get; set; }

    [Required(ErrorMessage = "Please select a user.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a user.")]
    public int UserAccountId { get; set; }

    [Required(ErrorMessage = "Please select reminder type.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select reminder type.")]
    public int ReminderTypeKhiemNvdid { get; set; }

    [Required(ErrorMessage = "Please select a vehicle.")]
    [Display(Name = "Vehicle VIN")]
    public string VehicleVin { get; set; } = null!;

    [Required(ErrorMessage = "Due date is required.")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Due Date")]
    public DateTime DueDate { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Due Km must be a positive number.")]
    [Display(Name = "Due Km")]
    public int? DueKm { get; set; }

    [StringLength(200, ErrorMessage = "Message cannot exceed 200 characters.")]
    public string? Message { get; set; }

    public bool IsSent { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ReminderTypeKhiemNvd ReminderTypeKhiemNvd { get; set; } = null!;

    public virtual SystemUserAccount SystemUserAccount { get; set; } = null!;
}
