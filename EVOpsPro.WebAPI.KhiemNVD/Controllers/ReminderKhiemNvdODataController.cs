using EVOpsPro.Repositories.KhiemNVD.Models;
using EVOpsPro.Servcies.KhiemNVD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Linq;

namespace EVOpsPro.WebAPI.KhiemNVD.Controllers
{
    [Authorize]
    [Route("odata/ReminderKhiemNvd")]
    public class ReminderKhiemNvdODataController : ODataController
    {
        private readonly IReminderKhiemNvdService _service;

        public ReminderKhiemNvdODataController(IReminderKhiemNvdService service)
        {
            _service = service;
        }

        [EnableQuery(PageSize = 50)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data = await _service.GetAllAsync();
            return Ok(data.AsQueryable());
        }

        [EnableQuery]
        [HttpGet("({key})")]
        public async Task<IActionResult> Get([FromODataUri] int key)
        {
            var entity = await _service.GetByIdAsync(key);
            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }
    }
}
