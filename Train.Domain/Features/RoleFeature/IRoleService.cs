using Contracts;
using Contracts.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train.Domain.Features.RoleFeature
{
    public interface IRoleService
    {
        Task<Result<List<RoleListDto>>> GetAllAsync(CancellationToken cancellationToken);
        Task<Result<RoleDetailDto>> GetByIdAsync(long id, CancellationToken cancellationToken);
        Task<Result<long>> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken);
        Task<Result<RoleDetailDto>> UpdateAsync(UpdateRoleRequest request, CancellationToken cancellationToken);
        Task<Result<bool>> DeleteAsync(long id, CancellationToken cancellationToken);
    }
}
