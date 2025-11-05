namespace EVOpsPro.WebAPI.KhiemNVD.Models
{
    public sealed class PowerBIReminderDto
    {
        public int ReminderId { get; set; }
        public string VehicleVin { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public int? DueKm { get; set; }
        public bool IsSent { get; set; }
        public bool IsActive { get; set; }
        public string? Message { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int ReminderTypeId { get; set; }
        public string ReminderTypeName { get; set; } = string.Empty;
        public bool ReminderTypeIsRecurring { get; set; }
        public int? ReminderTypeIntervalDays { get; set; }
        public int? ReminderTypeIntervalKm { get; set; }
        public bool ReminderTypeIsPaymentRelated { get; set; }
        public int UserAccountId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public bool UserIsActive { get; set; }
    }
}
