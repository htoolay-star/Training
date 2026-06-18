using Contracts;
using Contracts.Admin;
using Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;
using Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train.Domain.Features.RoleFeature
{
    public sealed class RoleService(AppDbContext db) : IRoleService
    {
        public async Task<Result<List<RoleListDto>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var roles = await db.Roles
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .Select(r => new RoleListDto { Id = r.Id, Code = r.Code, Name = r.Name, IsSystem = r.IsSystem })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return Result<List<RoleListDto>>.Success(roles);
        }

        public async Task<Result<RoleDetailDto>> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            var role = await db.Roles
                .AsNoTracking()
                .Where(r => r.Id == id)
                .Select(r => new RoleDetailDto
                {
                    Id = r.Id,
                    Code = r.Code,
                    Name = r.Name,
                    IsSystem = r.IsSystem,
                })
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return role is null
                ? Result<RoleDetailDto>.SetResponse(ConstantResponseCode.RecordNotFound, EnumRespType.NotFound)
                : Result<RoleDetailDto>.Success(role);
        }

        public async Task<Result<long>> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken)
        {
            // Unique code (ignore soft-delete filter so a deleted code can't silently collide).
            var exists = await db.Roles.IgnoreQueryFilters()
                .AnyAsync(r => r.Code == request.Code, cancellationToken).ConfigureAwait(false);
            if (exists)
                return Result<long>.SetResponse(ConstantResponseCode.RecordDuplicate, EnumRespType.DuplicateRecord);

            var role = new Role { Code = request.Code, Name = request.Name, IsSystem = false };

            db.Roles.Add(role);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Result<long>.SetResponse(ConstantResponseCode.Saved, EnumRespType.Success, role.Id);
        }

        public async Task<Result<RoleDetailDto>> UpdateAsync(UpdateRoleRequest request, CancellationToken cancellationToken)
        {
            var role = await db.Roles
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);

            if (role is null)
                return Result<RoleDetailDto>.SetResponse(ConstantResponseCode.RecordNotFound, EnumRespType.NotFound);
            if (role.IsSystem)
                return Result<RoleDetailDto>.SetResponse(ConstantResponseCode.RoleProtected, EnumRespType.BadRequest);

            role.Name = request.Name;

            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Result<RoleDetailDto>.SetResponse(ConstantResponseCode.Saved, EnumRespType.Success, new RoleDetailDto
            {
                Id = role.Id,
                Code = role.Code,
                Name = role.Name,
                IsSystem = role.IsSystem,
            });
        }

        public async Task<Result<bool>> DeleteAsync(long id, CancellationToken cancellationToken)
        {
            var role = await db.Roles.FirstOrDefaultAsync(r => r.Id == id, cancellationToken).ConfigureAwait(false);
            if (role is null)
                return Result<bool>.SetResponse(ConstantResponseCode.RecordNotFound, EnumRespType.NotFound);
            if (role.IsSystem)
                return Result<bool>.SetResponse(ConstantResponseCode.RoleProtected, EnumRespType.BadRequest);

            var assigned = await db.AdminUsers.AnyAsync(u => u.RoleId == id, cancellationToken).ConfigureAwait(false);
            if (assigned)
                return Result<bool>.Error("This role is assigned to one or more users and cannot be deleted.");

            db.Roles.Remove(role); // soft delete (AuditableEntity)
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Result<bool>.SetResponse(ConstantResponseCode.Saved, EnumRespType.Success, true);
        }
    }
}
