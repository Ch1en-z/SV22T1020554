
using SV22T1020554.DataLayers.Interfaces;
using SV22T1020554.DataLayers.SQLServer;
using SV22T1020554.Models.Security;

namespace SV22T1020554.BusinessLayers
{
    /// <summary>
    /// Cung c?p các ch?c nãng x? l? d? li?u liên quan ð?n tài kho?n ngý?i dùng
    /// </summary>
    public static class UserAccountService
    {
        private static readonly UserAccountRepository userAccountDB;

        static UserAccountService()
        {
            userAccountDB = new UserAccountRepository(Configuration.ConnectionString);
        }

        /// <summary>
        /// Xác th?c tài kho?n ðãng nh?p
        /// </summary>
        public static async Task<UserAccount?> AuthorizeAsync(string userName, string password)
        {
            return await userAccountDB.Authorize(userName, password);
        }

        /// <summary>
        /// Ð?i m?t kh?u
        /// </summary>
        public static async Task<bool> ChangePasswordAsync(string userName, string password)
        {
            return await userAccountDB.ChangePassword(userName, password);
        }

        /// <summary>
        /// Ðãng k? tài kho?n m?i
        /// </summary>
        public static async Task<int> RegisterAsync(string customerName, string contactName, string email, string province, string address, string phone, string password)
        {
            return await userAccountDB.Register(customerName, contactName, email, province, address, phone, password);
        }
    }
}
