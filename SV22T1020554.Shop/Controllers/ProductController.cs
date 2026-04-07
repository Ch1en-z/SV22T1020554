using Microsoft.AspNetCore.Mvc;
using SV22T1020554.BusinessLayers;
using SV22T1020554.Models.Catalog;
using SV22T1020554.Models.Common;

namespace SV22T1020554.Shop.Controllers
{
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 12;

        /// <summary>
        /// Danh sách s?n ph?m v?i t?m ki?m và l?c
        /// </summary>
        public async Task<IActionResult> Index(int page = 1, string searchValue = "",
            int categoryID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            var input = new ProductSearchInput
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue,
                CategoryID = categoryID,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            var result = await CatalogDataService.ListProductsAsync(input);

            // Get categories for filter sidebar
            var categories = await CatalogDataService.ListCategoriesAsync(new PaginationSearchInput
            {
                Page = 1,
                PageSize = 0 // Get all
            });

            ViewBag.Categories = categories.DataItems;
            ViewBag.SearchValue = searchValue;
            ViewBag.CategoryID = categoryID;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(result);
        }

        /// <summary>
        /// Chi ti?t s?n ph?m
        /// </summary>
        public async Task<IActionResult> Detail(int id = 0)
        {
            if (id <= 0)
                return RedirectToAction("Index");

            var product = await CatalogDataService.GetProductAsync(id);
            if (product == null)
                return RedirectToAction("Index");

            // Get photos and attributes
            var photos = await CatalogDataService.ListPhotosAsync(id);
            var attributes = await CatalogDataService.ListAttributesAsync(id);

            ViewBag.Photos = photos;
            ViewBag.Attributes = attributes;

            return View(product);
        }
    }
}

