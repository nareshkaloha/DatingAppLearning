using System.Collections.Generic;
using System.Linq;
using DatingApp.webapi.Model;
using Newtonsoft.Json;

namespace DatingApp.webapi.Data
{
    public static class Seed
    {
        public static void UserSeed(DataContext context)
        {
            if(!context.Users.Any())
            {
                 var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                foreach(var usr in users)
                {
                    byte[] passwordHash, passwordSalt ;
                    CreateHashPassword("password", out passwordHash, out passwordSalt);
                    
                    usr.PasswordHash = passwordHash;
                    usr.PasswordSalt = passwordSalt;
                    usr.Username = usr.Username.ToLower();               
                    context.Users.Add(usr);
                }
                context.SaveChanges();
            }
        }  

        private static void CreateHashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash= hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }            
        }      
    }
}