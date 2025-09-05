using Inventory.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Inventory.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ModulesController : ControllerBase
    {
        private readonly IModuleService _moduleService;

        public ModulesController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        [HttpGet("my-modules")]
        public async Task<IActionResult> GetMyModules()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var modules = await _moduleService.GetModulesByUserIdAsync(userId);

                return Ok(modules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener módulos: {ex.Message}");
            }
        }

        [HttpGet("has-permission/{permissionCode}")]
        public async Task<IActionResult> HasPermission(string permissionCode)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var hasPermission = await _moduleService.HasPermissionAsync(userId, permissionCode);

                return Ok(new { hasPermission });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al verificar permiso: {ex.Message}");
            }
        }
    }
}