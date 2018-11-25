using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Managers;
using UserManagement.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UserManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite("Data Source=app.db"));

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<MyUserManager>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // These are the claims/permissions for user authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Users-View", policy => policy.RequireClaim("Users-View"));
                options.AddPolicy("Users-Edit", policy => policy.RequireClaim("Users-Edit"));
                options.AddPolicy("Users-Details", policy => policy.RequireClaim("Users-Details"));
                options.AddPolicy("Users-Delete", policy => policy.RequireClaim("Users-Delete"));
                options.AddPolicy("Roles-View", policy => policy.RequireClaim("Roles-View"));
                options.AddPolicy("Roles-Details", policy => policy.RequireClaim("Roles-Details"));
                options.AddPolicy("Roles-Edit", policy => policy.RequireClaim("Roles-Edit"));
                options.AddPolicy("Roles-Delete", policy => policy.RequireClaim("Roles-Delete"));
            });

            services.AddSingleton<IEmailSender, EmailSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            DataInitializer.SeedData(userManager, roleManager);
            app.UseMvc();
        }
    }
}
