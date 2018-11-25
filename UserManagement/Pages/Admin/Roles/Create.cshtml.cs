using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace UserManagement.Pages.Admin.Roles
{
    [Authorize(Policy = "Roles-Create")]
    public class CreateModel : PageModel
    {
        private readonly RoleManager<Role> _roleManager;

        public CreateModel(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Description { get; set; }

        [BindProperty]
        public IEnumerable<string> SelectedClaims { get; set; }

        public SelectList ClaimList { get; set; }

        public IActionResult OnGet()
        {
            var cList = AllClaims.GetList();

            ClaimList = new SelectList(cList);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Role role = new Role{ Name = Name, Description = Description};

            await _roleManager.CreateAsync(role);

            // Add selected claims to the new role
            foreach (var claim in SelectedClaims)
            {
                await _roleManager.AddClaimAsync(role, new Claim(claim, ""));
            }

            return RedirectToPage("./Index");
        }
    }
}