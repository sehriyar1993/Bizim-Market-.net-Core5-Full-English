using Borsa.Entities;
using Borsa.WebUI.Identity;
using Borsa.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Borsa.WebUI.Controllers
{
	// [Authorize(Roles = "Admin")]
	[AllowAnonymous]

	public class AdminRoleController : Controller
    {
        private RoleManager<IdentityRole> roleManager;
        private UserManager<ApplicationUser> userManager;
        public AdminRoleController(RoleManager<IdentityRole> _roleManager, UserManager<ApplicationUser> _userManager)
        {
            roleManager = _roleManager;
            userManager = _userManager;
        }
        public IActionResult RoleList()
        {
            var values = roleManager.Roles.ToList();
            return View(values);
        }
        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = model.Name
                };
                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult UpdateRole(string id)
        {
            var value = roleManager.Roles.FirstOrDefault(x => x.Id == id);
            if (value == null)
                return RedirectToAction("RoleList");
            RoleUpdateViewModel model = new RoleUpdateViewModel
            {
                Id = value.Id,
                Name = value.Name
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateRole(RoleUpdateViewModel model)
        {
            var value = roleManager.Roles.FirstOrDefault(x => x.Id == model.Id);
            value.Name = model.Name;
            value.Id = model.Id;
            var result = await roleManager.UpdateAsync(value);
            if (result.Succeeded)
            {
                return RedirectToAction("RoleList");
            }
            return View(model);
        }
        public async Task<IActionResult> DeleteRole(string id)
        {
            var value = roleManager.Roles.FirstOrDefault(x => x.Id == id);
            var result = await roleManager.DeleteAsync(value);
            if (result.Succeeded)
            {
                return RedirectToAction("RoleList");
            }
            return View();
        }
        public IActionResult UserRoleList()
        {
            var values = userManager.Users.ToList();
            return View(values);
        }
        [HttpGet]
        public async Task<IActionResult> AssignRole(string id)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            var roles = await roleManager.Roles.ToListAsync();
            var userRoles = await userManager.GetRolesAsync(user);
            List<RoleAssignViewModel> model = new List<RoleAssignViewModel>();
            foreach (var role in roles)
            {
                RoleAssignViewModel roleAssignViewModel = new RoleAssignViewModel();
                roleAssignViewModel.RoleId = role.Id;
                roleAssignViewModel.Name = role.Name;
                roleAssignViewModel.Exists = userRoles.Contains(role.Name);
                model.Add(roleAssignViewModel);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AssignRole(string id, List<RoleAssignViewModel> model)
        {
            var user = userManager.Users.FirstOrDefault(x => x.Id == id);
            foreach (var item in model)
            {
                if (item.Exists)
                {
                    await userManager.AddToRoleAsync(user, item.Name);
                }
                else
                {
                    await userManager.RemoveFromRoleAsync(user, item.Name);
                }
            }
            return RedirectToAction("UserRoleList");
        }
        public IActionResult Index()
        {

            return View(roleManager.Roles);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (ModelState.IsValid)
            {
                var result = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var x in result.Errors)
                    {
                        ModelState.AddModelError("", x.Description);
                    }
                }
            }
            return View(name);
        }


        public async Task<IActionResult> Delete(IdentityRole model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            await roleManager.DeleteAsync(role);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);

            var members = new List<ApplicationUser>();
            var nonmembers = new List<ApplicationUser>();

            foreach (var user in userManager.Users)
            {
                var list = await userManager.IsInRoleAsync(user, role.Name) ? members : nonmembers;
                list.Add(user);
            }

            var model = new RoleDetails()
            {
                Role = role,
                Members = members,
                NonMembers = nonmembers
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(RoleEditModel model)
        {
            IdentityResult result;

            if (ModelState.IsValid)
            {
                foreach (var userId in model.IdsToAdd ?? new string[] { })
                {
                    var user = await userManager.FindByIdAsync(userId);

                    if (user != null)
                    {
                        result = await userManager.AddToRoleAsync(user, model.RoleName);

                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }

                }

                foreach (var userId in model.IdsToDelete ?? new string[] { })
                {
                    var user = await userManager.FindByIdAsync(userId);

                    if (user != null)
                    {
                        result = await userManager.RemoveFromRoleAsync(user, model.RoleName);

                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }
            }
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Edit", model.RoleId);
            }
        }
    }
}
