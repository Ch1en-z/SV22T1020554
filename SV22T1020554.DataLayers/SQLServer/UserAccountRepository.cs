using Dapper;
using Microsoft.Data.SqlClient;
using SV22T1020554.DataLayers.Interfaces;
using SV22T1020554.Models.Security;
using System.Security.Cryptography;
using System.Text;

namespace SV22T1020554.DataLayers.SQLServer
{
    /// <summary>
    /// Xử lý dữ liệu tài khoản người dùng (khách hàng) trên SQL Server
    /// </summary>
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly string _connectionString;

        public UserAccountRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Mã hóa mật khẩu bằng MD5
        /// </summary>
        private string HashPassword(string password)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes).ToLower();
            }
        }

        public async Task<UserAccount?> Authorize(string userName, string password)
        {
            UserAccount? result = null;
            string hashedPassword = HashPassword(password);

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT  CAST(e.EmployeeID AS NVARCHAR) AS UserId,
                                    e.Email AS UserName,
                                    e.FullName AS DisplayName,
                                    e.Email AS Email,
                                    e.Photo AS Photo,
                                    e.RoleNames AS RoleNames
                            FROM    Employees AS e
                            WHERE   e.Email = @UserName
                                AND e.Password = @Password
                                AND e.IsWorking = 1
                            UNION ALL
                            SELECT  CAST(c.CustomerID AS NVARCHAR) AS UserId,
                                    c.Email AS UserName,
                                    c.CustomerName AS DisplayName,
                                    c.Email AS Email,
                                    '' AS Photo,
                                    'customer' AS RoleNames
                            FROM    Customers AS c
                            WHERE   c.Email = @UserName
                                AND c.Password = @Password
                                AND (c.IsLocked IS NULL OR c.IsLocked = 0)";

                var parameters = new
                {
                    UserName = userName,
                    Password = hashedPassword
                };

                result = await connection.QueryFirstOrDefaultAsync<UserAccount>(sql, parameters);
            }

            return result;
        }

        public async Task<bool> ChangePassword(string userName, string password)
        {
            string hashedPassword = HashPassword(password);

            using (var connection = new SqlConnection(_connectionString))
            {
                // Cập nhật cho cả Nhân viên và Khách hàng dựa trên UserName (thường là Email)
                var sql = @"UPDATE Employees SET Password = @Password WHERE Email = @UserName;
                            UPDATE Customers SET Password = @Password WHERE Email = @UserName;";
                
                var parameters = new
                {
                    UserName = userName ?? "",
                    Password = hashedPassword
                };

                int rowsAffected = await connection.ExecuteAsync(sql, parameters);
                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Đăng ký tài khoản khách hàng mới
        /// </summary>
        public async Task<int> Register(string customerName, string contactName, string email, string province, string address, string phone, string password)
        {
            int result = 0;
            string hashedPassword = HashPassword(password);

            using (var connection = new SqlConnection(_connectionString))
            {
                // Check if email already exists
                var checkSql = "SELECT COUNT(*) FROM Customers WHERE Email = @Email";
                int count = await connection.ExecuteScalarAsync<int>(checkSql, new { Email = email });
                if (count > 0)
                    return -1; // Email already exists

                var sql = @"INSERT INTO Customers(CustomerName, ContactName, Province, Address, Phone, Email, Password, IsLocked)
                            VALUES(@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @Password, 0);
                            SELECT SCOPE_IDENTITY();";

                var parameters = new
                {
                    CustomerName = customerName ?? "",
                    ContactName = contactName ?? "",
                    Province = province ?? "",
                    Address = address ?? "",
                    Phone = phone ?? "",
                    Email = email ?? "",
                    Password = hashedPassword
                };

                result = await connection.ExecuteScalarAsync<int>(sql, parameters);
            }

            return result;
        }
    }
}
