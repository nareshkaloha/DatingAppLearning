using System.Collections.Generic;
using System.Linq;
using DatingApp.webapi.Model;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DatingApp.webapi.Data
{
    public static class Seed
    {
        // public static void UserSeed(DataContext context)
        // {
        //     if(!context.Users.Any())
        //     {
        //          var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
        //         var users = JsonConvert.DeserializeObject<List<User>>(userData);

        //         foreach(var usr in users)
        //         {
        //             byte[] passwordHash, passwordSalt ;
        //             CreateHashPassword("password", out passwordHash, out passwordSalt);
                    
        //             // usr.PasswordHash = passwordHash;
        //             // usr.PasswordSalt = passwordSalt;
        //             usr.UserName = usr.UserName.ToLower();               
        //             context.Users.Add(usr);
        //         }
        //         context.SaveChanges();
        //     }
        // }  

        private static void CreateHashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash= hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }            
        }    

        //refactored after using aspnet core identity ..

        public static void UserSeed(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            if(!userManager.Users.Any())
            {
                 var roles = new List<Role>()
                {
                    new Role{Name = "Member"},
                    new Role{Name = "VIP"},
                    new Role{Name = "Moderator"},
                    new Role{Name = "Admin"}
                };

                foreach (var role in roles)
                {
                    roleManager.CreateAsync(role).Wait();
                }

                 var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                foreach(var usr in users)
                {
                    userManager.CreateAsync(usr, "p@ssword!").Wait();
                    userManager.AddToRoleAsync(usr, "Member").Wait();
                }
            }

            //create an admin user
            var user = new User() {UserName = "Admin"};
            var userResult  = userManager.CreateAsync(user, "p@ssword!").Result;

            if(userResult.Succeeded)
            {
                var admin = userManager.FindByNameAsync("Admin").Result;
                userManager.AddToRolesAsync(admin, new [] {"Admin", "Moderator"}).Wait();
            }
        }   
    }
}