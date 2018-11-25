using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace UserManagement.Pages.Admin.Roles
{
    [Authorize(Policy = "Roles-Delete")]
    public class DeleteModel : PageModel
    {
        private readonly RoleManager<Role> _roleManager;

        public DeleteModel(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        public string Name { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Role role = await _roleManager.FindByIdAsync(id.ToString());
            Id = role.Id;
            Name = role.Name;

            if (role.Name == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id.ToString());

            if (role.Name != null)
            {
                var deleteRoleResult = await _roleManager.DeleteAsync(role);
                if (!deleteRoleResult.Succeeded)
                {
                    return Page();
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
