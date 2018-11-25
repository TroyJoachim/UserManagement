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
using UserManagement.Managers;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace UserManagement.Pages.Admin.Users
{
    [Authorize(Policy = "Users-Edit")]
    public class EditModel : PageModel
    {
        // Using custom UserManager
        private readonly MyUserManager _userManager;
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context, MyUserManager usersManager)
        {
            _userManager = usersManager;
            _context = context;
            vm = new UserEditViewModel();
        }

        [BindProperty]
        public UserEditViewModel vm { get; set; }

        [BindProperty]
        public IList<string> SelectedRoles { get; set; }

        [Display(Name = "Groups")]
        public SelectList RoleList { get; set; }

        public class UserEditViewModel
        {
            public int Id { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm New password")]
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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get the user
            var user = await _userManager.FindByIdAsync(id.ToString());

            // Populate the ViewModel
            vm = new UserEditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Enabled = user.Enabled
            };
            // Get the roles for the user
            var userRoles = await _userManager.GetRolesAsync(user);
            SelectedRoles = new List<string>();
            foreach (var userRole in userRoles)
            {
                SelectedRoles.Add(userRole);
            }

            // Get all the roles
            var roles = await _context.Role.ToListAsync();
            RoleList = new SelectList(roles);

            if (user.Email == null)
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

            // Get the user
            var user = await _userManager.FindByIdAsync(vm.Id.ToString());
            if (user.UserName == null)
            {
                return Page();
            }
            user.Email = vm.Email;
            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.PhoneNumber = vm.PhoneNumber;
            user.Enabled = vm.Enabled;

            // Update the user password if it was set
            if (vm.Password != null)
            {
                var passwordResetResult = await _userManager.ChangePasswordAsync(user, vm.Password);
                
                if (!passwordResetResult.Succeeded)
                {
                    // Add the errors to the ModelState
                    foreach (var error in passwordResetResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    // If we get here something went wrong
                    return Page();
                }
            }

            // Update the user
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Get the selected roles
                var selectedRoles = SelectedRoles;

                // Get all the roles
                var roles = await _context.Role.ToListAsync();
                var strRoles = roles.Select(r => r.Name);

                // Remove all the roles from the user
                await _userManager.RemoveFromRolesAsync(user, strRoles);

                // Add the UserRoles
                await _userManager.AddToRolesAsync(user, selectedRoles);
                

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
