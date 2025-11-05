using EVOpsPro.Repositories.KhiemNVD.Models;
using EVOpsPro.Servcies.KhiemNVD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVOpsPro.WebAPI.KhiemNVD.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReminderTypeKhiemNvdController : ControllerBase
    {
        private readonly IReminderTypeKhiemNvdService _reminderTypeService;

        public ReminderTypeKhiemNvdController(IReminderTypeKhiemNvdService reminderTypeService)
        {
            _reminderTypeService = reminderTypeService;
        }

        [HttpGet]
        [Authorize(Roles = "1,2")]
        public async Task<List<ReminderTypeKhiemNvd>> GetAllAsync()
        {
            return await _reminderTypeService.GetAllAsync();
        }

        [HttpGet("active")]
        [Authorize(Roles = "1,2")]
        public async Task<List<ReminderTypeKhiemNvd>> GetActiveAsync()
        {
            return await _reminderTypeService.GetActiveAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "1,2")]
        public async Task<ActionResult<ReminderTypeKhiemNvd>> GetByIdAsync(int id)
        {
            var entity = await _reminderTypeService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }

        [HttpPost]
        [Authorize(Roles = "1,2")]
        public async Task<int> CreateAsync(ReminderTypeKhiemNvd reminderType)
        {
            if (!ModelState.IsValid)
            {
                return 0;
            }

            return await _reminderTypeService.CreateAsync(reminderType);
        }

        [HttpPut]
        [Authorize(Roles = "1,2")]
        public async Task<int> UpdateAsync(ReminderTypeKhiemNvd reminderType)
        {
            if (!ModelState.IsValid)
            {
                return 0;
            }

            return await _reminderTypeService.UpdateAsync(reminderType);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "1")]
        public async Task<bool> DeleteAsync(int id)
        {
            return await _reminderTypeService.DeleteAsync(id);
        }
    }
}
