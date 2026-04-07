using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SV22T1020554.BusinessLayers;
using SV22T1020554.Models.Common;
using SV22T1020554.Models.Partner;

namespace SV22T1020554.Admin.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        /// <summary>
        /// Hi?n th? danh sách nhà cung c?p
        /// </summary>
        private const int PAGE_SIZE = 20;
        private const string SEARCH_CONDITION = "SupplierSearchCondition";

        /// <summary>
        /// Hi?n th? danh sách nhà cung c?p
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

            var data = await PartnerDataService.ListSuppliersAsync(input);
            ViewBag.SearchValue = input.SearchValue;
            return View(data);
        }

        /// <summary>
        /// Hi?n th? form thêm nhà cung c?p m?i
        /// </summary>
        public async Task<IActionResult> Create()
        {
            ViewBag.Title = "B? sung nhà cung c?p";
            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
            var data = new Supplier()
            {
                SupplierID = 0
            };
            return View("Edit", data);
        }

        /// <summary>
        /// C?p nh?t thông tin nhà cung c?p
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "C?p nh?t nhà cung c?p";
            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
            var data = await PartnerDataService.GetSupplierAsync(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }

        /// <summary>
        /// Lýu d? li?u
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Save(Supplier data)
        {
            // Ki?m tra d? li?u
            if (string.IsNullOrWhiteSpace(data.SupplierName))
                ModelState.AddModelError(nameof(data.SupplierName), "Tên nhà cung c?p không ðý?c ð? tr?ng");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao d?ch không ðý?c ð? tr?ng");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email không ðý?c ð? tr?ng");
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Vui l?ng ch?n t?nh/thành");

            if (!ModelState.IsValid)
            {
                ViewBag.Title = data.SupplierID == 0 ? "B? sung nhà cung c?p" : "C?p nh?t nhà cung c?p";
                ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
                return View("Edit", data);
            }

            if (data.SupplierID == 0)
            {
                await PartnerDataService.AddSupplierAsync(data);
            }
            else
            {
                await PartnerDataService.UpdateSupplierAsync(data);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Xóa thông tin nhà cung c?p
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                bool result = await PartnerDataService.DeleteSupplierAsync(id);
                if (!result)
                    TempData["Message"] = "Không th? xóa nhà cung c?p này v? ðang có d? li?u liên quan";
                return RedirectToAction("Index");
            }

            var data = await PartnerDataService.GetSupplierAsync(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }

    }
}

