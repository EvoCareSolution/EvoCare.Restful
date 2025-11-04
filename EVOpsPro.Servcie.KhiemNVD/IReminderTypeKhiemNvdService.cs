using EVOpsPro.Repositories.KhiemNVD.Models;

namespace EVOpsPro.Servcies.KhiemNVD
{
    public interface IReminderTypeKhiemNvdService
    {
        Task<List<ReminderTypeKhiemNvd>> GetAllAsync();
        Task<List<ReminderTypeKhiemNvd>> GetActiveAsync();
        Task<ReminderTypeKhiemNvd?> GetByIdAsync(int id);
        Task<int> CreateAsync(ReminderTypeKhiemNvd entity);
        Task<int> UpdateAsync(ReminderTypeKhiemNvd entity);
        Task<bool> DeleteAsync(int id);
    }
}
