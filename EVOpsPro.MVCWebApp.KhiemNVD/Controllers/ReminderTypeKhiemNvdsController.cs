using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVOpsPro.MVCWebApp.KhiemNVD.Controllers
{
    [Authorize]
    public class ReminderTypeKhiemNvdsController : Controller
    {
        public IActionResult Index() => View();
    }
}
