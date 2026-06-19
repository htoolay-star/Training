using Contracts.Admin;
using Microsoft.AspNetCore.Mvc;
using Shared.Base;

namespace Train.Domain.Features.RoleFeature
{
    public class RoleController(IRoleService roleService) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
            Execute(await roleService.GetAllAsync(cancellationToken));

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken) =>
            Execute(await roleService.GetByIdAsync(id, cancellationToken));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleRequest request, CancellationToken cancellationToken) =>
            Execute(await roleService.CreateAsync(request, cancellationToken));

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateRoleRequest request, CancellationToken cancellationToken) =>
            Execute(await roleService.UpdateAsync(request, cancellationToken));

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken) =>
            Execute(await roleService.DeleteAsync(id, cancellationToken));
    }
}
