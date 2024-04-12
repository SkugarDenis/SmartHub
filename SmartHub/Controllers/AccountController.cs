using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartHub.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _conf;
        public AccountController(Microsoft.AspNetCore.Identity.UserManager<IdentityUser> userMgr,
            SignInManager<IdentityUser> signinMgr,
            IConfiguration conf,
            Microsoft.AspNetCore.Identity.RoleManager<IdentityRole> roleManager)
        {
            userManager = userMgr;
            signInManager = signinMgr;
            _conf = conf;
            _roleManager = roleManager;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    await signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        var SecretKey = _conf["JwtSettings:SecretKey"];
                        var Issuer = _conf["JwtSettings:Issuer"];
                        var Audience = _conf["JwtSettings:Audience"];

                        var token = GenerateToken(user.Id, user.UserName, SecretKey, Issuer, Audience);

                        return new JsonResult(new { Token = token });

                    }
                    ModelState.AddModelError(nameof(LoginViewModel.UserName), "Неверный логин или пароль");
                }
                ModelState.AddModelError(nameof(LoginViewModel.UserName), "Неверный логин или пароль");
            }

            return View(model);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangeCredentials(ChangeCredentialsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var user = await userManager.FindByIdAsync(userId);

                if (!model.NewPassword.Equals(model.NewPasswordSecond))
                {
                    return BadRequest();
                }

                if (user != null)
                {
                    var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View(model);
        }


        [Authorize]
        public IActionResult ChangeCredentials()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddOrEditUser()
        {
            var users = await userManager.Users.Where(x=>!x.UserName.Equals("admin")).Select(x=> new UserItem
            {
                 UserId= x.Id,
                 UserName= x.UserName
            }).ToListAsync();

            var roles = await _roleManager.Roles.Select(r => new RoleItem()
            {
                 Id= r.Id,
                 Name= r.Name
            }).ToListAsync();

            return View(new AddOrEditUserViewModel()
            {
                users = users,
                roles = roles
            });
        }

        [Authorize]
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddNewUser(AddNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await userManager.FindByNameAsync(model.UserName);

                if (existingUser != null)
                {
                    return BadRequest("Пользователь с таким именем уже существует");
                }

                var newUser = new IdentityUser { UserName = model.UserName };

                var result = await userManager.CreateAsync(newUser, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(AddOrEditUser));
                }
                else
                {
                    return BadRequest("Ошибка создания пользователя");
                }
            }

            return BadRequest("Неверные данные");
        }

        [Authorize]
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(DeleteUserViewModel model)
        {
            if (ModelState.IsValid)
            {

                var existingUser = await userManager.FindByIdAsync(model.UserId);

                if (existingUser != null)
                {
                    if (existingUser.UserName.Equals("admin"))
                    {
                        return BadRequest("Нельзя далить админа");
                    }

                    await userManager.DeleteAsync(existingUser);
                    return RedirectToAction(nameof(AddOrEditUser));
                }

                return BadRequest("Ошибка удаления пользователя");
            }

            return BadRequest("ModelStateInvalid");
        }


        public static string GenerateToken(string userId, string userName, string secretKey, string issuer, string audience, int expiryMinutes = 460)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.UniqueName, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

            return encodedToken;
        }
    }
}
