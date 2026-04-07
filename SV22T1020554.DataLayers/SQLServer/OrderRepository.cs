using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using SV22T1020554.Models.Sales;
using SV22T1020554.Models.Common;
using SV22T1020554.DataLayers.Interfaces;

namespace SV22T1020554.DataLayers.SQLServer
{
    /// <summary>
    /// Cài đặt các phép xử lý dữ liệu cho đơn hàng trên SQL Server
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> AddAsync(Order data)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"insert into Orders(CustomerID, OrderTime, DeliveryProvince, DeliveryAddress, EmployeeID, Status)
                            values (@CustomerID, getdate(), @DeliveryProvince, @DeliveryAddress, @EmployeeID, @Status);
                            select @@identity;";
                var parameters = new
                {
                    CustomerID = data.CustomerID,
                    DeliveryProvince = data.DeliveryProvince ?? "",
                    DeliveryAddress = data.DeliveryAddress ?? "",
                    EmployeeID = data.EmployeeID,
                    Status = 1 // Trạng thái đơn hàng vừa tạo (Order)
                };
                id = await connection.ExecuteScalarAsync<int>(sql, parameters);
            }
            return id;
        }

        public async Task<bool> DeleteAsync(int orderID)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"delete from OrderDetails where OrderID = @OrderID;
                            delete from Orders where OrderID = @OrderID;";
                var parameters = new { OrderID = orderID };
                result = await connection.ExecuteAsync(sql, parameters) > 0;
            }
            return result;
        }

        public async Task<OrderViewInfo?> GetAsync(int orderID)
        {
            OrderViewInfo? data = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select o.*, 
                                   c.CustomerName, c.ContactName as CustomerContactName, c.Address as CustomerAddress, c.Phone as CustomerPhone, c.Email as CustomerEmail,
                                   e.FullName as EmployeeName, 
                                   s.ShipperName, s.Phone as ShipperPhone,
                                   (select isnull(sum(Quantity * SalePrice), 0) from OrderDetails where OrderID = o.OrderID) as TotalValue
                            from Orders o
                            left join Customers c on o.CustomerID = c.CustomerID
                            left join Employees e on o.EmployeeID = e.EmployeeID
                            left join Shippers s on o.ShipperID = s.ShipperID
                            where o.OrderID = @OrderID";
                var parameters = new { OrderID = orderID };
                data = await connection.QueryFirstOrDefaultAsync<OrderViewInfo>(sql, parameters);
            }
            return data;
        }

        public async Task<PagedResult<OrderViewInfo>> ListAsync(OrderSearchInput input)
        {
            string searchValue = input.SearchValue ?? "";
            bool hasSearch = !string.IsNullOrEmpty(searchValue);
            if (hasSearch)
                searchValue = "%" + searchValue + "%";

            using (var connection = new SqlConnection(_connectionString))
            {
                // Điều kiện lọc theo các trường cố định (hiệu năng cao)
                string whereClause = "where (@CustomerID = 0 or o.CustomerID = @CustomerID) and (@Status = 0 or o.Status = @Status)";
                if (input.DateFrom.HasValue)
                    whereClause += " and o.OrderTime >= @DateFrom";
                if (input.DateTo.HasValue)
                    whereClause += " and o.OrderTime <= @DateTo";

                // Chỉ Join các bảng liên quan khi có tìm kiếm theo chuỗi (searchValue)
                string joinClause = "";
                if (hasSearch)
                {
                    joinClause = @"left join Customers c on o.CustomerID = c.CustomerID
                                   left join Employees e on o.EmployeeID = e.EmployeeID
                                   left join Shippers s on o.ShipperID = s.ShipperID";
                    whereClause += " and (c.CustomerName like @searchValue or e.FullName like @searchValue or s.ShipperName like @searchValue)";
                }

                var countSql = $@"select count(*) from Orders o {joinClause} {whereClause}";

                var listSql = $@"with cte as (
                                    select o.*, 
                                           c.CustomerName, c.ContactName as CustomerContactName, c.Address as CustomerAddress, c.Phone as CustomerPhone, c.Email as CustomerEmail,
                                           e.FullName as EmployeeName, 
                                           s.ShipperName, s.Phone as ShipperPhone,
                                           row_number() over (order by o.OrderTime desc) as RowNumber
                                    from Orders o
                                    left join Customers c on o.CustomerID = c.CustomerID
                                    left join Employees e on o.EmployeeID = e.EmployeeID
                                    left join Shippers s on o.ShipperID = s.ShipperID
                                    {whereClause}
                                )
                                select t.*,
                                       (select isnull(sum(Quantity * SalePrice), 0) from OrderDetails where OrderID = t.OrderID) as TotalValue
                                from cte as t
                                where (@pageSize = 0) 
                                   or (t.RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                                order by t.RowNumber";

                var parameters = new
                {
                    page = input.Page,
                    pageSize = input.PageSize,
                    searchValue = searchValue,
                    Status = (int)input.Status,
                    CustomerID = input.CustomerID,
                    DateFrom = input.DateFrom,
                    DateTo = input.DateTo
                };

                int rowCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);
                var data = await connection.QueryAsync<OrderViewInfo>(listSql, parameters);

                return new PagedResult<OrderViewInfo>
                {
                    Page = input.Page,
                    PageSize = input.PageSize,
                    RowCount = rowCount,
                    DataItems = data.ToList()
                };
            }
        }

        public async Task<bool> UpdateAsync(Order data)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"update Orders 
                            set CustomerID = @CustomerID,
                                DeliveryProvince = @DeliveryProvince,
                                DeliveryAddress = @DeliveryAddress,
                                EmployeeID = @EmployeeID,
                                AcceptTime = @AcceptTime,
                                ShipperID = @ShipperID,
                                ShippedTime = @ShippedTime,
                                FinishedTime = @FinishedTime,
                                Status = @Status
                            where OrderID = @OrderID";
                var parameters = new
                {
                    CustomerID = data.CustomerID,
                    DeliveryProvince = data.DeliveryProvince ?? "",
                    DeliveryAddress = data.DeliveryAddress ?? "",
                    EmployeeID = data.EmployeeID,
                    AcceptTime = data.AcceptTime,
                    ShipperID = data.ShipperID,
                    ShippedTime = data.ShippedTime,
                    FinishedTime = data.FinishedTime,
                    Status = data.Status,
                    OrderID = data.OrderID
                };
                result = await connection.ExecuteAsync(sql, parameters) > 0;
            }
            return result;
        }

        public async Task<bool> AddDetailAsync(OrderDetail data)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"insert into OrderDetails(OrderID, ProductID, Quantity, SalePrice)
                            values (@OrderID, @ProductID, @Quantity, @SalePrice)";
                var parameters = new
                {
                    OrderID = data.OrderID,
                    ProductID = data.ProductID,
                    Quantity = data.Quantity,
                    SalePrice = data.SalePrice
                };
                result = await connection.ExecuteAsync(sql, parameters) > 0;
            }
            return result;
        }

        public async Task<bool> DeleteDetailAsync(int orderID, int productID)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "delete from OrderDetails where OrderID = @OrderID and ProductID = @ProductID";
                var parameters = new { OrderID = orderID, ProductID = productID };
                result = await connection.ExecuteAsync(sql, parameters) > 0;
            }
            return result;
        }

        public async Task<OrderDetailViewInfo?> GetDetailAsync(int orderID, int productID)
        {
            OrderDetailViewInfo? data = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select od.*, p.ProductName, p.Photo, p.Unit
                            from OrderDetails od
                            join Products p on od.ProductID = p.ProductID
                            where od.OrderID = @OrderID and od.ProductID = @ProductID";
                var parameters = new { OrderID = orderID, ProductID = productID };
                data = await connection.QueryFirstOrDefaultAsync<OrderDetailViewInfo>(sql, parameters);
            }
            return data;
        }

        public async Task<List<OrderDetailViewInfo>> ListDetailsAsync(int orderID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select od.*, p.ProductName, p.Photo, p.Unit
                            from OrderDetails od
                            join Products p on od.ProductID = p.ProductID
                            where od.OrderID = @OrderID";
                var parameters = new { OrderID = orderID };
                var data = await connection.QueryAsync<OrderDetailViewInfo>(sql, parameters);
                return data.ToList();
            }
        }

        public async Task<bool> UpdateDetailAsync(OrderDetail data)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"update OrderDetails 
                            set Quantity = @Quantity,
                                SalePrice = @SalePrice
                            where OrderID = @OrderID and ProductID = @ProductID";
                var parameters = new
                {
                    Quantity = data.Quantity,
                    SalePrice = data.SalePrice,
                    OrderID = data.OrderID,
                    ProductID = data.ProductID
                };
                result = await connection.ExecuteAsync(sql, parameters) > 0;
            }
            return result;
        }

        /// <summary>
        /// Lấy tổng số đơn hàng
        /// </summary>
        public async Task<int> CountAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "select count(*) from Orders";
                return await connection.ExecuteScalarAsync<int>(sql);
            }
        }

        /// <summary>
        /// Lấy doanh thu hôm nay (các đơn hàng đã hoàn thành)
        /// </summary>
        public async Task<decimal> GetTodayRevenueAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select isnull(sum(od.Quantity * od.SalePrice), 0)
                            from Orders o
                            join OrderDetails od on o.OrderID = od.OrderID
                            where o.Status = 4
                              and cast(o.FinishedTime as date) = cast(getdate() as date)";
                return await connection.ExecuteScalarAsync<decimal>(sql);
            }
        }

        /// <summary>
        /// Lấy doanh thu theo tháng (12 tháng gần nhất)
        /// </summary>
        public async Task<List<MonthlyRevenue>> GetMonthlyRevenueAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"with months as (
                                select top 12 
                                    year = datepart(year, dateadd(month, -n, getdate())),
                                    month = datepart(month, dateadd(month, -n, getdate()))
                                from (select top 12 row_number() over (order by object_id) - 1 as n from sys.objects) t
                                order by year, month
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
                return result.ToList();
            }
        }

        /// <summary>
        /// Lấy danh sách đơn hàng cần xử lý (trạng thái New hoặc Accepted)
        /// </summary>
        public async Task<List<OrderViewInfo>> GetPendingOrdersAsync(int top = 5)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
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

                var parameters = new { top = top };
                var data = await connection.QueryAsync<OrderViewInfo>(sql, parameters);
                return data.ToList();
            }
        }
    }
}
