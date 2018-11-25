using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace UserManagement.Pages.Admin.Users
{
    [Authorize(Policy = "Users-Details")]
    public class DetailsModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public DetailsModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public int Id { get; set; }

        public string UserName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public bool Enabled { get; set; }

        [Display(Name = "Roles")]
        public IList<string> SelectedRoles { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user.UserName == null)
            {
                return NotFound();
            }

            Id = user.Id;
            UserName = user.UserName;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            PhoneNumber = user.PhoneNumber;
            Enabled = user.Enabled;

            // Get the roles for the user
            var userRoles = await _userManager.GetRolesAsync(user);
            SelectedRoles = new List<string>();
            foreach (var userRole in userRoles)
            {
                SelectedRoles.Add(userRole);
            }

            return Page();
        }
    }
}
