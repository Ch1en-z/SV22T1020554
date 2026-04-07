using System.Security.Cryptography;
using System.Text;

namespace SV22T1020554.Admin
{
    /// <summary>
    /// L?p cung c?p các hàm ti?n ích s? d?ng cho m? hóa
    /// </summary>
    public static class CryptHelper
    {
        /// <summary>
        /// M? hóa MD5 m?t chu?i
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string HashMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
