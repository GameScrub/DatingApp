using System.Security.Cryptography;
using System.Text;

namespace DatingApp.API.Helpers
{
    public static class Hash
    {
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmack = new HMACSHA512())
            {
                passwordSalt = hmack.Key;
                passwordHash = hmack.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmack = new HMACSHA512(passwordSalt))
            {
                var ComputedHash = hmack.ComputeHash(Encoding.UTF8.GetBytes(password));

                for (var i = 0; i < ComputedHash.Length; i++)
                    if (ComputedHash[i] != passwordHash[i])
                        return false;
            }

            return true;
        }
    }
}