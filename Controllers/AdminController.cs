using AuthApi.Models.Domain;
using AuthApi.Models.DTO;
using AuthApi.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhotosWebApp.Models;
using System.Data;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;

namespace AuthApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly UserManager<ApplicationUser> userManager;
     
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
            userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetDataAsync()
        {
            var userClaims = HttpContext.User.Claims.ToList();
            string msg = "Data from admin controller";
            return Ok(new Status(200, "Success", msg));
        }

        [HttpGet]
        public async Task<ActionResult<List<AllUsers>>> GetAllUsers()
        {
            var result = await _adminService.GetAllUsers();
            foreach (AllUsers res in result)
            {
                if (res.ProfileImage != null)
                {
                    string img = $"{enums.apiUrl}/Resources/ProfileImages/{res.ProfileImage}";
                    res.ProfileImage = img;
                }
            }
            return Ok(new Status(200, "Success", result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AllUsers>> GetSingleUser(JustId model)
        {
            var result = await _adminService.GetSingleUser(model.id);
            if (result is null)
            {
                return Ok(new Status(404, "User not exist",null));
            }
            if (result.ProfileImage != null)
            {
                 string img = $"{enums.apiUrl}/Resources/ProfileImages/{result.ProfileImage}";
                 result.ProfileImage = img;
            }
            return Ok(new Status(200, "Success", result));
        }
    }
}
