using Microsoft.Data.SqlClient;
using Dapper;
using SV22T1020554.Models.Sales;
using SV22T1020554.DataLayers.Interfaces;

namespace SV22T1020554.BusinessLayers
{
    /// <summary>
    /// Service cung cấp các chức năng thống kê dashboard cho trang quản trị
    /// </summary>
    public static class DashboardService
    {
        /// <summary>
        /// Lấy số liệu thống kê tổng quan cho dashboard
        /// </summary>
        public static async Task<DashboardStatistics> GetStatisticsAsync()
        {
            using var connection = new SqlConnection(Configuration.ConnectionString);
            await connection.OpenAsync();

            var stats = new DashboardStatistics
            {
                TotalOrders = await connection.ExecuteScalarAsync<int>("select count(*) from Orders"),
                TotalCustomers = await connection.ExecuteScalarAsync<int>("select count(*) from Customers"),
                TotalProducts = await connection.ExecuteScalarAsync<int>("select count(*) from Products"),
                TodayRevenue = await connection.ExecuteScalarAsync<decimal>(
                    @"select isnull(sum(od.Quantity * od.SalePrice), 0)
                      from Orders o
                      join OrderDetails od on o.OrderID = od.OrderID
                      where o.Status = 4
                        and cast(o.FinishedTime as date) = cast(getdate() as date)"),
                PendingOrders = await connection.ExecuteScalarAsync<int>("select count(*) from Orders where Status in (1, 2)")
            };

            return stats;
        }

        /// <summary>
        /// Lấy doanh thu theo tháng (12 tháng gần nhất)
        /// </summary>
        public static async Task<List<MonthlyRevenue>> GetMonthlyRevenueAsync()
        {
            using var connection = new SqlConnection(Configuration.ConnectionString);
            await connection.OpenAsync();

            var sql = @"with months as (
                            select top 12 
                                year = datepart(year, dateadd(month, -n, getdate())),
                                month = datepart(month, dateadd(month, -n, getdate()))
                            from (select top 12 row_number() over (order by object_id) - 1 as n from sys.objects) t
                            order by year desc, month desc
                        )
                        select 
                            m.year as Year,
                            m.month as Month,
                            Revenue = isnull(sum(od.Quantity * od.SalePrice), 0)
                        from months m
                        left join Orders o on datepart(year, o.FinishedTime) = m.year 
                                          and datepart(month, o.FinishedTime) = m.month
                                          and o.Status = 4
                        left join OrderDetails od on o.OrderID = od.OrderID
                        group by m.year, m.month
                        order by m.year, m.month";

            var result = await connection.QueryAsync<MonthlyRevenue>(sql);
            return result.OrderBy(m => m.Year).ThenBy(m => m.Month).ToList();
        }

        /// <summary>
        /// Lấy danh sách đơn hàng cần xử lý
        /// </summary>
        public static async Task<List<OrderViewInfo>> GetPendingOrdersAsync(int top = 5)
        {
            using var connection = new SqlConnection(Configuration.ConnectionString);
            await connection.OpenAsync();

            var sql = @"select top (@top) 
                               o.*, 
                               c.CustomerName, c.ContactName as CustomerContactName, 
                               c.Address as CustomerAddress, c.Phone as CustomerPhone, c.Email as CustomerEmail,
                               e.FullName as EmployeeName, 
                               s.ShipperName, s.Phone as ShipperPhone,
                               (select isnull(sum(Quantity * SalePrice), 0) from OrderDetails where OrderID = o.OrderID) as TotalValue
                        from Orders o
                        left join Customers c on o.CustomerID = c.CustomerID
                        left join Employees e on o.EmployeeID = e.EmployeeID
                        left join Shippers s on o.ShipperID = s.ShipperID
                        where o.Status in (1, 2)
                        order by o.OrderTime desc";

            var result = await connection.QueryAsync<OrderViewInfo>(sql, new { top = top });
            return result.ToList();
        }

        /// <summary>
        /// Lấy top sản phẩm bán chạy
        /// </summary>
        public static async Task<List<TopProduct>> GetTopSellingProductsAsync(int top = 5)
        {
            using var connection = new SqlConnection(Configuration.ConnectionString);
            await connection.OpenAsync();

            var sql = @"select top (@top) 
                               p.ProductID,
                               p.ProductName,
                               TotalSold = sum(od.Quantity),
                               TotalRevenue = sum(od.Quantity * od.SalePrice)
                        from OrderDetails od
                        join Products p on od.ProductID = p.ProductID
                        join Orders o on od.OrderID = o.OrderID
                        where o.Status = 4
                        group by p.ProductID, p.ProductName
                        order by TotalSold desc";

            var result = await connection.QueryAsync<TopProduct>(sql, new { top = top });
            return result.ToList();
        }
    }

    /// <summary>
    /// Dữ liệu thống kê dashboard
    /// </summary>
    public class DashboardStatistics
    {
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
        public decimal TodayRevenue { get; set; }
        public int PendingOrders { get; set; }
    }

    /// <summary>
    /// Dữ liệu sản phẩm bán chạy
    /// </summary>
    public class TopProduct
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
