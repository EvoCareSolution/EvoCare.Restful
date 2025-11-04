using System;
using EVOpsPro.Repositories.KhiemNVD;
using EVOpsPro.Repositories.KhiemNVD.Models;

namespace EVOpsPro.Servcies.KhiemNVD
{
    public class ReminderTypeKhiemNvdService : IReminderTypeKhiemNvdService
    {
        private readonly ReminderTypeKhiemNvdRepository _repository;
        public ReminderTypeKhiemNvdService() => _repository = new ReminderTypeKhiemNvdRepository();

        public Task<List<ReminderTypeKhiemNvd>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<List<ReminderTypeKhiemNvd>> GetActiveAsync()
        {
            return _repository.GetActiveAsync();
        }

        public Task<ReminderTypeKhiemNvd?> GetByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }

        public async Task<int> CreateAsync(ReminderTypeKhiemNvd entity)
        {
            try
            {
                if (await _repository.ExistsByNameAsync(entity.TypeName))
                {
                    return 0;
                }

                entity.CreatedDate ??= DateTime.UtcNow;
                return await _repository.CreateAsync(entity);
            }
            catch (Exception)
            {
            }

            return 0;
        }

        public async Task<int> UpdateAsync(ReminderTypeKhiemNvd entity)
        {
            try
            {
                if (await _repository.ExistsByNameAsync(entity.TypeName, entity.ReminderTypeKhiemNvdid))
                {
                    return 0;
                }

                entity.CreatedDate ??= DateTime.UtcNow;
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
    }
}
