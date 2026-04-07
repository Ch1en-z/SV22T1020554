
using SV22T1020554.BusinessLayers;
using SV22T1020554.DataLayers.Interfaces;
using SV22T1020554.DataLayers.SQLServer;
using SV22T1020554.Models.Common;
using SV22T1020554.Models.Partner;

/// <summary>
/// Cung c?p các ch?c nãng x? l? d? li?u liên quan ð?n các ð?i tác c?a h? th?ng
/// bao g?m: nhà cung c?p (Supplier), khách hàng (Customer) và ngý?i giao hàng (Shipper)
/// </summary>
public static class PartnerDataService
{
    private static readonly IGenericRepository<Supplier> supplierDB;
    private static readonly ICustomerRepository customerDB;
    private static readonly IGenericRepository<Shipper> shipperDB;

    /// <summary>
    /// Ctor
    /// </summary>
    static PartnerDataService()
    {
        supplierDB = new SupplierRepository(Configuration.ConnectionString);
        customerDB = new CustomerRepository(Configuration.ConnectionString);
        shipperDB = new ShipperRepository(Configuration.ConnectionString);
    }

    #region Supplier

    /// <summary>
    /// T?m ki?m và l?y danh sách nhà cung c?p dý?i d?ng phân trang.
    /// </summary>
    /// <param name="input">
    /// Thông tin t?m ki?m và phân trang (t? khóa t?m ki?m, trang c?n hi?n th?, s? d?ng m?i trang).
    /// </param>
    /// <returns>
    /// K?t qu? t?m ki?m dý?i d?ng danh sách nhà cung c?p có phân trang.
    /// </returns>
    public static async Task<PagedResult<Supplier>> ListSuppliersAsync(PaginationSearchInput input)
    {
        return await supplierDB.ListAsync(input);
    }

    /// <summary>
    /// L?y thông tin chi ti?t c?a m?t nhà cung c?p d?a vào m? nhà cung c?p.
    /// </summary>
    /// <param name="supplierID">M? nhà cung c?p c?n t?m.</param>
    /// <returns>
    /// Ð?i tý?ng Supplier n?u t?m th?y, ngý?c l?i tr? v? null.
    /// </returns>
    public static async Task<Supplier?> GetSupplierAsync(int supplierID)
    {
        return await supplierDB.GetAsync(supplierID);
    }

    /// <summary>
    /// B? sung m?t nhà cung c?p m?i vào h? th?ng.
    /// </summary>
    /// <param name="data">Thông tin nhà cung c?p c?n b? sung.</param>
    /// <returns>M? nhà cung c?p ðý?c t?o m?i.</returns>
    public static async Task<int> AddSupplierAsync(Supplier data)
    {
        //TODO: Ki?m tra d? li?u h?p l?
        return await supplierDB.AddAsync(data);
    }

    /// <summary>
    /// C?p nh?t thông tin c?a m?t nhà cung c?p.
    /// </summary>
    /// <param name="data">Thông tin nhà cung c?p c?n c?p nh?t.</param>
    /// <returns>
    /// True n?u c?p nh?t thành công, ngý?c l?i False.
    /// </returns>
    public static async Task<bool> UpdateSupplierAsync(Supplier data)
    {
        //TODO: Ki?m tra d? li?u h?p l?
        return await supplierDB.UpdateAsync(data);
    }

    /// <summary>
    /// Xóa m?t nhà cung c?p d?a vào m? nhà cung c?p.
    /// </summary>
    /// <param name="supplierID">M? nhà cung c?p c?n xóa.</param>
    /// <returns>
    /// True n?u xóa thành công, False n?u nhà cung c?p ðang ðý?c s? d?ng
    /// ho?c vi?c xóa không th?c hi?n ðý?c.
    /// </returns>
    public static async Task<bool> DeleteSupplierAsync(int supplierID)
    {
        if (await supplierDB.IsUsed(supplierID))
            return false;

        return await supplierDB.DeleteAsync(supplierID);
    }

    /// <summary>
    /// Ki?m tra xem m?t nhà cung c?p có ðang ðý?c s? d?ng trong d? li?u hay không.
    /// </summary>
    /// <param name="supplierID">M? nhà cung c?p c?n ki?m tra.</param>
    /// <returns>
    /// True n?u nhà cung c?p ðang ðý?c s? d?ng, ngý?c l?i False.
    /// </returns>
    public static async Task<bool> IsUsedSupplierAsync(int supplierID)
    {
        return await supplierDB.IsUsed(supplierID);
    }

    #endregion

    #region Customer

    /// <summary>
    /// T?m ki?m và l?y danh sách khách hàng dý?i d?ng phân trang.
    /// </summary>
    /// <param name="input">
    /// Thông tin t?m ki?m và phân trang.
    /// </param>
    /// <returns>
    /// Danh sách khách hàng phù h?p v?i ði?u ki?n t?m ki?m.
    /// </returns>
    public static async Task<PagedResult<Customer>> ListCustomersAsync(PaginationSearchInput input)
    {
        return await customerDB.ListAsync(input);
    }

    /// <summary>
    /// L?y thông tin chi ti?t c?a m?t khách hàng d?a vào m? khách hàng.
    /// </summary>
    /// <param name="customerID">M? khách hàng c?n t?m.</param>
    /// <returns>
    /// Ð?i tý?ng Customer n?u t?m th?y, ngý?c l?i tr? v? null.
    /// </returns>
    public static async Task<Customer?> GetCustomerAsync(int customerID)
    {
        return await customerDB.GetAsync(customerID);
    }

    /// <summary>
    /// B? sung m?t khách hàng m?i vào h? th?ng.
    /// </summary>
    /// <param name="data">Thông tin khách hàng c?n b? sung.</param>
    /// <returns>M? khách hàng ðý?c t?o m?i.</returns>
    public static async Task<int> AddCustomerAsync(Customer data)
    {
        //TODO: Ki?m tra d? li?u h?p l?
        return await customerDB.AddAsync(data);
    }

    /// <summary>
    /// C?p nh?t thông tin c?a m?t khách hàng.
    /// </summary>
    /// <param name="data">Thông tin khách hàng c?n c?p nh?t.</param>
    /// <returns>
    /// True n?u c?p nh?t thành công, ngý?c l?i False.
    /// </returns>
    public static async Task<bool> UpdateCustomerAsync(Customer data)
    {
        //TODO: Ki?m tra d? li?u h?p l?
        return await customerDB.UpdateAsync(data);
    }

    /// <summary>
    /// Xóa m?t khách hàng d?a vào m? khách hàng.
    /// </summary>
    /// <param name="customerID">M? khách hàng c?n xóa.</param>
    /// <returns>
    /// True n?u xóa thành công, False n?u khách hàng ðang ðý?c s? d?ng
    /// ho?c vi?c xóa không th?c hi?n ðý?c.
    /// </returns>
    public static async Task<bool> DeleteCustomerAsync(int customerID)
    {
        if (await customerDB.IsUsed(customerID))
            return false;

        return await customerDB.DeleteAsync(customerID);
    }

    /// <summary>
    /// Ki?m tra xem m?t khách hàng có ðang ðý?c s? d?ng trong d? li?u hay không.
    /// </summary>
    /// <param name="customerID">M? khách hàng c?n ki?m tra.</param>
    /// <returns>
    /// True n?u khách hàng ðang ðý?c s? d?ng, ngý?c l?i False.
    /// </returns>
    public static async Task<bool> IsUsedCustomerAsync(int customerID)
    {
        return await customerDB.IsUsed(customerID);
    }

    /// <summary>
    /// Ki?m tra xem email c?a khách hàng có h?p l? không
    /// </summary>
    /// <param name="email">Ð?a ch? email c?n ki?m tra</param>
    /// <param name="customerID">
    /// B?ng 0 n?u ki?m tra email ð?i v?i khách hàng m?i.
    /// Khác 0 n?u ki?m tra email c?a khách hàng có m? là <paramref name="customerID"/>
    /// </param>
    /// <returns></returns>
    public static async Task<bool> ValidateCustomerEmailAsync(string email, int customerID = 0)
    {
        return await customerDB.ValidateEmailAsync(email, customerID);
    }

    #endregion

    #region Shipper

    /// <summary>
    /// T?m ki?m và l?y danh sách ngý?i giao hàng dý?i d?ng phân trang.
    /// </summary>
    /// <param name="input">
    /// Thông tin t?m ki?m và phân trang.
    /// </param>
    /// <returns>
    /// Danh sách ngý?i giao hàng phù h?p v?i ði?u ki?n t?m ki?m.
    /// </returns>
    public static async Task<PagedResult<Shipper>> ListShippersAsync(PaginationSearchInput input)
    {
        return await shipperDB.ListAsync(input);
    }

    /// <summary>
    /// L?y thông tin chi ti?t c?a m?t ngý?i giao hàng d?a vào m? ngý?i giao hàng.
    /// </summary>
    /// <param name="shipperID">M? ngý?i giao hàng c?n t?m.</param>
    /// <returns>
    /// Ð?i tý?ng Shipper n?u t?m th?y, ngý?c l?i tr? v? null.
    /// </returns>
    public static async Task<Shipper?> GetShipperAsync(int shipperID)
    {
        return await shipperDB.GetAsync(shipperID);
    }

    /// <summary>
    /// B? sung m?t ngý?i giao hàng m?i vào h? th?ng.
    /// </summary>
    /// <param name="data">Thông tin ngý?i giao hàng c?n b? sung.</param>
    /// <returns>M? ngý?i giao hàng ðý?c t?o m?i.</returns>
    public static async Task<int> AddShipperAsync(Shipper data)
    {
        //TODO: Ki?m tra d? li?u h?p l?
        return await shipperDB.AddAsync(data);
    }

    /// <summary>
    /// C?p nh?t thông tin c?a m?t ngý?i giao hàng.
    /// </summary>
    /// <param name="data">Thông tin ngý?i giao hàng c?n c?p nh?t.</param>
    /// <returns>
    /// True n?u c?p nh?t thành công, ngý?c l?i False.
    /// </returns>
    public static async Task<bool> UpdateShipperAsync(Shipper data)
    {
        //TODO: Ki?m tra d? li?u h?p l?
        return await shipperDB.UpdateAsync(data);
    }

    /// <summary>
    /// Xóa m?t ngý?i giao hàng d?a vào m? ngý?i giao hàng.
    /// </summary>
    /// <param name="shipperID">M? ngý?i giao hàng c?n xóa.</param>
    /// <returns>
    /// True n?u xóa thành công, False n?u ngý?i giao hàng ðang ðý?c s? d?ng
    /// ho?c vi?c xóa không th?c hi?n ðý?c.
    /// </returns>
    public static async Task<bool> DeleteShipperAsync(int shipperID)
    {
        if (await shipperDB.IsUsed(shipperID))
            return false;

        return await shipperDB.DeleteAsync(shipperID);
    }

    /// <summary>
    /// Ki?m tra xem m?t ngý?i giao hàng có ðang ðý?c s? d?ng trong d? li?u hay không.
    /// </summary>
    /// <param name="shipperID">M? ngý?i giao hàng c?n ki?m tra.</param>
    /// <returns>
    /// True n?u ngý?i giao hàng ðang ðý?c s? d?ng, ngý?c l?i False.
    /// </returns>
    public static async Task<bool> IsUsedShipperAsync(int shipperID)
    {
        return await shipperDB.IsUsed(shipperID);
    }

    #endregion
}
