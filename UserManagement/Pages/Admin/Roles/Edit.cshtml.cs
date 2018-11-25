using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace UserManagement.Pages.Admin.Roles
{
    [Authorize(Policy = "Roles-Edit")]
    public class EditModel : PageModel
    {
        private readonly RoleManager<Role> _roleManager;

        public EditModel(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        [Required]
        public string Name { get; set; }

        [BindProperty]
        [Required]
        public string Description { get; set; }

        [BindProperty]
        public IList<string> SelectedClaims { get; set; }

        public SelectList ClaimList { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get the role
            var role = await _roleManager.FindByIdAsync(id.ToString());
            Id = role.Id;
            Name = role.Name;
            Description = role.Description;

            // Get the claims for the role
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            SelectedClaims = new List<string>();
            foreach (var rc in roleClaims)
            {
                SelectedClaims.Add(rc.Type);
            }

            // Get all claims
            var ac = AllClaims.GetList();
            ClaimList = new SelectList(ac);

            // Return page
            if (role.Name == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Get the role and update the properties
            Role role = await _roleManager.FindByIdAsync(Id.ToString());
            role.Name = Name;
            role.Description = Description;

            // Save changes to the role
            await _roleManager.UpdateAsync(role);

            // Get all claims for the role
            var allClaims = await _roleManager.GetClaimsAsync(role);
            // Remove all claims for the role
            foreach (var claim in allClaims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            // Get the selected claims
            var sc = SelectedClaims;
            if (sc.Count != 0)
            {
                // Add the new claims to the role
                foreach (var c in sc)
                {
                    await _roleManager.AddClaimAsync(role, new Claim(c, ""));
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
