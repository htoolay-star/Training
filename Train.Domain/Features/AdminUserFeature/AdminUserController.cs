using Contracts.Admin;
using Microsoft.AspNetCore.Mvc;
using Shared.Base;

namespace Train.Domain.Features.AdminUserFeature
{
    public class AdminUserController(IAdminUserService adminUserService) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
            Execute(await adminUserService.GetAllAsync(cancellationToken));

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken) =>
            Execute(await adminUserService.GetByIdAsync(id, cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAdminUserRequest request, CancellationToken cancellationToken) =>
            Execute(await adminUserService.CreateAsync(request, cancellationToken));

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateAdminUserRequest request, CancellationToken cancellationToken) =>
            Execute(await adminUserService.UpdateAsync(request, cancellationToken));

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken) =>
            Execute(await adminUserService.ResetPasswordAsync(request, cancellationToken));

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken) =>
            Execute(await adminUserService.DeleteAsync(id, cancellationToken));
    }
}
