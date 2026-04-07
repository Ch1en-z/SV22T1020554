using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SV22T1020554.BusinessLayers;
using SV22T1020554.Models.Common;
using SV22T1020554.Models.Partner;

namespace SV22T1020554.Admin.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string SEARCH_CONDITION = "CustomerSearchCondition";

        /// <summary>
        /// Hi?n th? danh sách khách hàng (có t?m ki?m và phân trang)
        /// </summary>
        public async Task<IActionResult> Index(int page = 1, string searchValue = "")
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(SEARCH_CONDITION);
            if (input == null)
            {
                input = new PaginationSearchInput
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }

            if (Request.Query.ContainsKey("searchValue"))
            {
                input.SearchValue = searchValue ?? "";
                input.Page = 1;
            }
            else if (page > 0)
            {
                input.Page = page;
            }

            ApplicationContext.SetSessionData(SEARCH_CONDITION, input);

            var data = await PartnerDataService.ListCustomersAsync(input);
            ViewBag.SearchValue = input.SearchValue;
            return View(data);
        }

        /// <summary>
        /// Hi?n th? form t?o khách hàng m?i
        /// </summary>
        public async Task<IActionResult> Create()
        {
            ViewBag.Title = "B? sung khách hàng";
            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
            var customer = new Customer()
            {
                CustomerID = 0
            };
            return View("Edit", customer);
        }

        /// <summary>
        /// Hi?n th? form c?p nh?t thông tin khách hàng
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Ch?nh s?a khách hàng";
            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
            var customer = await PartnerDataService.GetCustomerAsync(id);
            if (customer == null)
                return RedirectToAction("Index");

            return View(customer);
        }

        /// <summary>
        /// Lýu d? li?u khách hàng (T?o m?i ho?c c?p nh?t)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Save(Customer data)
        {
            // Validate d? li?u
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không ðý?c ð? tr?ng");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao d?ch không ðý?c ð? tr?ng");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email không ðý?c ð? tr?ng");

            // Ki?m tra email trùng
            if (!string.IsNullOrWhiteSpace(data.Email))
            {
                bool emailExists = await PartnerDataService.ValidateCustomerEmailAsync(data.Email, data.CustomerID);
                if (emailExists)
                    ModelState.AddModelError(nameof(data.Email), "Email ð? ðý?c s? d?ng b?i khách hàng khác");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Title = data.CustomerID == 0 ? "B? sung khách hàng" : "Ch?nh s?a khách hàng";
                ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
                return View("Edit", data);
            }

            if (data.CustomerID == 0)
            {
                await PartnerDataService.AddCustomerAsync(data);
            }
            else
            {
                await PartnerDataService.UpdateCustomerAsync(data);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Xác nh?n xóa ho?c th?c hi?n xóa khách hàng
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                bool result = await PartnerDataService.DeleteCustomerAsync(id);
                if (!result)
                    TempData["Message"] = "Không th? xóa khách hàng này v? ðang có d? li?u liên quan";
                return RedirectToAction("Index");
            }

            var customer = await PartnerDataService.GetCustomerAsync(id);
            if (customer == null)
                return RedirectToAction("Index");

            return View(customer);
        }
    }
}

