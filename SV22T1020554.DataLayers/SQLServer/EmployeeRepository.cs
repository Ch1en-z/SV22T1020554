using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Security.Cryptography;
using System.Text;
using SV22T1020554.Models.HR;
using SV22T1020554.Models.Common;
using SV22T1020554.DataLayers.Interfaces;

namespace SV22T1020554.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu cho nhân viên trên SQL Server
    /// </summary>
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        /// <summary>
        /// Khởi tạo repository với chuỗi kết nối
        /// </summary>
        public EmployeeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Mã hóa mật khẩu bằng MD5 (Đồng bộ với UserAccountRepository)
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

        /// <summary>
        /// Bổ sung một nhân viên mới vào cơ sở dữ liệu
        /// </summary>
        public async Task<int> AddAsync(Employee data)
        {
            int id = 0;
            string hashedPassword = HashPassword(data.Password ?? "123456");

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"insert into Employees(FullName, BirthDate, Address, Phone, Email, Photo, IsWorking, Password, RoleNames)
                            values (@FullName, @BirthDate, @Address, @Phone, @Email, @Photo, @IsWorking, @Password, @RoleNames);
                            select @@identity;";

                var parameters = new
                {
                    FullName = data.FullName ?? "",
                    BirthDate = data.BirthDate,
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    Photo = data.Photo ?? "",
                    IsWorking = data.IsWorking,
                    Password = hashedPassword,
                    RoleNames = data.RoleNames ?? ""
                };

                id = await connection.ExecuteScalarAsync<int>(sql, parameters);
            }
            return id;
        }

        /// <summary>
        /// Xóa một nhân viên khỏi cơ sở dữ liệu
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "delete from Employees where EmployeeID = @EmployeeID";
                var parameters = new { EmployeeID = id };
                result = await connection.ExecuteAsync(sql, parameters) > 0;
            }
            return result;
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một nhân viên
        /// </summary>
        public async Task<Employee?> GetAsync(int id)
        {
            Employee? data = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "select * from Employees where EmployeeID = @EmployeeID";
                var parameters = new { EmployeeID = id };
                data = await connection.QueryFirstOrDefaultAsync<Employee>(sql, parameters);
            }
            return data;
        }

        /// <summary>
        /// Kiểm tra xem nhân viên hiện tại có dữ liệu liên quan (đơn hàng) hay không
        /// </summary>
        public async Task<bool> IsUsed(int id)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if exists(select * from Orders where EmployeeID = @EmployeeID)
                                select 1
                            else 
                                select 0";
                var parameters = new { EmployeeID = id };
                result = await connection.ExecuteScalarAsync<bool>(sql, parameters);
            }
            return result;
        }

        /// <summary>
        /// Tìm kiếm, đếm số lượng và lấy danh sách nhân viên phân trang
        /// </summary>
        public async Task<PagedResult<Employee>> ListAsync(PaginationSearchInput input)
        {
            string searchValue = input.SearchValue ?? "";
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = new SqlConnection(_connectionString))
            {
                var countSql = @"select count(*) from Employees 
                                 where (@searchValue = N'') or (FullName like @searchValue)";

                var listSql = @"select *
                                from (
                                    select *, row_number() over (order by FullName) as RowNumber
                                    from Employees 
                                    where (@searchValue = N'') or (FullName like @searchValue)
                                ) as t
                                where (@pageSize = 0) 
                                   or (t.RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                                order by t.RowNumber";

                var parameters = new
                {
                    page = input.Page,
                    pageSize = input.PageSize,
                    searchValue = searchValue
                };

                int rowCount = await connection.ExecuteScalarAsync<int>(countSql, new { searchValue = searchValue });
                var data = await connection.QueryAsync<Employee>(listSql, parameters);

                return new PagedResult<Employee>
                {
                    Page = input.Page,
                    PageSize = input.PageSize,
                    RowCount = rowCount,
                    DataItems = data.ToList()
                };
            }
        }

        /// <summary>
        /// Cập nhật thông tin của một nhân viên
        /// </summary>
        public async Task<bool> UpdateAsync(Employee data)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"update Employees 
                            set FullName = @FullName,
                                BirthDate = @BirthDate,
                                Address = @Address,
                                Phone = @Phone,
                                Email = @Email,
                                Photo = @Photo,
                                IsWorking = @IsWorking,
                                RoleNames = @RoleNames
                            where EmployeeID = @EmployeeID";

                var parameters = new
                {
                    FullName = data.FullName ?? "",
                    BirthDate = data.BirthDate,
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    Photo = data.Photo ?? "",
                    IsWorking = data.IsWorking,
                    RoleNames = data.RoleNames ?? "",
                    EmployeeID = data.EmployeeID
                };

                result = await connection.ExecuteAsync(sql, parameters) > 0;
            }
            return result;
        }

        /// <summary>
        /// Kiểm tra xem địa chỉ email có bị trùng với nhân viên khác hay không
        /// </summary>
        public async Task<bool> ValidateEmailAsync(string email, int id = 0)
        {
            bool isDuplicate = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if exists(select * from Employees where Email = @Email and EmployeeID <> @EmployeeID)
                                select 1
                            else 
                                select 0";
                var parameters = new
                {
                    Email = email ?? "",
                    EmployeeID = id
                };
                isDuplicate = await connection.ExecuteScalarAsync<bool>(sql, parameters);
            }
            return !isDuplicate;
        }
        /// <summary>
        /// Cập nhật mật khẩu cho nhân viên
        /// </summary>
        public async Task<bool> UpdatePasswordAsync(int id, string password)
        {
            string hashedPassword = HashPassword(password);
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "UPDATE Employees SET Password = @Password WHERE EmployeeID = @EmployeeID";
                var parameters = new { Password = hashedPassword, EmployeeID = id };
                return (await connection.ExecuteAsync(sql, parameters)) > 0;
            }
        }

        /// <summary>
        /// Cập nhật danh sách quyền cho nhân viên
        /// </summary>
        public async Task<bool> UpdateRoleNamesAsync(int id, string roleNames)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "UPDATE Employees SET RoleNames = @RoleNames WHERE EmployeeID = @EmployeeID";
                var parameters = new { RoleNames = roleNames, EmployeeID = id };
                return (await connection.ExecuteAsync(sql, parameters)) > 0;
            }
        }
    }
}
