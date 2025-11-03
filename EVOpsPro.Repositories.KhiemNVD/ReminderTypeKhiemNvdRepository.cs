using EVOpsPro.Repositories.KhiemNVD.Basic;
using EVOpsPro.Repositories.KhiemNVD.DBContext;
using EVOpsPro.Repositories.KhiemNVD.Models;
using Microsoft.EntityFrameworkCore;

namespace EVOpsPro.Repositories.KhiemNVD
{
    public class ReminderTypeKhiemNvdRepository : GenericRepository<ReminderTypeKhiemNvd>
    {
        public ReminderTypeKhiemNvdRepository()
        {
        }

        public ReminderTypeKhiemNvdRepository(FA25_PRN232_SE1713_G2_EVOpsProContext context) => _context = context;

        public async Task<List<ReminderTypeKhiemNvd>> GetAllAsync()
        {
            return await _context.ReminderTypeKhiemNvds.OrderByDescending(r => r.CreatedDate).ToListAsync();
        }

        public async Task<List<ReminderTypeKhiemNvd>> GetActiveAsync()
        {
            return await _context.ReminderTypeKhiemNvds
                .Where(r => r.IsActive)
                .OrderBy(r => r.TypeName)
                .ToListAsync();
        }

        public async Task<ReminderTypeKhiemNvd?> GetByIdAsync(int id)
        {
            return await _context.ReminderTypeKhiemNvds
                .FirstOrDefaultAsync(r => r.ReminderTypeKhiemNvdid == id);
        }

        public async Task<bool> ExistsByNameAsync(string typeName, int? excludeId = null)
        {
            typeName = typeName?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(typeName))
            {
                return false;
            }

            var query = _context.ReminderTypeKhiemNvds.AsQueryable();

            if (excludeId.HasValue)
            {
                query = query.Where(r => r.ReminderTypeKhiemNvdid != excludeId.Value);
            }

            return await query.AnyAsync(r => r.TypeName == typeName);
        }
    }
}
