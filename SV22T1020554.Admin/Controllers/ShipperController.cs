using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SV22T1020554.BusinessLayers;
using SV22T1020554.Models.Common;
using SV22T1020554.Models.Partner;

namespace SV22T1020554.Admin.Controllers
{
    [Authorize]
    public class ShipperController : Controller
    {
        /// <summary>
        /// Hi?n th? danh sách ðõn v? v?n chuy?n
        /// </summary>
        private const int PAGE_SIZE = 20;
        private const string SEARCH_CONDITION = "ShipperSearchCondition";

        /// <summary>
        /// Hi?n th? danh sách ðõn v? v?n chuy?n
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

            var data = await PartnerDataService.ListShippersAsync(input);
            ViewBag.SearchValue = input.SearchValue;
            return View(data);
        }

        /// <summary>
        /// Hi?n th? form thêm ðõn v? v?n chuy?n m?i
        /// </summary>
        public IActionResult Create()
        {
            ViewBag.Title = "B? sung ngý?i giao hàng";
            var data = new Shipper()
            {
                ShipperID = 0
            };
            return View("Edit", data);
        }

        /// <summary>
        /// C?p nh?t thông tin ðõn v? v?n chuy?n
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "C?p nh?t ngý?i giao hàng";
            var data = await PartnerDataService.GetShipperAsync(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }

        /// <summary>
        /// Lýu d? li?u
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Save(Shipper data)
        {
            if (string.IsNullOrWhiteSpace(data.ShipperName))
                ModelState.AddModelError(nameof(data.ShipperName), "Tên ngý?i giao hàng không ðý?c ð? tr?ng");

            if (!ModelState.IsValid)
            {
                ViewBag.Title = data.ShipperID == 0 ? "B? sung ngý?i giao hàng" : "C?p nh?t ngý?i giao hàng";
                return View("Edit", data);
            }

            if (data.ShipperID == 0)
            {
                await PartnerDataService.AddShipperAsync(data);
            }
            else
            {
                await PartnerDataService.UpdateShipperAsync(data);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Xóa ðõn v? v?n chuy?n
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                bool result = await PartnerDataService.DeleteShipperAsync(id);
                if (!result)
                    TempData["Message"] = "Không th? xóa ngý?i giao hàng này v? ðang có d? li?u liên quan";
                return RedirectToAction("Index");
            }

            var data = await PartnerDataService.GetShipperAsync(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }
    }
}

