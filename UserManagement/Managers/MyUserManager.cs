using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace UserManagement.Managers
{
    // This class was created to extend the UserManager
    public class MyUserManager : UserManager<User>
    {
        public MyUserManager(
                IUserStore<User> store, 
                IOptions<IdentityOptions> optionsAccessor, 
                IPasswordHasher<User> passwordHasher, 
                IEnumerable<IUserValidator<User>> userValidators, 
                IEnumerable<IPasswordValidator<User>> passwordValidators,
                ILookupNormalizer keyNormalizer,
                IdentityErrorDescriber errors,
                IServiceProvider services,
                ILogger<UserManager<User>> logger
            ) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {    
        }

        // Added method so the administrators can manually reset user passwords
        public async Task<IdentityResult> ChangePasswordAsync(User user, string newPassword)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (newPassword == null)
                throw new ArgumentNullException(nameof(newPassword));

            return await UpdatePasswordHash(user, newPassword, true);

        } 
    }
}
