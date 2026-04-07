using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SV22T1020554.BusinessLayers;
using SV22T1020554.Models.Common;
using SV22T1020554.Models.Catalog;

namespace SV22T1020554.Admin.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string SEARCH_CONDITION = "CategorySearchCondition";

        /// <summary>
        /// Hi?n th? danh sách lo?i hàng
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

            var data = await CatalogDataService.ListCategoriesAsync(input);
            ViewBag.SearchValue = input.SearchValue;
            return View(data);
        }

        /// <summary>
        /// Hi?n th? form thêm lo?i hàng m?i
        /// </summary>
        public IActionResult Create()
        {
            ViewBag.Title = "B? sung lo?i hàng";
            var data = new Category()
            {
                CategoryID = 0
            };
            return View("Edit", data);
        }

        /// <summary>
        /// C?p nh?t thông tin lo?i hàng
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "C?p nh?t lo?i hàng";
            var data = await CatalogDataService.GetCategoryAsync(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }

        /// <summary>
        /// Lýu d? li?u
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Save(Category data)
        {
            if (string.IsNullOrWhiteSpace(data.CategoryName))
                ModelState.AddModelError(nameof(data.CategoryName), "Tên lo?i hàng không ðý?c ð? tr?ng");

            if (!ModelState.IsValid)
            {
                ViewBag.Title = data.CategoryID == 0 ? "B? sung lo?i hàng" : "C?p nh?t lo?i hàng";
                return View("Edit", data);
            }

            if (data.CategoryID == 0)
            {
                await CatalogDataService.AddCategoryAsync(data);
            }
            else
            {
                await CatalogDataService.UpdateCategoryAsync(data);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Xóa lo?i hàng
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                bool result = await CatalogDataService.DeleteCategoryAsync(id);
                if (!result)
                    TempData["Message"] = "Không th? xóa lo?i hàng này v? ðang có d? li?u liên quan";
                return RedirectToAction("Index");
            }

            var data = await CatalogDataService.GetCategoryAsync(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }
    }
}

