using Contracts;
using Contracts.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
