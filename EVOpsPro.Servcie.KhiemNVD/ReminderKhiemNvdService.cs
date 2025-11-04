using System;
using EVOpsPro.Repositories.KhiemNVD;
using EVOpsPro.Repositories.KhiemNVD.ModelExtensions;
using EVOpsPro.Repositories.KhiemNVD.Models;

namespace EVOpsPro.Servcies.KhiemNVD
{
    public class ReminderKhiemNvdService : IReminderKhiemNvdService
    {
        private readonly ReminderKhiemNvdRepository _repository;
        public ReminderKhiemNvdService() => _repository = new ReminderKhiemNvdRepository();

        public Task<List<ReminderKhiemNvd>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<ReminderKhiemNvd?> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }

        public Task<List<ReminderKhiemNvd>> SearchAsync(ReminderSearchRequest request)
        {
            request ??= new ReminderSearchRequest();
            return _repository.SearchAsync(request);
        }

        public Task<PaginationResult<List<ReminderKhiemNvd>>> SearchWithPagingAsync(ReminderSearchRequest request)
        {
            request ??= new ReminderSearchRequest();
            return _repository.SearchWithPagingAsync(request);
        }

        public async Task<int> CreateAsync(ReminderKhiemNvd entity)
        {
            try
            {
                entity.CreatedDate ??= DateTime.UtcNow;
                return await _repository.CreateAsync(entity);
            }
            catch (Exception)
            {
            }

            return 0;
        }

        public async Task<int> UpdateAsync(ReminderKhiemNvd entity)
        {
            try
            {
                entity.ModifiedDate = DateTime.UtcNow;
                return await _repository.UpdateAsync(entity);
            }
            catch (Exception)
            {
            }

            return 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    return false;
                }

                return await _repository.RemoveAsync(entity);
            }
            catch (Exception)
            {
            }

            return false;
        }

        public async Task<bool> MarkAsSentAsync(int id, string? message)
        {
            try
            {
                return await _repository.MarkAsSentAsync(id, message);
            }
            catch (Exception)
            {
            }

            return false;
        }

        public Task<List<VehicleVinOption>> GetVehicleVinOptionsAsync(string? keyword)
        {
            return _repository.GetVehicleVinOptionsAsync(keyword);
        }
    }
}
