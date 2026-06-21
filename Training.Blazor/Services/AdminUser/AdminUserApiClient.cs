using Contracts;
using Contracts.Admin;

namespace Training.Blazor.Services.AdminUser
{
    public sealed class AdminUserApiClient : IAdminUserApiClient
    {
        private readonly ApiClient _apiClient;
        public AdminUserApiClient(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        public Task<Result<List<AdminUserListDto>>?> GetAllAsync()
        {
            return _apiClient.GetAsync<List<AdminUserListDto>>("api/AdminUser");
        }
        public Task<Result<AdminUserDetailDto>?> GetByIdAsync(long id)
        {
            return _apiClient.GetAsync<AdminUserDetailDto>($"api/AdminUser/{id}");
        }
        public Task<Result<long>?> CreateAsync(CreateAdminUserRequest request)
        {
            return _apiClient.PostAsync<long>("api/AdminUser", request);
        }
        public Task<Result<AdminUserDetailDto>?> UpdateAsync(UpdateAdminUserRequest request)
        {
            return _apiClient.PutAsync<AdminUserDetailDto>("api/AdminUser", request);
        }
        public Task<Result<bool>?> ResetPasswordAsync(ResetPasswordRequest request)
        {
            return _apiClient.PostAsync<bool>("api/AdminUser/ResetPassword", request);
        }
        public Task<Result<bool>?> DeleteAsync(long id)
        {
            return _apiClient.DeleteAsync<bool>($"api/AdminUser/{id}");
        }
    }
}
