using System;
using System.Linq;
using EVOpsPro.Repositories.KhiemNVD.Basic;
using EVOpsPro.Repositories.KhiemNVD.DBContext;
using EVOpsPro.Repositories.KhiemNVD.ModelExtensions;
using EVOpsPro.Repositories.KhiemNVD.Models;
using Microsoft.EntityFrameworkCore;

namespace EVOpsPro.Repositories.KhiemNVD
{
    public class ReminderKhiemNvdRepository : GenericRepository<ReminderKhiemNvd>
    {
        public ReminderKhiemNvdRepository()
        {
        }

        public ReminderKhiemNvdRepository(FA25_PRN232_SE1713_G2_EVOpsProContext context) => _context = context;

        public async Task<List<ReminderKhiemNvd>> GetAllAsync()
        {
            return await _context.ReminderKhiemNvds
                .Include(r => r.ReminderTypeKhiemNvd)
                .Include(r => r.SystemUserAccount)
                .OrderByDescending(r => r.DueDate)
                .ToListAsync();
        }

        public async Task<ReminderKhiemNvd?> GetByIdAsync(int id)
        {
            return await _context.ReminderKhiemNvds
                .Include(r => r.ReminderTypeKhiemNvd)
                .Include(r => r.SystemUserAccount)
                .FirstOrDefaultAsync(r => r.ReminderKhiemNvdid == id);
        }

        public async Task<List<ReminderKhiemNvd>> SearchAsync(ReminderSearchRequest request)
        {
            var query = BuildFilterQuery(request);

            return await query
                .OrderByDescending(r => r.DueDate)
                .ThenBy(r => r.VehicleVin)
                .ToListAsync();
        }

        public async Task<PaginationResult<List<ReminderKhiemNvd>>> SearchWithPagingAsync(ReminderSearchRequest request)
        {
            var query = BuildFilterQuery(request);

            var totalItems = await query.CountAsync();
            var pageSize = request.PageSize ?? 10;
            var currentPage = request.CurrentPage ?? 1;

            var items = await query
                .OrderByDescending(r => r.DueDate)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResult<List<ReminderKhiemNvd>>
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                Items = items
            };
        }

        public async Task<bool> MarkAsSentAsync(int reminderId, string? message)
        {
            var reminder = await _context.ReminderKhiemNvds
                .AsTracking()
                .FirstOrDefaultAsync(r => r.ReminderKhiemNvdid == reminderId);

            if (reminder == null)
            {
                return false;
            }

            reminder.IsSent = true;
            reminder.ModifiedDate = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(message))
            {
                reminder.Message = message;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<VehicleVinOption>> GetVehicleVinOptionsAsync(string? keyword = null)
        {
            var query = _context.CustomerVehicleTriNcs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var term = keyword.Trim();
                query = query.Where(v => v.Vin.Contains(term) || v.CustomerFullName.Contains(term));
            }

            return await query
                .OrderBy(v => v.Vin)
                .Select(v => new VehicleVinOption
                {
                    Vin = v.Vin,
                    CustomerFullName = v.CustomerFullName
                })
                .Take(100)
                .ToListAsync();
        }

        private IQueryable<ReminderKhiemNvd> BuildFilterQuery(ReminderSearchRequest request)
        {
            var query = _context.ReminderKhiemNvds
                .Include(r => r.ReminderTypeKhiemNvd)
                .Include(r => r.SystemUserAccount)
                .AsQueryable();

            if (request.UserAccountId.HasValue)
            {
                query = query.Where(r => r.UserAccountId == request.UserAccountId.Value);
            }

            if (request.ReminderTypeId.HasValue)
            {
                query = query.Where(r => r.ReminderTypeKhiemNvdid == request.ReminderTypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.VehicleVin))
            {
                var vin = request.VehicleVin.Trim();
                query = query.Where(r => r.VehicleVin == vin);
            }

            if (request.IsSent.HasValue)
            {
                query = query.Where(r => r.IsSent == request.IsSent.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(r => r.IsActive == request.IsActive.Value);
            }

            if (request.DueDateFrom.HasValue)
            {
                query = query.Where(r => r.DueDate >= request.DueDateFrom.Value);
            }

            if (request.DueDateTo.HasValue)
            {
                query = query.Where(r => r.DueDate <= request.DueDateTo.Value);
            }

            if (request.MinDueKm.HasValue)
            {
                query = query.Where(r => r.DueKm >= request.MinDueKm.Value);
            }

            if (request.MaxDueKm.HasValue)
            {
                query = query.Where(r => r.DueKm <= request.MaxDueKm.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var keyword = request.Keyword.Trim();
                query = query.Where(r =>
                    r.VehicleVin.Contains(keyword) ||
                    (r.Message != null && r.Message.Contains(keyword)));
            }

            return query;
        }
    }
}
