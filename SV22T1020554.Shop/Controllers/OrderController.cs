using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020554.BusinessLayers;
using SV22T1020554.Models.Sales;
using SV22T1020554.Shop.Models;
using System.Security.Claims;

namespace SV22T1020554.Shop.Controllers
{
    public class OrderController : Controller
    {
        private const string CART_SESSION_KEY_PREFIX = "ShoppingCart_";

        #region Helpers

        private string GetCartSessionKey()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "guest";
            return CART_SESSION_KEY_PREFIX + userId;
        }

        private List<CartItem> GetCart()
        {
            return ApplicationContext.GetSessionData<List<CartItem>>(GetCartSessionKey()) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            ApplicationContext.SetSessionData(GetCartSessionKey(), cart);
        }

        #endregion

        #region Cart

        /// <summary>
        /// Hi?n th? gi? hàng
        /// </summary>
        [Authorize]
        public IActionResult Cart()
        {
            var cart = GetCart();
            return View(cart);
        }

        /// <summary>
        /// L?y thông tin s?n ph?m cho modal thêm gi? hàng (AJAX)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProductInfo(int productID)
        {
            var product = await CatalogDataService.GetProductAsync(productID);
            if (product == null)
                return Json(new { success = false });

            string categoryName = "";
            if (product.CategoryID.HasValue && product.CategoryID.Value > 0)
            {
                var category = await CatalogDataService.GetCategoryAsync(product.CategoryID.Value);
                categoryName = category?.CategoryName ?? "";
            }

            // L?y thu?c tính s?n ph?m
            var attributes = await CatalogDataService.ListAttributesAsync(productID);

            // Xác ð?nh lo?i s?n ph?m c?n size hay color
            var lowerCat = categoryName.ToLower();
            bool needSize = lowerCat.Contains("giày") || lowerCat.Contains("dép") 
                || lowerCat.Contains("qu?n") || lowerCat.Contains("áo") 
                || lowerCat.Contains("th?i trang") || lowerCat.Contains("váy")
                || lowerCat.Contains("ð?m") || lowerCat.Contains("nón")
                || lowerCat.Contains("m?");
            bool needColor = true; // T?t c? s?n ph?m ð?u có th? ch?n màu

            return Json(new
            {
                success = true,
                productID = product.ProductID,
                productName = product.ProductName,
                photo = product.Photo ?? "",
                price = product.Price,
                unit = product.Unit,
                categoryName = categoryName,
                needSize = needSize,
                needColor = needColor,
                attributes = attributes.Select(a => new { a.AttributeName, a.AttributeValue }).ToList()
            });
        }

        /// <summary>
        /// Thêm hàng vào gi? (AJAX)
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productID, int quantity = 1, string color = "", string size = "")
        {
            if (quantity < 1) quantity = 1;

            var product = await CatalogDataService.GetProductAsync(productID);
            if (product == null)
                return Json(new { success = false, message = "S?n ph?m không t?n t?i" });

            string categoryName = "";
            if (product.CategoryID.HasValue && product.CategoryID.Value > 0)
            {
                var category = await CatalogDataService.GetCategoryAsync(product.CategoryID.Value);
                categoryName = category?.CategoryName ?? "";
            }

            var cart = GetCart();
            // T?m item trùng c? productID, color, size
            var existingItem = cart.FirstOrDefault(c => c.ProductID == productID 
                && c.Color == (color ?? "") && c.Size == (size ?? ""));

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    Photo = product.Photo ?? "",
                    Unit = product.Unit,
                    Price = product.Price,
                    Quantity = quantity,
                    Color = color ?? "",
                    Size = size ?? "",
                    CategoryName = categoryName
                });
            }

            SaveCart(cart);
            int totalCount = cart.Sum(c => c.Quantity);
            return Json(new { success = true, message = "Ð? thêm vào gi? hàng!", count = totalCount });
        }

        /// <summary>
        /// C?p nh?t s? lý?ng trong gi? hàng
        /// </summary>
        [HttpPost]
        public IActionResult UpdateCart(int[] productIDs, int[] quantities)
        {
            var cart = GetCart();

            if (productIDs != null && quantities != null)
            {
                for (int i = 0; i < productIDs.Length; i++)
                {
                    var item = cart.FirstOrDefault(c => c.ProductID == productIDs[i]);
                    if (item != null && i < quantities.Length)
                    {
                        if (quantities[i] > 0)
                            item.Quantity = quantities[i];
                        else
                            cart.Remove(item);
                    }
                }
            }

            SaveCart(cart);
            return RedirectToAction("Cart");
        }

        /// <summary>
        /// Xóa m?t hàng kh?i gi?
        /// </summary>
        public IActionResult RemoveFromCart(int productID)
        {
            var cart = GetCart();
            cart.RemoveAll(c => c.ProductID == productID);
            SaveCart(cart);
            return RedirectToAction("Cart");
        }

        /// <summary>
        /// Xóa toàn b? gi? hàng
        /// </summary>
        public IActionResult ClearCart()
        {
            SaveCart(new List<CartItem>());
            return RedirectToAction("Cart");
        }

        /// <summary>
        /// S? lý?ng s?n ph?m trong gi? (dùng cho AJAX badge)
        /// </summary>
        [HttpGet]
        public IActionResult GetCartCount()
        {
            var cart = GetCart();
            return Json(new { count = cart.Sum(c => c.Quantity) });
        }

        #endregion

        #region Checkout

        /// <summary>
        /// Nh?n danh sách s?n ph?m ðý?c ch?n t? gi? hàng và chuy?n sang trang checkout
        /// </summary>
        [Authorize]
        [HttpPost]
        public IActionResult CheckoutSelected(int[] selectedProductIDs)
        {
            if (selectedProductIDs == null || selectedProductIDs.Length == 0)
                return RedirectToAction("Cart");

            // Lýu danh sách s?n ph?m ðý?c ch?n vào TempData
            TempData["SelectedProductIDs"] = System.Text.Json.JsonSerializer.Serialize(selectedProductIDs);
            return RedirectToAction("Checkout");
        }

        /// <summary>
        /// Hi?n th? form thanh toán (ch? hi?n th? s?n ph?m ð? ch?n)
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Cart");

            // L?y danh sách s?n ph?m ð? ch?n
            List<CartItem> selectedItems;
            if (TempData["SelectedProductIDs"] is string json)
            {
                var selectedIDs = System.Text.Json.JsonSerializer.Deserialize<int[]>(json) ?? Array.Empty<int>();
                selectedItems = cart.Where(c => selectedIDs.Contains(c.ProductID)).ToList();
                // Gi? l?i trong TempData ð? POST checkout có th? dùng
                TempData["SelectedProductIDs"] = System.Text.Json.JsonSerializer.Serialize(selectedIDs);
            }
            else
            {
                // N?u không có danh sách ch?n, l?y toàn b? gi? hàng
                selectedItems = cart;
            }

            if (!selectedItems.Any())
                return RedirectToAction("Cart");

            int customerID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var customer = await PartnerDataService.GetCustomerAsync(customerID);
            ViewBag.Customer = customer;
            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
            return View(selectedItems);
        }

        /// <summary>
        /// X? l? ð?t hàng (ch? ð?t hàng s?n ph?m ð? ch?n)
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Checkout(string deliveryProvince, string deliveryAddress, int[] selectedProductIDs)
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Cart");

            // L?y danh sách s?n ph?m c?n ð?t
            List<CartItem> checkoutItems;
            if (selectedProductIDs != null && selectedProductIDs.Length > 0)
            {
                checkoutItems = cart.Where(c => selectedProductIDs.Contains(c.ProductID)).ToList();
            }
            else if (TempData["SelectedProductIDs"] is string json)
            {
                var ids = System.Text.Json.JsonSerializer.Deserialize<int[]>(json) ?? Array.Empty<int>();
                checkoutItems = cart.Where(c => ids.Contains(c.ProductID)).ToList();
            }
            else
            {
                checkoutItems = cart;
            }

            if (!checkoutItems.Any())
                return RedirectToAction("Cart");

            int customerID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            if (string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
            {
                ViewBag.ErrorMessage = "Vui l?ng nh?p ð?y ð? thông tin giao hàng";
                var customer = await PartnerDataService.GetCustomerAsync(customerID);
                ViewBag.Customer = customer;
                ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
                return View(checkoutItems);
            }

            // Create order
            var order = new Order
            {
                CustomerID = customerID,
                DeliveryProvince = deliveryProvince,
                DeliveryAddress = deliveryAddress
            };

            int orderID = await SalesDataService.AddOrderAsync(order);
            if (orderID > 0)
            {
                // Add order details
                foreach (var item in checkoutItems)
                {
                    var detail = new OrderDetail
                    {
                        OrderID = orderID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        SalePrice = item.Price
                    };
                    await SalesDataService.AddDetailAsync(detail);
                }

                // Ch? xóa s?n ph?m ð? ð?t kh?i gi? hàng (gi? l?i s?n ph?m chýa ch?n)
                var orderedProductIDs = checkoutItems.Select(c => c.ProductID).ToHashSet();
                var remainingCart = cart.Where(c => !orderedProductIDs.Contains(c.ProductID)).ToList();
                SaveCart(remainingCart);

                TempData["SuccessMessage"] = "Ð?t hàng, thanh toán thành công! M? ðõn hàng: " + orderID;
                return RedirectToAction("Detail", new { id = orderID });
            }

            ViewBag.ErrorMessage = "Ð?t hàng th?t b?i. Vui l?ng th? l?i.";
            var cust = await PartnerDataService.GetCustomerAsync(customerID);
            ViewBag.Customer = cust;
            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
            return View(checkoutItems);
        }

        #endregion

        #region Order History & Tracking

        /// <summary>
        /// L?ch s? ðõn hàng
        /// </summary>
        [Authorize]
        public async Task<IActionResult> Index(int page = 1, int status = 0)
        {
            int customerID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var input = new OrderSearchInput
            {
                Page = page,
                PageSize = 10,
                SearchValue = "",
                CustomerID = customerID,
                Status = (OrderStatusEnum)status
            };

            var result = await SalesDataService.ListOrdersAsync(input);

            ViewBag.Status = status;
            return View(result);
        }

        /// <summary>
        /// Chi ti?t ðõn hàng
        /// </summary>
        [Authorize]
        public async Task<IActionResult> Detail(int id = 0)
        {
            if (id <= 0)
                return RedirectToAction("Index");

            int customerID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null || order.CustomerID != customerID)
                return RedirectToAction("Index");

            var details = await SalesDataService.ListDetailsAsync(id);
            ViewBag.OrderDetails = details;

            return View(order);
        }

        #endregion
    }
}

