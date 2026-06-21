using Contracts;
using Contracts.Admin;

namespace Training.Blazor.Services.Role
{
    public interface IRoleApiClient
    {
        Task<Result<List<RoleListDto>>?> GetRolesAsync();
        Task<Result<RoleDetailDto>?> GetRoleByIdAsync(long id);
        Task<Result<long>?> CreateRoleAsync(CreateRoleRequest request);
        Task<Result<RoleDetailDto>?> UpdateRoleAsync(UpdateRoleRequest request);
        Task<Result<bool>?> DeleteRoleAsync(long id);
    }
}
