using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartHub.DataContext;
using SmartHub.DataContext.DbModels;
using SmartHub.Models;

namespace SmartHub.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataDbContext _dataDbContext;

        public SettingsController(RoleManager<IdentityRole> roleManager, DataDbContext dataDbContext)
        {
            _roleManager = roleManager;
            _dataDbContext = dataDbContext;
        }


        [Authorize]
        public async Task<IActionResult> Index()
        {
            var groups = new List<GroupWithRolesViewModel>();

            foreach (var groupEntity in _dataDbContext.GroupEntities.Include(x=>x.Roles))
            {
                var groupWithRoles = new GroupWithRolesViewModel
                {
                    GroupId = groupEntity.Id,
                    GroupName = groupEntity.Name,
                    Roles = new List<RoleViewModel>()
                };


                if (groupEntity.Roles is not null)
                {
                    foreach (var roleId in groupEntity.Roles.Select(x => x.IdRole))
                    {
                        var role = await _roleManager.FindByIdAsync(roleId);
                        if (role != null)
                        {
                            groupWithRoles.Roles.Add(new RoleViewModel
                            {
                                RoleId = role.Id,
                                RoleName = role.Name
                            });
                        }
                    }
                }

                groups.Add(groupWithRoles);
            }

            // Получение списка всех ролей и передача его в ViewBag
            ViewBag.Roles = _roleManager.Roles.Select(r => new SelectListItem { Value = r.Id, Text = r.Name }).ToList();

            return View(groups);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddRolesToGroup(int groupId, string[] selectedRoles)
        {
            var group = await _dataDbContext.GroupEntities.Include(g => g.Roles).FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null)
            {
                return NotFound();
            }

            if (selectedRoles != null)
            {
                foreach (var roleId in selectedRoles)
                {
                    var role = await _roleManager.FindByIdAsync(roleId);
                    if (role != null)
                    {
                        if (!group.Roles.Any(r => r.IdRole == roleId))
                        {
                            var relationship = new RelationshipGroupAndRole
                            {
                                idGroup = groupId,
                                NameGroup = group.Name,
                                IdRole = roleId
                            };

                            if (group.Roles == null)
                            {
                                group.Roles = new List<RelationshipGroupAndRole>();
                            }
                            group.Roles.Add(relationship);
                        }
                    }
                }

                await _dataDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RemoveRoleFromGroup(int groupId, string roleId)
        {
            var group = await _dataDbContext.GroupEntities.Include(x => x.Roles)
                                                .FirstOrDefaultAsync(x => x.Id == groupId);

            if (group == null)
            {
                return NotFound();
            }

            var relationshipToRemove = group.Roles.FirstOrDefault(r => r.idGroup == groupId && r.IdRole == roleId);
            if (relationshipToRemove != null)
            {
                group.Roles.Remove(relationshipToRemove);
                await _dataDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "admin")]
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

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
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
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role.Name.Equals("admin"))
            {
                ModelState.AddModelError(string.Empty, "Нельзя удалить админа");
                return View("Error");
            }

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

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateGroup()
        {
            var model = new CreateGroupViewModel()
            {
                groups = await _dataDbContext.GroupEntities
                    .Select(x => new GroupItem()
                    {
                        id = x.Id,
                        name = x.Name,
                    })
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateGroup(string groupName)
        {
            if (_dataDbContext.GroupEntities.AsNoTracking().Any(x => x.Name.Equals(groupName)))
            {
                return View("Error");
            }

            var newGroup = new DataContext.DbModels.GroupEntity()
            {
                Name = groupName
            };

            _dataDbContext.GroupEntities.Add(newGroup);

            await _dataDbContext.SaveChangesAsync();

            var model = new CreateGroupViewModel()
            {
                groups = await _dataDbContext.GroupEntities.AsNoTracking().Select(x => new GroupItem()
                {
                    id = x.Id,
                    name = x.Name,
                }).ToListAsync()
            };

            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteGroup(int Id)
        {
            var groupToDelete = await _dataDbContext.GroupEntities.FindAsync(Id);

            if (groupToDelete == null)
            {
                return View("Error");
            }

            var relationshipsToDelete = _dataDbContext.RelationshipGroupsAndroles
                .Where(r => r.idGroup == Id);

            _dataDbContext.RelationshipGroupsAndroles.RemoveRange(relationshipsToDelete);
            _dataDbContext.GroupEntities.Remove(groupToDelete);
            await _dataDbContext.SaveChangesAsync();

            var model = new CreateGroupViewModel()
            {
                groups = await _dataDbContext.GroupEntities.AsNoTracking().Select(x => new GroupItem()
                {
                    id = x.Id,
                    name = x.Name,
                }).ToListAsync()
            };

            return RedirectToAction("CreateGroup");
        }


    }
}
