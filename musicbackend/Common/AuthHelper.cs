using System.Security.Cryptography;
using System.Text;

namespace musicbackend.Common
{
    public class AuthHelper
    {
        public string HashPassword(string password)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes = sha1.ComputeHash(Encoding.ASCII.GetBytes(password));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
