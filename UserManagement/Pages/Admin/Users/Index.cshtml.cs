using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Identity;

namespace UserManagement.Pages.Admin.Users
{
    [Authorize(Policy = "Users-View")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public IndexModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public IList<UserIndexViewModel> IndexModelList { get; set; }

        public class UserIndexViewModel
        {
            public int Id { get; set; }

            public string Username { get; set; }

            public string Email { get; set; }

            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }
        }

        public async Task OnGetAsync()
        {
            // Get all the users
            var users = await _userManager.Users.ToListAsync();

            // Add the users to the PageModel
            IndexModelList = new List<UserIndexViewModel>();
            foreach (var u in users)
            {
                IndexModelList.Add(new UserIndexViewModel { Id = u.Id, Username = u.UserName, Email = u.Email, FirstName = u.FirstName, LastName = u.LastName});
            }
        }
    }
}
