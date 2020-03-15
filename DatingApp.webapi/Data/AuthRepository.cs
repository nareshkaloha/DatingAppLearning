using System;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.webapi.Model;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.webapi.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            this._context = context;
        }
        public async Task<User> Login(string username, string password)
        {
            var userFromDB = await _context.Users.FirstOrDefaultAsync(x=> x.UserName == username);

            if(userFromDB==null) return null;
           
            //if(!VerifyPassword(userFromDB.PasswordSalt, userFromDB.PasswordHash, password)) return null;
           
            return userFromDB;
        }
        private bool VerifyPassword(byte[] passwordSalt, byte[] passwordHash, string password)
        {
             using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var genPasswordHash=  hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));              

                for(int i=0; i< genPasswordHash.Length; i++)
                {
                    if(genPasswordHash[i] != passwordHash[i]) return false;
                }

                return true;
            }
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt ;
            
            CreateHashPassword(password, out passwordHash, out passwordSalt);

            // user.PasswordHash=passwordHash;
            // user.PasswordSalt =passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreateHashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash= hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }            
        }

        public async Task<bool> UserExists(string username)
        {
            return await  _context.Users.AnyAsync(x=> x.UserName == username);
        }
    }
}