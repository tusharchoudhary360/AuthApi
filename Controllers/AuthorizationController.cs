using AuthApi.Models;
using AuthApi.Models.Domain;
using AuthApi.Models.DTO;
using AuthApi.Repositories.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace AuthApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IFileService _fileService;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ITokenService _tokenService;
        public AuthorizationController(DatabaseContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService,
            IEmailService emailService,
            IFileService fs
            )
        {
            this._context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this._tokenService = tokenService;
            this._emailService = emailService;
            this._fileService = fs;
        }


        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Ok(new Status(400, "user not exist", null));
            }
            if (!await userManager.CheckPasswordAsync(user, model.Password))
            {
                return Ok(new Status(400, "incorrect password", null));
            }
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);
                
                var token = _tokenService.GetToken(user,userRoles);
                var refreshToken = _tokenService.GetRefreshToken();
                var tokenInfo = _context.TokenInfo.FirstOrDefault(a => a.Usename == user.Email);
                if (tokenInfo == null)
                {
                    var info = new TokenInfo
                    {
                        Usename = user.Email,
                        RefreshToken = refreshToken,
                        RefreshTokenExpiry = DateTime.Now.AddDays(1)
                    };
                    _context.TokenInfo.Add(info);
                }

                else
                {
                    tokenInfo.RefreshToken = refreshToken;
                    tokenInfo.RefreshTokenExpiry = DateTime.Now.AddDays(1);
                }
                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

                return Ok(new Status(200, "Login Success",new LoginResponse
                {
                    Name = user.Name,
                    Username = user.UserName,
                    Email =user.Email,
                    Token = token.TokenString,
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo
                }));

            }
            //login failed condition
            return Ok(new Status(400, "Not able to login server error", null));
        }

        //registration for user

        [HttpPost]
        public async Task<IActionResult> Registration([FromForm] RegistrationModel model)
        {
            var status = new Status();
            if (!ModelState.IsValid)
            {
                return Ok(new Status(400, "Please pass all the Fields", null));
            }
            // check if user exists
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                return Ok(new Status(400, "Username already taken", null));
            }
            var emailExist = await userManager.FindByEmailAsync(model.Email);
            if (emailExist != null)
            {
                return Ok(new Status(400, "Email already register", null));
            }
            if (model.ImageFile == null)
            {
                return Ok(new Status(400, "Please Upload an Image", null));
            }
            var user = new ApplicationUser
            {
                UserName = model.Username,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = model.Email,
                Name = model.Name
            };
            // create a user here
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                string msg = result.ToString();
                return Ok(new Status(200,msg, null));
            }

            // add roles here
            // for admin registration UserRoles.Admin instead of UserRoles.Roles
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await roleManager.RoleExistsAsync(UserRoles.User))
            {
                await userManager.AddToRoleAsync(user, UserRoles.User);
            }
            var fileResult = _fileService.SaveImage(model.ImageFile);

            if (fileResult.Item1 == 1)
            {
                model.ProfileImage = fileResult.Item2;
            }
            var alluserInstance = new AllUsers
            {
                Username = model.Username,
                Name = model.Name,
                Email =model.Email,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                Country =model.Country,
                Hobby = (Models.Domain.Hobby)model.Hobby,
                ProfileImage =model.ProfileImage,
            };

            await _context.AllUsers.AddAsync(alluserInstance);
            await _context.SaveChangesAsync();
            return Ok(new Status(200, "Successfully registered", null));

        }

        // registration Admin
        [HttpPost]
        public async Task<IActionResult> RegistrationAdmin([FromBody] RegistrationModel model)
        {
            var status = new Status();
            if (!ModelState.IsValid)
            {
                return Ok(new Status(400, "Please pass all the Fields", null));
            }
            // check if user exists
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                return Ok(new Status(400, "Username already taken", null));
            }
            var emailExist = await userManager.FindByEmailAsync(model.Email);
            if (emailExist != null)
            {
                return Ok(new Status(400, "Email already register", null));
            }
            var user = new ApplicationUser
            {
                UserName = model.Username,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = model.Email,
                Name = model.Name
            };
            // create a user here
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return Ok(new Status(400, "User creation failed", null));
            }

            // add roles here
            // for admin registration UserRoles.Admin instead of UserRoles.Roles
            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            if (await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            return Ok(new Status(200, "Successfully registered", null));

        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var status = new Status();
            // check validations
            if (!ModelState.IsValid)
            {
                return Ok(new Status(400, "please pass all the valid fields", null));
            }
            // lets find the user
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                return Ok(new Status(400, "invalid email", null));
            }

            // check current password
            if (!await userManager.CheckPasswordAsync(user, model.CurrentPassword))
            {
                return Ok(new Status(400, "invalid current password", null));
            }

            // change password here
            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return Ok(new Status(400, "Failed to change password", null));
            }
            return Ok(new Status(200, "Password has changed successfully", result));
        }

        [HttpPost]
        public IActionResult SendEmail([FromForm] Message message)
        {
            _emailService.SendEmail(message);
            return Ok(new Status(200, "Email Send Success", null));
        }
    }
}
