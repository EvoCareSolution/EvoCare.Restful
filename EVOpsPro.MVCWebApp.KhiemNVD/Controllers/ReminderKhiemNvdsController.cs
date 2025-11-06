using EVOpsPro.Repositories.KhiemNVD.ModelExtensions;
using EVOpsPro.Repositories.KhiemNVD.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Net.Http;
using System.Linq;
using X.PagedList;

namespace EVOpsPro.MVCWebApp.KhiemNVD.Controllers
{
    [Authorize]
    public class ReminderKhiemNvdsController : Controller
    {
        private readonly string APIEndPoint = "https://localhost:7114/api/";

        private HttpClient CreateAuthorizedClient()
        {
            var httpClient = new HttpClient();
            var tokenString = HttpContext.Request.Cookies["TokenString"];
            if (!string.IsNullOrWhiteSpace(tokenString))
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenString);
            }

            return httpClient;
        }

        private async Task PopulateReminderTypesAsync(int? selectedId = null)
        {
            using var httpClient = CreateAuthorizedClient();
            using var response = await httpClient.GetAsync(APIEndPoint + "ReminderTypeKhiemNvd/active");

            var types = new List<ReminderTypeKhiemNvd>();
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                types = JsonConvert.DeserializeObject<List<ReminderTypeKhiemNvd>>(content) ?? new List<ReminderTypeKhiemNvd>();
            }

            ViewBag.ReminderTypes = new SelectList(types, "ReminderTypeKhiemNvdid", "TypeName", selectedId);
        }

        private async Task PopulateUserAccountsAsync(int? selectedId = null)
        {
            var accounts = await FetchSystemUserAccountsAsync();
            ViewBag.UserAccounts = new SelectList(accounts, "UserAccountId", "FullName", selectedId);
        }

        private async Task PopulateVehicleVinsAsync(string? selectedVin = null)
        {
            var vehicles = await FetchVehicleVinOptionsAsync();
            var list = vehicles
                .Select(v => new
                {
                    v.Vin,
                    Display = string.IsNullOrWhiteSpace(v.CustomerFullName) ? v.Vin : $"{v.Vin} - {v.CustomerFullName}"
                })
                .ToList();

            ViewBag.VehicleVins = new SelectList(list, "Vin", "Display", selectedVin);
        }

        // GET: ReminderKhiemNvds
        public async Task<IActionResult> Index(
            int? userAccountId,
            int? reminderTypeId,
            bool? isSent,
            bool? isActive,
            DateTime? dueDateFrom,
            DateTime? dueDateTo,
            string? keyword,
            string? vehicleVin,
            int? currentPage)
        {
            var searchRequest = new ReminderSearchRequest
            {
                UserAccountId = userAccountId,
                ReminderTypeId = reminderTypeId,
                IsSent = isSent,
                IsActive = isActive,
                DueDateFrom = dueDateFrom,
                DueDateTo = dueDateTo,
                VehicleVin = vehicleVin,
                Keyword = keyword,
                CurrentPage = currentPage ?? 1,
                PageSize = 5
            };

            ViewBag.SearchRequest = searchRequest;
            AssignFilterQuery(searchRequest);
            await PopulateReminderTypesAsync(reminderTypeId);
            await PopulateUserAccountsAsync(userAccountId);
            await PopulateVehicleVinsAsync(vehicleVin);

            using var httpClient = CreateAuthorizedClient();
            using var response = await httpClient.PostAsJsonAsync(APIEndPoint + "ReminderKhiemNvds/searchWithPaging", searchRequest);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<PaginationResult<List<ReminderKhiemNvd>>>(content);
                if (result != null)
                {
                    var items = result.Items ?? new List<ReminderKhiemNvd>();
                    var pagedList = new StaticPagedList<ReminderKhiemNvd>(
                        items,
                        result.CurrentPage,
                        result.PageSize,
                        result.TotalItems);

                    ViewBag.Reminders = pagedList;
                    return View(items);
                }
            }

            ViewBag.Reminders = new StaticPagedList<ReminderKhiemNvd>(new List<ReminderKhiemNvd>(), 1, 5, 0);
            return View(new List<ReminderKhiemNvd>());
        }

        // GET: ReminderKhiemNvds/Details/5
        public async Task<IActionResult> Details(int? id, [FromQuery] ReminderSearchRequest? filters)
        {
            AssignFilterQuery(filters);
            if (id == null)
            {
                return NotFound();
            }

            var reminder = await FetchReminderAsync(id.Value);
            if (reminder == null)
            {
                return NotFound();
            }

            return View(reminder);
        }

        // GET: ReminderKhiemNvds/Create
        public async Task<IActionResult> Create([FromQuery] ReminderSearchRequest? filters)
        {
            AssignFilterQuery(filters);
            await PopulateReminderTypesAsync();
            await PopulateUserAccountsAsync();
            await PopulateVehicleVinsAsync();
            var reminder = new ReminderKhiemNvd
            {
                DueDate = DateTime.Now,
                IsActive = true,
                IsSent = false
            };
            return View(reminder);
        }

        // POST: ReminderKhiemNvds/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReminderKhiemNvd reminder, [FromForm] ReminderSearchRequest? filters)
        {
            AssignFilterQuery(filters);
            ClearNavigationValidationState();
            if (!ModelState.IsValid)
            {
                await PopulateReminderTypesAsync(reminder.ReminderTypeKhiemNvdid);
                await PopulateUserAccountsAsync(reminder.UserAccountId);
                await PopulateVehicleVinsAsync(reminder.VehicleVin);
                return View(reminder);
            }

            using var httpClient = CreateAuthorizedClient();
            var response = await httpClient.PostAsJsonAsync(APIEndPoint + "ReminderKhiemNvds", reminder);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index), BuildFilterRouteValues(filters));
            }

            ModelState.AddModelError(string.Empty, "Unable to create reminder. Please try again.");
            await PopulateReminderTypesAsync(reminder.ReminderTypeKhiemNvdid);
            await PopulateUserAccountsAsync(reminder.UserAccountId);
            await PopulateVehicleVinsAsync(reminder.VehicleVin);
            return View(reminder);
        }

        // GET: ReminderKhiemNvds/Edit/5
        public async Task<IActionResult> Edit(int? id, [FromQuery] ReminderSearchRequest? filters)
        {
            AssignFilterQuery(filters);
            if (id == null)
            {
                return NotFound();
            }

            var reminder = await FetchReminderAsync(id.Value);
            if (reminder == null)
            {
                return NotFound();
            }

            await PopulateReminderTypesAsync(reminder.ReminderTypeKhiemNvdid);
            await PopulateUserAccountsAsync(reminder.UserAccountId);
            await PopulateVehicleVinsAsync(reminder.VehicleVin);
            return View(reminder);
        }

        // POST: ReminderKhiemNvds/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReminderKhiemNvd reminder, [FromForm] ReminderSearchRequest? filters)
        {
            AssignFilterQuery(filters);
            if (id != reminder.ReminderKhiemNvdid)
            {
                return BadRequest();
            }

            ClearNavigationValidationState();
            if (!ModelState.IsValid)
            {
                await PopulateReminderTypesAsync(reminder.ReminderTypeKhiemNvdid);
                await PopulateUserAccountsAsync(reminder.UserAccountId);
                await PopulateVehicleVinsAsync(reminder.VehicleVin);
                return View(reminder);
            }

            using var httpClient = CreateAuthorizedClient();
            var response = await httpClient.PutAsJsonAsync(APIEndPoint + "ReminderKhiemNvds", reminder);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index), BuildFilterRouteValues(filters));
            }

            ModelState.AddModelError(string.Empty, "Unable to update reminder. Please try again.");
            await PopulateReminderTypesAsync(reminder.ReminderTypeKhiemNvdid);
            await PopulateUserAccountsAsync(reminder.UserAccountId);
            await PopulateVehicleVinsAsync(reminder.VehicleVin);
            return View(reminder);
        }

        private void ClearNavigationValidationState()
        {
            ModelState.Remove(nameof(ReminderKhiemNvd.SystemUserAccount));
            ModelState.Remove(nameof(ReminderKhiemNvd.ReminderTypeKhiemNvd));
        }

        // GET: ReminderKhiemNvds/Delete/5
        public async Task<IActionResult> Delete(int? id, [FromQuery] ReminderSearchRequest? filters)
        {
            AssignFilterQuery(filters);
            if (id == null)
            {
                return NotFound();
            }

            var reminder = await FetchReminderAsync(id.Value);
            if (reminder == null)
            {
                return NotFound();
            }

            return View(reminder);
        }

        // POST: ReminderKhiemNvds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, [FromForm] ReminderSearchRequest? filters)
        {
            var routeValues = BuildFilterRouteValues(filters);
            using var httpClient = CreateAuthorizedClient();
            var response = await httpClient.DeleteAsync(APIEndPoint + "ReminderKhiemNvds/" + id);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index), routeValues);
            }

            TempData["ErrorMessage"] = "Unable to delete reminder. Please try again.";
            routeValues["id"] = id.ToString();
            return RedirectToAction(nameof(Delete), routeValues);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsSent(int id, string? message, [FromForm] ReminderSearchRequest? filters)
        {
            using var httpClient = CreateAuthorizedClient();

            var request = new HttpRequestMessage(HttpMethod.Patch, APIEndPoint + $"ReminderKhiemNvds/{id}/mark-sent")
            {
                Content = JsonContent.Create(new { message })
            };

            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Unable to mark reminder as sent.";
            }

            return RedirectToAction(nameof(Index), BuildFilterRouteValues(filters));
        }

        private void AssignFilterQuery(ReminderSearchRequest? filters)
        {
            var filterValues = BuildFilterQuery(filters);
            ViewBag.FilterQuery = filterValues;
            ViewBag.FilterQueryString = BuildFilterQueryString(filterValues);
        }

        private static Dictionary<string, object?> BuildFilterRouteValues(ReminderSearchRequest? request)
        {
            var values = new Dictionary<string, object?>();
            if (request == null)
            {
                return values;
            }

            void Add(string key, object? value)
            {
                if (value == null)
                {
                    return;
                }

                if (value is string s && string.IsNullOrWhiteSpace(s))
                {
                    return;
                }

                values[key] = value;
            }

            Add("userAccountId", request.UserAccountId);
            Add("reminderTypeId", request.ReminderTypeId);
            Add("vehicleVin", request.VehicleVin);
            Add("keyword", request.Keyword);
            Add("currentPage", request.CurrentPage);
            Add("pageSize", request.PageSize);
            Add("isSent", request.IsSent);
            Add("isActive", request.IsActive);
            Add("dueDateFrom", request.DueDateFrom?.ToString("yyyy-MM-dd"));
            Add("dueDateTo", request.DueDateTo?.ToString("yyyy-MM-dd"));
            Add("minDueKm", request.MinDueKm);
            Add("maxDueKm", request.MaxDueKm);

            return values;
        }

        private static Dictionary<string, string> BuildFilterQuery(ReminderSearchRequest? request)
        {
            var values = new Dictionary<string, string>();
            if (request == null)
            {
                return values;
            }

            void Add(string key, string? value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                values[key] = value;
            }

            Add("userAccountId", request.UserAccountId?.ToString());
            Add("reminderTypeId", request.ReminderTypeId?.ToString());
            Add("vehicleVin", request.VehicleVin);
            Add("keyword", request.Keyword);
            Add("currentPage", request.CurrentPage?.ToString());
            Add("pageSize", request.PageSize?.ToString());
            Add("isSent", request.IsSent.HasValue ? request.IsSent.Value.ToString().ToLowerInvariant() : null);
            Add("isActive", request.IsActive.HasValue ? request.IsActive.Value.ToString().ToLowerInvariant() : null);
            Add("dueDateFrom", request.DueDateFrom?.ToString("yyyy-MM-dd"));
            Add("dueDateTo", request.DueDateTo?.ToString("yyyy-MM-dd"));
            Add("minDueKm", request.MinDueKm?.ToString());
            Add("maxDueKm", request.MaxDueKm?.ToString());

            return values;
        }

        private static string BuildFilterQueryString(IDictionary<string, string> values)
        {
            if (values == null || values.Count == 0)
            {
                return string.Empty;
            }

            var pairs = values.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
            return "?" + string.Join("&", pairs);
        }

        private async Task<ReminderKhiemNvd?> FetchReminderAsync(int id)
        {
            using var httpClient = CreateAuthorizedClient();
            using var response = await httpClient.GetAsync(APIEndPoint + "ReminderKhiemNvds/" + id);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ReminderKhiemNvd>(content);
        }

        private async Task<List<SystemUserAccount>> FetchSystemUserAccountsAsync()
        {
            using var httpClient = CreateAuthorizedClient();
            using var response = await httpClient.GetAsync(APIEndPoint + "SystemUserAccounts");
            if (!response.IsSuccessStatusCode)
            {
                return new List<SystemUserAccount>();
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<SystemUserAccount>>(content) ?? new List<SystemUserAccount>();
        }

        private async Task<List<VehicleVinOption>> FetchVehicleVinOptionsAsync()
        {
            using var httpClient = CreateAuthorizedClient();
            using var response = await httpClient.GetAsync(APIEndPoint + "ReminderKhiemNvds/vin-options");
            if (!response.IsSuccessStatusCode)
            {
                return new List<VehicleVinOption>();
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<VehicleVinOption>>(content) ?? new List<VehicleVinOption>();
        }

    }
}
