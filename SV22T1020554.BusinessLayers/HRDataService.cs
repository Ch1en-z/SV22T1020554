using SV22T1020554.DataLayers.Interfaces;
using SV22T1020554.DataLayers.SQLServer;
using SV22T1020554.Models.Common;
using SV22T1020554.Models.HR;


namespace SV22T1020554.BusinessLayers
{
    /// <summary>
    /// Cung c?p các ch?c nãng x? l? d? li?u liên quan ð?n nhân s? c?a h? th?ng    
    /// </summary>
    public static class HRDataService
    {
        private static readonly IEmployeeRepository employeeDB;

        /// <summary>
        /// Constructor
        /// </summary>
        static HRDataService()
        {
            employeeDB = new EmployeeRepository(Configuration.ConnectionString);
        }

        #region Employee

        /// <summary>
        /// T?m ki?m và l?y danh sách nhân viên dý?i d?ng phân trang.
        /// </summary>
        /// <param name="input">
        /// Thông tin t?m ki?m và phân trang (t? khóa t?m ki?m, trang c?n hi?n th?, s? d?ng m?i trang).
        /// </param>
        /// <returns>
        /// K?t qu? t?m ki?m dý?i d?ng danh sách nhân viên có phân trang.
        /// </returns>
        public static async Task<PagedResult<Employee>> ListEmployeesAsync(PaginationSearchInput input)
        {
            return await employeeDB.ListAsync(input);
        }

        /// <summary>
        /// L?y thông tin chi ti?t c?a m?t nhân viên d?a vào m? nhân viên.
        /// </summary>
        /// <param name="employeeID">M? nhân viên c?n t?m.</param>
        /// <returns>
        /// Ð?i tý?ng Employee n?u t?m th?y, ngý?c l?i tr? v? null.
        /// </returns>
        public static async Task<Employee?> GetEmployeeAsync(int employeeID)
        {
            return await employeeDB.GetAsync(employeeID);
        }

        /// <summary>
        /// B? sung m?t nhân viên m?i vào h? th?ng.
        /// </summary>
        /// <param name="data">Thông tin nhân viên c?n b? sung.</param>
        /// <returns>M? nhân viên ðý?c t?o m?i.</returns>
        public static async Task<int> AddEmployeeAsync(Employee data)
        {
            //TODO: Ki?m tra d? li?u h?p l?
            return await employeeDB.AddAsync(data);
        }

        /// <summary>
        /// C?p nh?t thông tin c?a m?t nhân viên.
        /// </summary>
        /// <param name="data">Thông tin nhân viên c?n c?p nh?t.</param>
        /// <returns>
        /// True n?u c?p nh?t thành công, ngý?c l?i False.
        /// </returns>
        public static async Task<bool> UpdateEmployeeAsync(Employee data)
        {
            //TODO: Ki?m tra d? li?u h?p l?
            return await employeeDB.UpdateAsync(data);
        }

        /// <summary>
        /// Xóa m?t nhân viên d?a vào m? nhân viên.
        /// </summary>
        /// <param name="employeeID">M? nhân viên c?n xóa.</param>
        /// <returns>
        /// True n?u xóa thành công, False n?u nhân viên ðang ðý?c s? d?ng
        /// ho?c vi?c xóa không th?c hi?n ðý?c.
        /// </returns>
        public static async Task<bool> DeleteEmployeeAsync(int employeeID)
        {
            if (await employeeDB.IsUsed(employeeID))
                return false;

            return await employeeDB.DeleteAsync(employeeID);
        }

        /// <summary>
        /// Ki?m tra xem m?t nhân viên có ðang ðý?c s? d?ng trong d? li?u hay không.
        /// </summary>
        /// <param name="employeeID">M? nhân viên c?n ki?m tra.</param>
        /// <returns>
        /// True n?u nhân viên ðang ðý?c s? d?ng, ngý?c l?i False.
        /// </returns>
        public static async Task<bool> IsUsedEmployeeAsync(int employeeID)
        {
            return await employeeDB.IsUsed(employeeID);
        }

        /// <summary>
        /// Ki?m tra xem email c?a nhân viên có h?p l? không
        /// (không b? trùng v?i email c?a nhân viên khác).
        /// </summary>
        /// <param name="email">Ð?a ch? email c?n ki?m tra.</param>
        /// <param name="employeeID">
        /// N?u employeeID = 0: ki?m tra email ð?i v?i nhân viên m?i.
        /// N?u employeeID khác 0: ki?m tra email c?a nhân viên có m? là employeeID.
        /// </param>
        /// <returns>
        /// True n?u email h?p l? (không trùng), ngý?c l?i False.
        /// </returns>
        public static async Task<bool> ValidateEmployeeEmailAsync(string email, int employeeID = 0)
        {
            return await employeeDB.ValidateEmailAsync(email, employeeID);
        }

        /// <summary>
        /// C?p nh?t m?t kh?u cho nhân viên
        /// </summary>
        public static async Task<bool> ChangePasswordAsync(int id, string password)
        {
            return await employeeDB.UpdatePasswordAsync(id, password);
        }

        /// <summary>
        /// C?p nh?t danh sách quy?n cho nhân viên
        /// </summary>
        public static async Task<bool> ChangeRoleNamesAsync(int id, string roleNames)
        {
            return await employeeDB.UpdateRoleNamesAsync(id, roleNames);
        }

        #endregion
    }
}
