using Microsoft.AspNetCore.Mvc;
using SV22T1020554.BusinessLayers;
using SV22T1020554.Models.Catalog;
using SV22T1020554.Models.Common;
using SV22T1020554.Shop.Models;
using System.Diagnostics;

namespace SV22T1020554.Shop.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            // Get featured products (latest 8 products)
            var input = new ProductSearchInput
            {
                Page = 1,
                PageSize = 8,
                SearchValue = "",
                CategoryID = 0,
                MinPrice = 0,
                MaxPrice = 0
            };

            var result = await CatalogDataService.ListProductsAsync(input);
            ViewBag.FeaturedProducts = result.DataItems;

            // Get categories
            var categories = await CatalogDataService.ListCategoriesAsync(new PaginationSearchInput
            {
                Page = 1,
                PageSize = 0
            });
            ViewBag.Categories = categories.DataItems;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

