using EVOpsPro.WebAPI.KhiemNVD.Models;
using EVOpsPro.Servcies.KhiemNVD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVOpsPro.WebAPI.KhiemNVD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PowerBIController : ControllerBase
    {
        private readonly IReminderKhiemNvdService _reminderService;

        public PowerBIController(IReminderKhiemNvdService reminderService)
        {
            _reminderService = reminderService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<PowerBIReminderDto>> Get()
        {
            var reminders = await _reminderService.GetAllAsync();
            return reminders.Select(r => new PowerBIReminderDto
            {
                ReminderId = r.ReminderKhiemNvdid,
                VehicleVin = r.VehicleVin,
                DueDate = r.DueDate,
                DueKm = r.DueKm,
                IsSent = r.IsSent,
                IsActive = r.IsActive,
                Message = r.Message,
                CreatedDate = r.CreatedDate,
                ModifiedDate = r.ModifiedDate,
                ReminderTypeId = r.ReminderTypeKhiemNvdid,
                ReminderTypeName = r.ReminderTypeKhiemNvd?.TypeName ?? string.Empty,
                ReminderTypeIsRecurring = r.ReminderTypeKhiemNvd?.IsRecurring ?? false,
                ReminderTypeIntervalDays = r.ReminderTypeKhiemNvd?.IntervalDays,
                ReminderTypeIntervalKm = r.ReminderTypeKhiemNvd?.IntervalKm,
                ReminderTypeIsPaymentRelated = r.ReminderTypeKhiemNvd?.IsPaymentRelated ?? false,
                UserAccountId = r.UserAccountId,
                UserName = r.SystemUserAccount?.UserName ?? string.Empty,
                FullName = r.SystemUserAccount?.FullName ?? string.Empty,
                Email = r.SystemUserAccount?.Email ?? string.Empty,
                Phone = r.SystemUserAccount?.Phone ?? string.Empty,
                RoleId = r.SystemUserAccount?.RoleId ?? 0,
                UserIsActive = r.SystemUserAccount?.IsActive ?? false
            });
        }
    }
}
