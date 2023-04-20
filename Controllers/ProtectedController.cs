using AuthApi.Models.Domain;
using AuthApi.Models.DTO;
using AuthApi.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ProtectedController : ControllerBase
    {
        //private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly DatabaseContext _context;

        public ProtectedController(IUserService userService, DatabaseContext context)
        {
            _userService = userService;
            this._context = context;
        }

        [HttpGet]
        public IActionResult GetData()
        {
            //var userId = _userManager.GetUserId(HttpContext.User);
            //var userClaims = HttpContext.User.Claims.ToList();
            //var username = User.Identity.Name;
            //var userEmail = User.FindFirstValue(ClaimTypes.Email);
            string msg = "Data from protected controller";
            return Ok(new Status(200, "Success", msg));
        }

        [HttpGet]
        public IActionResult userDetails()
        {
            var ab = User.Identities.ToList();
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = _userService.GetSingleUser(userEmail);
            var userinfo = result.Result;
            if (result is null)
            {
                return Ok(new Status(404, "User not exist", null));
            }
            if (userinfo.ProfileImage != null)
            {
                string img = string.Format("https://localhost:7184/Resources/ProfileImages/{0}", userinfo.ProfileImage);
                userinfo.ProfileImage = img;
            }
            return Ok(new Status(200, "Success", userinfo));

        }

        [HttpPost]
        public async Task<IActionResult> AddImageAsync([FromForm] UploadImage model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new Status(400, "Please pass all the Fields", null));
            }
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = _userService.AddImage(model.formFile, userEmail);
            if (result is null)
            {
                return Ok(new Status(400, "Error while adding image", null));
            }
            var userImageInstance = new UserImages
            {
                UserEmail = userEmail,
                UserImage = result.Result,
            };
            await _context.UserImages.AddAsync(userImageInstance);
            await _context.SaveChangesAsync();
            return Ok(new Status(200, "Image Added Successfully", null));
        }

        [HttpGet]
        public async Task<ActionResult<List<UserImages>>> ShowImages()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.AllUsers.SingleAsync(u => u.Email == userEmail);
            int userID = user.Id;
            var result = _userService.ShowImage(userEmail);
            if (result is null)
            {
                return Ok(new Status(400, "No image Found", null));
            }
            List<UserImageDto> resultList = new List<UserImageDto>();
            foreach (var res in result)
            {
                UserImageDto tempVar = new UserImageDto();
                tempVar.UserImage = res.UserImage;
                tempVar.Id = res.Id;

                resultList.Add(tempVar);
            }
            foreach (var res in resultList)
            {
                res.UserImage = string.Format("https://localhost:7184/Resources/{0}/{1}",userID.ToString(), res.UserImage);
            }

            return Ok(new Status(200, "User Images", resultList));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImageAsync(int id )
        {
            if (id==null)
            {
                return Ok(new Status(400, "Please pass an id of image", null));
            }
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var result = _userService.DeleteImage(id, userEmail);
            if (result.Result is null)
            {
                return Ok(new Status(400, "Error while deleting image", null));
            }
            return Ok(new Status(200,result.Result, null));
        }
    }
}
