using SV22T1020554.Models.Sales;

namespace SV22T1020554.Admin
{
    /// <summary>
    /// Cung c?p các ch?c nãng x? l? trên gi? hàng
    /// (Gi? hàng lýu trong session)
    /// </summary>
    public static class ShoppingCartService
    {
        /// <summary>
        /// Tên bi?n ð? lýu gi? hàng trong session
        /// </summary>
        private const string CART = "ShoppingCart";

        /// <summary>
        /// L?y gi? hàng t? session
        /// </summary>
        /// <returns></returns>
        public static List<OrderDetailViewInfo> GetShoppingCart()
        {
            var cart = ApplicationContext.GetSessionData<List<OrderDetailViewInfo>>(CART);
            if (cart == null)
            {
                cart = new List<OrderDetailViewInfo>();
                ApplicationContext.SetSessionData(CART, cart);
            }
            return cart;
        }

        /// <summary>
        /// L?y thông tin 1 m?t hàng t? gi? hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static OrderDetailViewInfo? GetCartItem(int productID)
        {
            var cart = GetShoppingCart();
            return cart.Find(m => m.ProductID == productID);
        }

        /// <summary>
        /// Thêm hàng vào gi? hàng
        /// </summary>
        /// <param name="item"></param>
        public static void AddCartItem(OrderDetailViewInfo item)
        {
            var cart = GetShoppingCart();
            var existsItem = cart.Find(m => m.ProductID == item.ProductID);
            if (existsItem == null)
            {
                cart.Add(item);
            }
            else
            {
                existsItem.Quantity += item.Quantity;
                existsItem.SalePrice = item.SalePrice;
            }
            ApplicationContext.SetSessionData(CART, cart);
        }

        /// <summary>
        /// C?p nh?t s? lý?ng và giá c?a m?t m?t hàng trong gi? hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="quantity"></param>
        /// <param name="salePrice"></param>
        public static void UpdateCartItem(int productID, int quantity, decimal salePrice)
        {
            var cart = GetShoppingCart();
            var item = cart.Find(m => m.ProductID == productID);
            if (item != null)
            {
                item.Quantity = quantity;
                item.SalePrice = salePrice;
                ApplicationContext.SetSessionData(CART, cart);
            }
        }

        /// <summary>
        /// Xóa m?t m?t hàng ra kh?i gi? hàng
        /// </summary>
        /// <param name="productID"></param>
        public static void RemoveCartItem(int productID)
        {
            var cart = GetShoppingCart();
            int index = cart.FindIndex(m => m.ProductID == productID);
            if (index >= 0)
            {
                cart.RemoveAt(index);
                ApplicationContext.SetSessionData(CART, cart);
            }
        }

        /// <summary>
        /// Xóa toàn b? gi? hàng
        /// </summary>
        public static void ClearCart()
        {
            var cart = new List<OrderDetailViewInfo>();
            ApplicationContext.SetSessionData(CART, cart);
        }
    }
}
