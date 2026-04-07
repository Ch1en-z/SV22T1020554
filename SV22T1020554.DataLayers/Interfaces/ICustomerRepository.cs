using SV22T1020554.Models.Partner;

namespace SV22T1020554.DataLayers.Interfaces
{
    /// <summary>
    /// Ð?nh ngh?a các phép x? l? d? li?u trên Customer
    /// </summary>
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        /// <summary>
        /// Ki?m tra xem m?t ð?a ch? email có h?p l? hay không?
        /// </summary>
        /// <param name="email">Email c?n ki?m tra</param>
        /// <param name="id">
        /// N?u id = 0: Ki?m tra email c?a khách hàng m?i.
        /// N?u id <> 0: Ki?m tra email ð?i v?i khách hàng ð? t?n t?i
        /// </param>
        /// <returns></returns>
        Task<bool> ValidateEmailAsync(string email, int id = 0);
    }
}
