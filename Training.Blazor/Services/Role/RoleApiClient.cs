using Contracts;
using Contracts.Admin;

namespace Training.Blazor.Services.Role
{
    public sealed class RoleApiClient : IRoleApiClient
    {
        private readonly ApiClient _apiClient;
        public RoleApiClient(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        public Task<Result<List<RoleListDto>>?> GetRolesAsync()
        {
            return _apiClient.GetAsync<List<RoleListDto>>("api/Role");
        }
        public Task<Result<RoleDetailDto>?> GetRoleByIdAsync(long id)
        {
            return _apiClient.GetAsync<RoleDetailDto>($"api/Role/{id}");
        }
        public Task<Result<long>?> CreateRoleAsync(CreateRoleRequest request)
        {
            return _apiClient.PostAsync<long>("api/Role", request);
        }
        public Task<Result<RoleDetailDto>?> UpdateRoleAsync(UpdateRoleRequest request)
        {
            return _apiClient.PutAsync<RoleDetailDto>("api/Role", request);
        }
        public Task<Result<bool>?> DeleteRoleAsync(long id)
        {
            return _apiClient.DeleteAsync<bool>($"api/Role/{id}");
        }
    }
}
