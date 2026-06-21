using Contracts;
using Contracts.Admin;

namespace Training.Blazor.Services.AdminUser
{
    public interface IAdminUserApiClient
    {
        Task<Result<List<AdminUserListDto>>?> GetAllAsync();
        Task<Result<AdminUserDetailDto>?> GetByIdAsync(long id);
        Task<Result<long>?> CreateAsync(CreateAdminUserRequest request);
        Task<Result<AdminUserDetailDto>?> UpdateAsync(UpdateAdminUserRequest request);
        Task<Result<bool>?> ResetPasswordAsync(ResetPasswordRequest request);
        Task<Result<bool>?> DeleteAsync(long id);
    }
}
