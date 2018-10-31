using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Repository
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
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username.ToLower());

            if(user == null)
            {
                return null;
            }
             
            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }
            
            return user;
        }

        public async Task<bool> UserExists(string username)
        {
            
            if(await _context.Users.AnyAsync(x => x.Username == username.ToLower()))
            {
                return true;
            }
            
            return false;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash;
            byte[] passwordSalt;

            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Username = user.Username.ToLower();

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmack = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmack.Key;
                passwordHash = hmack.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
             using(var hmack = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var ComputedHash = hmack.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for(int i=0; i<ComputedHash.Length; i++)
                {
                    if(ComputedHash[i] != passwordHash[i]) 
                    {
                        return false;
                    }
                }
            }

            return true;
        }
      
    }
}