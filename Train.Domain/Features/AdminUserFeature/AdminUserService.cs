using Contracts;
using Contracts.Admin;
using Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;
using Shared.Auth;
using Shared.Constants;

namespace Train.Domain.Features.AdminUserFeature
{
    public sealed class AdminUserService(AppDbContext db, IPasswordHasher passwordHasher) : IAdminUserService
    {
        public async Task<Result<List<AdminUserListDto>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var users = await db.AdminUsers
                .AsNoTracking()
                .OrderBy(u => u.UserName)
                .Select(u => new AdminUserListDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    RoleId = u.RoleId,
                    RoleName = u.Role!.Name,
                    IsActive = u.IsActive,
                    LastLoginAt = u.LastLoginAt
                })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return Result<List<AdminUserListDto>>.Success(users);
        }

        public async Task<Result<AdminUserDetailDto>> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            var user = await db.AdminUsers
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new AdminUserDetailDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    RoleId = u.RoleId,
                    IsActive = u.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return user is null
                ? Result<AdminUserDetailDto>.SetResponse(ConstantResponseCode.RecordNotFound, EnumRespType.NotFound)
                : Result<AdminUserDetailDto>.Success(user);
        }

        public async Task<Result<long>> CreateAsync(CreateAdminUserRequest request, CancellationToken cancellationToken)
        {
            var dupe = await db.AdminUsers.IgnoreQueryFilters()
                .AnyAsync(u => u.UserName == request.UserName, cancellationToken).ConfigureAwait(false);
            if (dupe)
                return Result<long>.SetResponse(ConstantResponseCode.RecordDuplicate, EnumRespType.DuplicateRecord);

            if (!await RoleExistsAsync(request.RoleId, cancellationToken).ConfigureAwait(false))
                return Result<long>.SetResponse(ConstantResponseCode.RecordNotFound, EnumRespType.NotFound);

            var user = new AdminUser
            {
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = passwordHasher.Hash(request.Password),
                RoleId = request.RoleId,
                IsActive = request.IsActive
            };

            db.AdminUsers.Add(user);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Result<long>.SetResponse(ConstantResponseCode.Saved, EnumRespType.Success, user.Id);
        }

        public async Task<Result<AdminUserDetailDto>> UpdateAsync(UpdateAdminUserRequest request, CancellationToken cancellationToken)
        {
            var user = await db.AdminUsers.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken).ConfigureAwait(false);
            if (user is null)
                return Result<AdminUserDetailDto>.SetResponse(ConstantResponseCode.RecordNotFound, EnumRespType.NotFound);

            if (!await RoleExistsAsync(request.RoleId, cancellationToken).ConfigureAwait(false))
                return Result<AdminUserDetailDto>.SetResponse(ConstantResponseCode.RecordNotFound, EnumRespType.NotFound);

            user.Email = request.Email;
            user.RoleId = request.RoleId;
            user.IsActive = request.IsActive;

            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Result<AdminUserDetailDto>.SetResponse(ConstantResponseCode.Saved, EnumRespType.Success, new AdminUserDetailDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleId = user.RoleId,
                IsActive = user.IsActive
            });
        }

        public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await db.AdminUsers.FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken).ConfigureAwait(false);
            if (user is null)
                return Result<bool>.SetResponse(ConstantResponseCode.RecordNotFound, EnumRespType.NotFound);

            user.PasswordHash = passwordHasher.Hash(request.NewPassword);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Result<bool>.SetResponse(ConstantResponseCode.Saved, EnumRespType.Success, true);
        }

        public async Task<Result<bool>> DeleteAsync(long id, CancellationToken cancellationToken)
        {
            var user = await db.AdminUsers.FirstOrDefaultAsync(u => u.Id == id, cancellationToken).ConfigureAwait(false);
            if (user is null)
                return Result<bool>.SetResponse(ConstantResponseCode.RecordNotFound, EnumRespType.NotFound);

            db.AdminUsers.Remove(user); // soft delete (AuditableEntity)
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Result<bool>.SetResponse(ConstantResponseCode.Saved, EnumRespType.Success, true);
        }

        private Task<bool> RoleExistsAsync(long roleId, CancellationToken cancellationToken) =>
            db.Roles.AnyAsync(r => r.Id == roleId, cancellationToken);
    }
}
