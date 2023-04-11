using AuthApi.Models.Domain;
using AuthApi.Models.DTO;
using AuthApi.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace AuthApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public IActionResult GetData()
        {
            var status = new Status();
            status.StatusCode = 1;
            status.Message = "Data from admin controller";
            return Ok(status);
        }

        [HttpGet]
        public async Task<ActionResult<List<AllUsers>>> GetAllUsers()
        {
            return await _adminService.GetAllUsers();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AllUsers>> GetSingleUser(int id)
        {
            var result = await _adminService.GetSingleUser(id);
            if (result is null)
            {
                return NotFound("User not exist");
            }
            return Ok(result);
        }
    }
}
