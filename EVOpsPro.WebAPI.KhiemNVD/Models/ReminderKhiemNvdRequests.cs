using System;
using System.ComponentModel.DataAnnotations;
using EVOpsPro.Repositories.KhiemNVD.Models;

namespace EVOpsPro.WebAPI.KhiemNVD.Models
{
    public abstract class ReminderKhiemNvdUpsertRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int UserAccountId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ReminderTypeKhiemNvdid { get; set; }

        [Required]
        [StringLength(50)]
        public string VehicleVin { get; set; } = string.Empty;

        [Required]
        public DateTime DueDate { get; set; }

        [Range(0, int.MaxValue)]
        public int? DueKm { get; set; }

        [StringLength(200)]
        public string? Message { get; set; }

        public bool IsSent { get; set; }

        public bool IsActive { get; set; } = true;

        protected ReminderKhiemNvd MapToEntity(int? reminderId = null)
        {
            return new ReminderKhiemNvd
            {
                ReminderKhiemNvdid = reminderId ?? 0,
                UserAccountId = UserAccountId,
                ReminderTypeKhiemNvdid = ReminderTypeKhiemNvdid,
                VehicleVin = VehicleVin,
                DueDate = DueDate,
                DueKm = DueKm,
                Message = Message,
                IsSent = IsSent,
                IsActive = IsActive
            };
        }
    }

    public sealed class ReminderKhiemNvdCreateRequest : ReminderKhiemNvdUpsertRequest
    {
        public ReminderKhiemNvd ToEntity() => MapToEntity();
    }

    public sealed class ReminderKhiemNvdUpdateRequest : ReminderKhiemNvdUpsertRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ReminderKhiemNvdid { get; set; }

        public ReminderKhiemNvd ToEntity() => MapToEntity(ReminderKhiemNvdid);
    }
}
