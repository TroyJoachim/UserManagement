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
    [Authorize(Policy = "Roles-Details")]
    public class DetailsModel : PageModel
    {
        private readonly RoleManager<Role> _roleManager;

        public DetailsModel(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public IList<string> SelectedClaims { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get the role
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role.Name == null)
            {
                return NotFound();
            }

            // Add properties
            Id = role.Id;
            Name = role.Name;
            Description = role.Description;

            // Get and add the claims for the role
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            SelectedClaims = new List<string>();
            foreach (var rc in roleClaims)
            {
                SelectedClaims.Add(rc.Type);
            }

            return Page();
        }
    }
}
