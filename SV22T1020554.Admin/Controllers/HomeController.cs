using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SV22T1020554.Admin.Models;
using SV22T1020554.BusinessLayers;

namespace SV22T1020554.Admin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var stats = await DashboardService.GetStatisticsAsync();
            var monthlyRevenue = await DashboardService.GetMonthlyRevenueAsync();
            var pendingOrders = await DashboardService.GetPendingOrdersAsync(5);
            var topProducts = await DashboardService.GetTopSellingProductsAsync(5);

            ViewBag.Statistics = stats;
            ViewBag.MonthlyRevenue = monthlyRevenue;
            ViewBag.PendingOrders = pendingOrders;
            ViewBag.TopProducts = topProducts;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
