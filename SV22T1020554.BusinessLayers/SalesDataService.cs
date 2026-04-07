
using SV22T1020554.DataLayers.Interfaces;
using SV22T1020554.DataLayers.SQLServer;
using SV22T1020554.Models.Common;
using SV22T1020554.Models.Sales;

namespace SV22T1020554.BusinessLayers
{
    /// <summary>
    /// Cung c?p các ch?c nãng x? l? d? li?u liên quan ð?n bán hàng
    /// bao g?m: ðõn hàng (Order) và chi ti?t ðõn hàng (OrderDetail).
    /// </summary>
    public static class SalesDataService
    {
        private static readonly IOrderRepository orderDB;

        /// <summary>
        /// Constructor
        /// </summary>
        static SalesDataService()
        {
            orderDB = new OrderRepository(Configuration.ConnectionString);
        }

        #region Order

        /// <summary>
        /// T?m ki?m và l?y danh sách ðõn hàng dý?i d?ng phân trang
        /// </summary>
        public static async Task<PagedResult<OrderViewInfo>> ListOrdersAsync(OrderSearchInput input)
        {
            return await orderDB.ListAsync(input);
        }

        /// <summary>
        /// L?y thông tin chi ti?t c?a m?t ðõn hàng
        /// </summary>
        public static async Task<OrderViewInfo?> GetOrderAsync(int orderID)
        {
            return await orderDB.GetAsync(orderID);
        }

        /// <summary>
        /// T?o ðõn hàng m?i
        /// </summary>
        public static async Task<int> AddOrderAsync(Order data)
        {
            data.Status = OrderStatusEnum.New;
            data.OrderTime = DateTime.Now;

            return await orderDB.AddAsync(data);
        }

        /// <summary>
        /// C?p nh?t thông tin ðõn hàng
        /// </summary>
        public static async Task<bool> UpdateOrderAsync(Order data)
        {
            //TODO: Ki?m tra d? li?u và tr?ng thái ðõn hàng trý?c khi c?p nh?t
            return await orderDB.UpdateAsync(data);
        }

        /// <summary>
        /// Xóa ðõn hàng
        /// </summary>
        public static async Task<bool> DeleteOrderAsync(int orderID)
        {
            //TODO: Ki?m tra tr?ng thái ðõn hàng trý?c khi xóa
            return await orderDB.DeleteAsync(orderID);
        }

        #endregion

        #region Order Status Processing

        /// <summary>
        /// Duy?t ðõn hàng
        /// </summary>
        public static async Task<bool> AcceptOrderAsync(int orderID, int employeeID)
        {
            var order = await orderDB.GetAsync(orderID);
            if (order == null) 
                return false;

            if (order.Status != OrderStatusEnum.New)
                return false;

            order.EmployeeID = employeeID;
            order.AcceptTime = DateTime.Now;
            order.Status = OrderStatusEnum.Accepted;

            return await orderDB.UpdateAsync(order);
        }

        /// <summary>
        /// T? ch?i ðõn hàng
        /// </summary>
        public static async Task<bool> RejectOrderAsync(int orderID, int employeeID)
        {
            var order = await orderDB.GetAsync(orderID);
            if (order == null) 
                return false;

            if (order.Status != OrderStatusEnum.New)
                return false;

            order.EmployeeID = employeeID;
            order.FinishedTime = DateTime.Now;
            order.Status = OrderStatusEnum.Rejected;
            
            return await orderDB.UpdateAsync(order);
        }

        /// <summary>
        /// H?y ðõn hàng
        /// </summary>
        public static async Task<bool> CancelOrderAsync(int orderID)
        {
            var order = await orderDB.GetAsync(orderID);
            if (order == null) 
                return false;

            if (order.Status != OrderStatusEnum.New &&
                order.Status != OrderStatusEnum.Accepted)
                return false;

            order.FinishedTime = DateTime.Now;
            order.Status = OrderStatusEnum.Cancelled;
            
            return await orderDB.UpdateAsync(order);
        }

        /// <summary>
        /// Giao ðõn hàng cho ngý?i giao hàng
        /// </summary>
        public static async Task<bool> ShipOrderAsync(int orderID, int shipperID)
        {
            var order = await orderDB.GetAsync(orderID);
            if (order == null) 
                return false;

            if (order.Status != OrderStatusEnum.Accepted)
                return false;

            order.ShipperID = shipperID;
            order.ShippedTime = DateTime.Now;
            order.Status = OrderStatusEnum.Shipping;
            
            return await orderDB.UpdateAsync(order);
        }

        /// <summary>
        /// Hoàn t?t ðõn hàng
        /// </summary>
        public static async Task<bool> CompleteOrderAsync(int orderID)
        {
            var order = await orderDB.GetAsync(orderID);
            if (order == null) 
                return false;

            if (order.Status != OrderStatusEnum.Shipping)
                return false;

            order.FinishedTime = DateTime.Now;
            order.Status = OrderStatusEnum.Completed;
            
            return await orderDB.UpdateAsync(order);
        }

        #endregion

        #region Order Detail

        /// <summary>
        /// L?y danh sách m?t hàng c?a ðõn hàng
        /// </summary>
        public static async Task<List<OrderDetailViewInfo>> ListDetailsAsync(int orderID)
        {
            return await orderDB.ListDetailsAsync(orderID);
        }

        /// <summary>
        /// L?y thông tin m?t m?t hàng trong ðõn hàng
        /// </summary>
        public static async Task<OrderDetailViewInfo?> GetDetailAsync(int orderID, int productID)
        {
            return await orderDB.GetDetailAsync(orderID, productID);
        }

        /// <summary>
        /// Thêm m?t hàng vào ðõn hàng
        /// </summary>
        public static async Task<bool> AddDetailAsync(OrderDetail data)
        {
            //TODO: Ki?m tra d? li?u và tr?ng thái ðõn hàng trý?c khi thêm m?t hàng
            return await orderDB.AddDetailAsync(data);
        }

        /// <summary>
        /// C?p nh?t m?t hàng trong ðõn hàng
        /// </summary>
        public static async Task<bool> UpdateDetailAsync(OrderDetail data)
        {
            //TODO: Ki?m tra d? li?u và tr?ng thái ðõn hàng trý?c khi c?p nh?t m?t hàng
            return await orderDB.UpdateDetailAsync(data);
        }

        /// <summary>
        /// Xóa m?t hàng kh?i ðõn hàng
        /// </summary>
        public static async Task<bool> DeleteDetailAsync(int orderID, int productID)
        {
            //TODO: Ki?m tra tr?ng thái ðõn hàng trý?c khi xóa m?t hàng
            return await orderDB.DeleteDetailAsync(orderID, productID);
        }

        #endregion
    }
}
