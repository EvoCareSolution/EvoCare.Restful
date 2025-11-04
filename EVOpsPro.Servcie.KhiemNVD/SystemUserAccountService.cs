using EVOpsPro.Repositories.KhiemNVD;
using EVOpsPro.Repositories.KhiemNVD.Models;

namespace EVOpsPro.Servcies.KhiemNVD
{
    public class SystemUserAccountService
    {

        private readonly SystemUserAccountRepository _repository;
        public SystemUserAccountService() => _repository = new SystemUserAccountRepository();
        public async Task<SystemUserAccount> GetUserAccount(string username, string password)
        {
            try
            {
                return await _repository.GetByUsernameAsync(username, password);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<List<SystemUserAccount>> GetAllAsync()
        {
            return _repository.GetAllActiveAsync();
        }
    }
}
