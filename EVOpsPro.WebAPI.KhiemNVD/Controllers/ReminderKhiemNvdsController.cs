using EVOpsPro.Repositories.KhiemNVD.ModelExtensions;
using EVOpsPro.Repositories.KhiemNVD.Models;
using EVOpsPro.Servcies.KhiemNVD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EVOpsPro.WebAPI.KhiemNVD.Models;

namespace EVOpsPro.WebAPI.KhiemNVD.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReminderKhiemNvdsController : ControllerBase
    {
        private readonly IReminderKhiemNvdService _reminderService;

        public ReminderKhiemNvdsController(IReminderKhiemNvdService reminderService)
        {
            _reminderService = reminderService;
        }

        [HttpGet]
        [Authorize(Roles = "1,2")]
        public async Task<IEnumerable<ReminderKhiemNvd>> GetAllAsync()
        {
            return await _reminderService.GetAllAsync();
        }

        [HttpGet("vin-options")]
        [Authorize(Roles = "1,2")]
        public async Task<IEnumerable<VehicleVinOption>> GetVehicleVinOptions([FromQuery] string? keyword)
        {
            return await _reminderService.GetVehicleVinOptionsAsync(keyword);
        }

        [HttpPost("search")]
        [Authorize(Roles = "1,2")]
        public async Task<IEnumerable<ReminderKhiemNvd>> SearchAsync([FromBody] ReminderSearchRequest request)
        {
            request ??= new ReminderSearchRequest();
            return await _reminderService.SearchAsync(request);
        }

        [HttpPost("searchWithPaging")]
        [Authorize(Roles = "1,2")]
        public async Task<PaginationResult<List<ReminderKhiemNvd>>> SearchWithPaging([FromBody] ReminderSearchRequest request)
        {
            request ??= new ReminderSearchRequest();
            return await _reminderService.SearchWithPagingAsync(request);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "1,2")]
        public async Task<ActionResult<ReminderKhiemNvd>> GetById(int id)
        {
            var item = await _reminderService.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPost]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> Post(ReminderKhiemNvdCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _reminderService.CreateAsync(request.ToEntity());
            if (result <= 0)
            {
                return BadRequest("Unable to create reminder.");
            }

            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> Put(ReminderKhiemNvdUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _reminderService.UpdateAsync(request.ToEntity());
            if (result <= 0)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPatch("{id}/mark-sent")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> MarkAsSent(int id, [FromBody] MarkReminderSentRequest request)
        {
            var updated = await _reminderService.MarkAsSentAsync(id, request?.Message);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "1")]
        public async Task<bool> Delete(int id)
        {
            return await _reminderService.DeleteAsync(id);
        }

        public sealed record MarkReminderSentRequest(string? Message);
    }
}
