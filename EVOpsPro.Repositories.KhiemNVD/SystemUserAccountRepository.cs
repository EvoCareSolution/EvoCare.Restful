using EVOpsPro.Repositories.KhiemNVD.Basic;
using EVOpsPro.Repositories.KhiemNVD.DBContext;
using EVOpsPro.Repositories.KhiemNVD.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EVOpsPro.Repositories.KhiemNVD
{
    public class SystemUserAccountRepository : GenericRepository<SystemUserAccount>
    {
        public SystemUserAccountRepository()
        {
        }

        public SystemUserAccountRepository(FA25_PRN232_SE1713_G2_EVOpsProContext context) => _context = context;

        public async Task<SystemUserAccount> GetByUsernameAsync(string username, string password)
        {
            username = username?.Trim() ?? string.Empty;

            return await _context.SystemUserAccounts.FirstOrDefaultAsync(u =>
                u.Password == password && u.IsActive &&
                (u.UserName == username ||
                 u.Email == username ||
                 u.Phone == username ||
                 u.EmployeeCode == username));
        }

        public async Task<List<SystemUserAccount>> GetAllActiveAsync()
        {
            return await _context.SystemUserAccounts
                .Where(u => u.IsActive)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }
    }

}
