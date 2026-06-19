using Contracts;
using Contracts.Admin;

namespace Train.Domain.Features.AdminUserFeature
{
    public interface IAdminUserService
    {
        Task<Result<List<AdminUserListDto>>> GetAllAsync(CancellationToken cancellationToken);
        Task<Result<AdminUserDetailDto>> GetByIdAsync(long id, CancellationToken cancellationToken);
        Task<Result<long>> CreateAsync(CreateAdminUserRequest request, CancellationToken cancellationToken);
        Task<Result<AdminUserDetailDto>> UpdateAsync(UpdateAdminUserRequest request, CancellationToken cancellationToken);
        Task<Result<bool>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken);
        Task<Result<bool>> DeleteAsync(long id, CancellationToken cancellationToken);
    }
}
