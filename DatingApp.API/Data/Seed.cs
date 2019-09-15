using System.Collections.Generic;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Linq;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly UserManager<User> _userManger;
        private readonly RoleManager<Role> _roleManager;

        public Seed(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
            _userManger = userManager;

        }

        public void SeedUsers()
        {
            if (!_userManger.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                var roles = new List<Role>
                {
                    new Role{Name = "Member"},
                    new Role{Name = "Admin"},
                    new Role{Name = "Moderator"},
                    new Role{Name = "VIP"}
                };

                foreach (var role in roles)
                {
                    _roleManager.CreateAsync(role).Wait();
                }

                foreach (var user in users)
                {
                    user.Photos.SingleOrDefault().IsApproved = true;
                    _userManger.CreateAsync(user, "password").Wait();
                    _userManger.AddToRoleAsync(user, "Member").Wait();
                }

                var adminUser = new User
                {
                    UserName = "Admin"
                };

                IdentityResult result = _userManger.CreateAsync(adminUser, "password").Result;

                if (result.Succeeded)
                {
                    var admin = _userManger.FindByNameAsync("Admin").Result;
                    _userManger.AddToRolesAsync(admin, new[] {"Admin", "Moderator"}).Wait();
                }
            }
        }
    }
}