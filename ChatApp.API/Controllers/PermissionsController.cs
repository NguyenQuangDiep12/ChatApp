using ChatApp.Core.Services;
using ChatApp.Service.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var permissions = await _permissionService.GetAllAsync();
            var dtos = permissions.Select(p => new PermissionDto(p.Id, p.PermissionCode, p.Description, p.Scope.ToString()));
            return Ok(dtos);
        }

        [HttpGet("scope/{scope}")]
        public async Task<IActionResult> GetByScope(string scope)
        {
            var permissions = await _permissionService.GetByScopeAsync(scope);
            var dtos = permissions.Select(p => new PermissionDto(p.Id, p.PermissionCode, p.Description, p.Scope.ToString()));
            return Ok(dtos);
        }
    }
}
