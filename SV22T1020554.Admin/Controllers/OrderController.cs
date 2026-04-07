using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SV22T1020554.BusinessLayers;
using SV22T1020554.Models.Catalog;
using SV22T1020554.Models.Sales;
using SV22T1020554.Models.Common;
using SV22T1020554.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SV22T1020554.Admin.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const string SHOPPING_CART = "ShoppingCart";
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
        
        /// <summary>
        /// Hi?n th? danh sách ðõn hàng
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// T?m ki?m và l?c ðõn hàng
        /// </summary>
        public async Task<IActionResult> Search(OrderSearchInput input)
        {
            var data = await SalesDataService.ListOrdersAsync(input);
            return PartialView(data);
        }
        
        public async Task<IActionResult> SearchProduct(ProductSearchInput input)
        {
            if (input.Page <= 0) input.Page = 1;
            if (input.PageSize <= 0) input.PageSize = 24;

            var result = await CatalogDataService.ListProductsAsync(input);
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, input);
            return PartialView(result);
        }

        // --- L?P ÐÕN HÀNG (GI? HÀNG) ---
        public IActionResult Create()
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = 24,
                    SearchValue = ""
                };
            }
            return View(input);
        }

        /// <summary>
        /// Hi?n th? gi? hàng
        /// </summary>
        public IActionResult ShowShoppingCart()
        {
            var cart = GetShoppingCart();
            return PartialView(cart);
        }

        /// <summary>
        /// Thêm m?t hàng vào gi? hàng
        /// </summary>
        public IActionResult AddToCart(OrderDetailViewInfo item)
        {
            if (item.SalePrice <= 0 || item.Quantity <= 0)
                return Json("Giá bán và s? lý?ng không h?p l?");

            var cart = GetShoppingCart();
            var existsItem = cart.FirstOrDefault(m => m.ProductID == item.ProductID);
            if (existsItem == null)
            {
                cart.Add(item);
            }
            else
            {
                existsItem.Quantity += item.Quantity;
                existsItem.SalePrice = item.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            return Json("");
        }

        /// <summary>
        /// Xóa m?t hàng kh?i gi? hàng
        /// </summary>
        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetShoppingCart();
            int index = cart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                cart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            return Json("");
        }

        /// <summary>
        /// C?p nh?t gi? hàng
        /// </summary>
        public IActionResult UpdateCartItem(int id, int quantity, decimal salePrice)
        {
            if (quantity <= 0)
                return Json("S? lý?ng không h?p l?");

            var cart = GetShoppingCart();
            var existsItem = cart.FirstOrDefault(m => m.ProductID == id);
            if (existsItem != null)
            {
                existsItem.Quantity = quantity;
                existsItem.SalePrice = salePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            return Json("");
        }

        /// <summary>
        /// Xóa toàn b? gi? hàng
        /// </summary>
        public IActionResult ClearCart()
        {
            var cart = GetShoppingCart();
            cart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            return Json("");
        }

        /// <summary>
        /// Kh?i t?o ðõn hàng
        /// </summary>
        public async Task<IActionResult> Init(int customerID, string deliveryProvince, string deliveryAddress)
        {
            var cart = GetShoppingCart();
            if (cart.Count == 0)
                return Json("Gi? hàng ðang tr?ng");

            if (customerID <= 0 || string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
                return Json("Vui l?ng ch?n khách hàng và nh?p ð?y ð? nõi giao hàng");

            int employeeID = 1; // T?m th?i dùng ID c? ð?nh c?a nhân viên ðang ðãng nh?p

            List<OrderDetail> details = new List<OrderDetail>();
            foreach (var item in cart)
            {
                details.Add(new OrderDetail()
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice
                });
            }

            int orderID = await SalesDataService.AddOrderAsync(new Order()
            {
                CustomerID = customerID,
                DeliveryProvince = deliveryProvince,
                DeliveryAddress = deliveryAddress,
                EmployeeID = employeeID
            });

            if (orderID > 0)
            {
                foreach (var item in details)
                {
                    item.OrderID = orderID;
                    await SalesDataService.AddDetailAsync(item);
                }
                cart.Clear();
                ApplicationContext.SetSessionData(SHOPPING_CART, cart);
                return Json(new { success = true, orderID = orderID });
            }

            return Json("Không l?p ðý?c ðõn hàng");
        }

        private List<OrderDetailViewInfo> GetShoppingCart()
        {
            var cart = ApplicationContext.GetSessionData<List<OrderDetailViewInfo>>(SHOPPING_CART);
            if (cart == null)
            {
                cart = new List<OrderDetailViewInfo>();
                ApplicationContext.SetSessionData(SHOPPING_CART, cart);
            }
            return cart;
        }

        /// <summary>
        /// Hi?n th? chi ti?t ðõn hàng
        /// </summary>
        /// <param name="id">M? ðõn hàng</param>
        /// <return></return>
        public async Task<IActionResult> Detail(int id)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }
            var details = await SalesDataService.ListDetailsAsync(id);
            ViewBag.Details = details;
            return View(order);
        }

        /// <summary>
        /// Duy?t ðõn hàng (chuy?n tr?ng thái thành ð? xác nh?n)
        /// </summary>
        /// <param name="id">M? ðõn hàng</param>
        /// <return></return>
        public async Task<IActionResult> Accept(int id)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null || order.Status != OrderStatusEnum.New)
            {
                TempData["ErrorMessage"] = "Ðõn hàng không t?n t?i ho?c không ? tr?ng thái ch? duy?t";
                return RedirectToAction("Detail", new { id = id });
            }

            int employeeID = 1; // TODO: L?y ID t? User Principal
            bool result = await SalesDataService.AcceptOrderAsync(id, employeeID);
            if (!result)
                TempData["ErrorMessage"] = "Không th? duy?t ðõn hàng này";
            return RedirectToAction("Detail", new { id = id });
        }

        /// <summary>
        /// Giao di?n ð? ch?n ngý?i giao hàng
        /// </summary>
        [HttpGet]
        public IActionResult Shipping(int id)
        {
            ViewBag.OrderID = id;
            return PartialView();
        }

        /// <summary>
        /// Xác nh?n chuy?n ðõn hàng sang tr?ng thái ðang giao hàng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Shipping(int id, int shipperID)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null || order.Status != OrderStatusEnum.Accepted)
            {
                TempData["ErrorMessage"] = "Ðõn hàng không t?n t?i ho?c không ? tr?ng thái ch? giao hàng";
                return RedirectToAction("Detail", new { id = id });
            }

            if (shipperID <= 0)
            {
                TempData["ErrorMessage"] = "Vui l?ng ch?n ngý?i giao hàng";
                return RedirectToAction("Detail", new { id = id });
            }

            bool result = await SalesDataService.ShipOrderAsync(id, shipperID);
            if (!result)
                TempData["ErrorMessage"] = "Không th? th?c hi?n chuy?n hàng";
            return RedirectToAction("Detail", new { id = id });
        }

        /// <summary>
        /// Hoàn t?t ðõn hàng (thành công)
        /// </summary>
        public async Task<IActionResult> Finish(int id)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null || order.Status != OrderStatusEnum.Shipping)
            {
                TempData["ErrorMessage"] = "Ðõn hàng không t?n t?i ho?c không ? tr?ng thái ðang giao";
                return RedirectToAction("Detail", new { id = id });
            }

            bool result = await SalesDataService.CompleteOrderAsync(id);
            if (!result)
                TempData["ErrorMessage"] = "Không th? hoàn t?t ðõn hàng này";
            return RedirectToAction("Detail", new { id = id });
        }

        /// <summary>
        /// T? ch?i ðõn hàng
        /// </summary>
        public async Task<IActionResult> Reject(int id)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null || order.Status != OrderStatusEnum.New)
            {
                TempData["ErrorMessage"] = "Ðõn hàng không t?n t?i ho?c không ? tr?ng thái ch? duy?t";
                return RedirectToAction("Detail", new { id = id });
            }

            int employeeID = 1; // TODO: L?y ID t? User Principal
            bool result = await SalesDataService.RejectOrderAsync(id, employeeID);
            if (!result)
                TempData["ErrorMessage"] = "Không th? t? ch?i ðõn hàng này";
            return RedirectToAction("Detail", new { id = id });
        }

        /// <summary>
        /// H?y ðõn hàng
        /// </summary>
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null || (order.Status != OrderStatusEnum.New && order.Status != OrderStatusEnum.Accepted))
            {
                TempData["ErrorMessage"] = "Ðõn hàng không th? h?y ? tr?ng thái hi?n t?i";
                return RedirectToAction("Detail", new { id = id });
            }

            bool result = await SalesDataService.CancelOrderAsync(id);
            if (!result)
                TempData["ErrorMessage"] = "Không th? h?y ðõn hàng này";
            return RedirectToAction("Detail", new { id = id });
        }

        /// <summary>
        /// Xóa ðõn hàng
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null || (order.Status != OrderStatusEnum.New && order.Status != OrderStatusEnum.Rejected && order.Status != OrderStatusEnum.Cancelled))
            {
                TempData["ErrorMessage"] = "Ch? có th? xóa ðõn hàng ? tr?ng thái v?a t?o, b? t? ch?i ho?c b? h?y";
                return RedirectToAction("Detail", new { id = id });
            }

            bool result = await SalesDataService.DeleteOrderAsync(id);
            if (!result)
                TempData["ErrorMessage"] = "Không th? xóa ðõn hàng này";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Giao di?n c?p nh?t m?t hàng trong ðõn hàng
        /// </summary>
        public async Task<IActionResult> EditDetail(int id, int productId)
        {
            var data = await SalesDataService.GetDetailAsync(id, productId);
            return PartialView(data);
        }

        /// <summary>
        /// C?p nh?t m?t hàng trong ðõn hàng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateDetail(OrderDetail data)
        {
            var order = await SalesDataService.GetOrderAsync(data.OrderID);
            if (order == null || order.Status != OrderStatusEnum.New)
            {
                return Json("Ch? có th? c?p nh?t m?t hàng khi ðõn hàng ? tr?ng thái ch? duy?t");
            }

            if (data.Quantity <= 0 || data.SalePrice < 0)
                return Json("D? li?u không h?p l?");

            bool result = await SalesDataService.UpdateDetailAsync(data);
            if (!result)
                return Json("Không th? c?p nh?t m?t hàng này");
            return Json("");
        }

        /// <summary>
        /// Xóa m?t hàng kh?i ðõn hàng
        /// </summary>
        public async Task<IActionResult> DeleteDetail(int id, int productId)
        {
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null || order.Status != OrderStatusEnum.New)
            {
                TempData["ErrorMessage"] = "Ch? có th? xóa m?t hàng khi ðõn hàng ? tr?ng thái ch? duy?t";
                return RedirectToAction("Detail", new { id = id });
            }

            bool result = await SalesDataService.DeleteDetailAsync(id, productId);
            if (!result)
                TempData["ErrorMessage"] = "Không th? xóa m?t hàng này kh?i ðõn hàng";
            return RedirectToAction("Detail", new { id = id });
        }
    }
}

