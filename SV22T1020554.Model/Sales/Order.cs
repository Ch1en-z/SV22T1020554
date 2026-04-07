namespace SV22T1020554.Models.Sales
{
    /// <summary>
    /// Ðõn hàng
    /// </summary>
    public class Order
    {
        /// <summary>
        /// M? ðõn hàng
        /// </summary>
        public int OrderID { get; set; }

        /// <summary>
        /// M? khách hàng
        /// </summary>
        public int? CustomerID { get; set; }

        /// <summary>
        /// Th?i ði?m ð?t hàng (th?i ði?m t?o ðõn hàng)
        /// </summary>
        public DateTime OrderTime { get; set; }

        /// <summary>
        /// T?nh/thành giao hàng
        /// </summary>
        public string? DeliveryProvince { get; set; }

        /// <summary>
        /// Ð?a ch? giao hàng
        /// </summary>
        public string? DeliveryAddress { get; set; }

        /// <summary>
        /// M? nhân viên x? l? ðõn hàng (ngý?i nh?n/duy?t ðõn hàng)
        /// </summary>
        public int? EmployeeID { get; set; }

        /// <summary>
        /// Th?i ði?m duy?t ðõn hàng (th?i ði?m nhân viên nh?n/duy?t ðõn hàng)
        /// </summary>
        public DateTime? AcceptTime { get; set; }

        /// <summary>
        /// M? ngý?i giao hàng
        /// </summary>
        public int? ShipperID { get; set; }

        /// <summary>
        /// Th?i ði?m ngý?i giao hàng nh?n ðõn hàng ð? giao
        /// </summary>
        public DateTime? ShippedTime { get; set; }

        /// <summary>
        /// Th?i ði?m k?t thúc ðõn hàng
        /// </summary>
        public DateTime? FinishedTime { get; set; }

        /// <summary>
        /// Tr?ng thái hi?n t?i c?a ðõn hàng
        /// </summary>
        public OrderStatusEnum Status { get; set; }
    }
}
