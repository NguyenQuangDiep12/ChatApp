using ChatApp.Core.Services;
using ChatApp.Service.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRolePermissionService _rolePermissionService;

        public RolesController(IRolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        [HttpGet("{id}/permissions")]
        public async Task<IActionResult> GetPermissions(Guid id)
        {
            var rps = await _rolePermissionService.GetByRoleIdAsync(id);
            var dtos = rps.Select(rp => new PermissionDto(
                rp.Permission.Id, 
                rp.Permission.PermissionCode, 
                rp.Permission.Description, 
                rp.Permission.Scope.ToString()
            ));
            return Ok(dtos);
        }

        [HttpPost("{id}/permissions")]
        public async Task<IActionResult> GrantPermission(Guid id, [FromBody] GrantPermissionRequest request)
        {
            await _rolePermissionService.GrantPermissionAsync(id, request.PermissionId);
            return Ok(new { message = "Permission granted" });
        }

        [HttpDelete("{id}/permissions/{permissionId}")]
        public async Task<IActionResult> RevokePermission(Guid id, Guid permissionId)
        {
            await _rolePermissionService.RevokePermissionAsync(id, permissionId);
            return Ok(new { message = "Permission revoked" });
        }
    }
}
