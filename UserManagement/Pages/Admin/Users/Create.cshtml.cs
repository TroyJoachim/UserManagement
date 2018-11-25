using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using UserManagement.Data;
using UserManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.Pages.Admin.Users
{
    [Authorize(Policy = "Users-Create")]
    public class CreateModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly UserManagement.Data.ApplicationDbContext _context;

        public CreateModel(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public UserCreateViewModel vm { get; set; }

        [BindProperty]
        public IEnumerable<string> SelectedRoles { get; set; }

        [Display(Name = "Groups")]
        public SelectList RoleList { get; set; }

        public class UserCreateViewModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            public bool Enabled { get; set; }
        }

        public async Task<IActionResult> OnGet()
        {
            // Get all the roles
            List<Role> roles = await _context.Role.ToListAsync();
            RoleList = new SelectList(roles, "Name");

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Create a new user
            var newUser = new User
            {
                UserName = vm.Email,
                Email = vm.Email,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                PhoneNumber = vm.PhoneNumber,
                Enabled = vm.Enabled
            };

            // Add the new user
            var result = await _userManager.CreateAsync(newUser, vm.Password);
            if (result.Succeeded)
            {
                // Get the selected roles
                var selectedRoles = SelectedRoles;

                // Add the UserRoles
                await _userManager.AddToRolesAsync(newUser, selectedRoles);

                return RedirectToPage("./Index");
            }

            // Add the errors to the ModelState
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}