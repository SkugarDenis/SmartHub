using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartHub.Models;

namespace SmartHub.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {

        private readonly RoleManager<IdentityRole> _roleManager;

        public SettingsController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }


        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View();
        }


        [Authorize]
        public async Task<IActionResult> CreateRole()
        {
            var roles = _roleManager.Roles.Where(x=>!x.Name.Equals("admin")).Select(r => new RoleItem
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();

            var model = new CreateRoleViewModel
            {
                Roles = roles
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {

            if (roleName.Equals("admin"))
            {
                ModelState.AddModelError(string.Empty, "Нельзя удалить админа");
                return View("Error");
            }

            if (_roleManager.Roles.Any(x=>x.Name.Equals(roleName)))
            {
                ModelState.AddModelError(string.Empty, "Эта роль уже существует");
                return View("Error");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                ModelState.AddModelError(string.Empty, "Имя роли не может быть пустым");
                return View("Error");
            }

            var role = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                var roles = _roleManager.Roles.Where(x => !x.Name.Equals("admin")).Select(r => new RoleItem
                {
                    Id = r.Id,
                    Name = r.Name
                }).ToList();

                var model = new CreateRoleViewModel
                {
                    Roles = roles
                };

                return View(model);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("Error");
            }
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("Error");
            }
        }

    }
}
