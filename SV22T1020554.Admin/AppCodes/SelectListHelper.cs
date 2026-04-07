using Microsoft.AspNetCore.Mvc.Rendering;
using SV22T1020554.BusinessLayers;
using SV22T1020554.Models.Common;
using SV22T1020554.Models.Catalog;
using SV22T1020554.Models.Partner;
using SV22T1020554.Models.Sales;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SV22T1020554.Admin
{
    public static class SelectListHelper
    {
        /// <summary>
        /// Danh sách t?nh thành
        /// </summary>
        public static async Task<List<SelectListItem>> Provinces()
        {
            var list = new List<SelectListItem>();
            var data = await DictionaryDataService.ListProvincesAsync();
            foreach (var item in data)
            {
                list.Add(new SelectListItem()
                {
                    Value = item.ProvinceName,
                    Text = item.ProvinceName
                });
            }
            return list;
        }

        /// <summary>
        /// Danh sách lo?i hàng
        /// </summary>
        public static async Task<List<SelectListItem>> Categories()
        {
            var list = new List<SelectListItem>();
            var data = await CatalogDataService.ListCategoriesAsync(new PaginationSearchInput() { Page = 1, PageSize = 0 });
            foreach (var item in data.DataItems)
            {
                list.Add(new SelectListItem()
                {
                    Value = item.CategoryID.ToString(),
                    Text = item.CategoryName
                });
            }
            return list;
        }

        /// <summary>
        /// Danh sách nhà cung c?p
        /// </summary>
        public static async Task<List<SelectListItem>> Suppliers()
        {
            var list = new List<SelectListItem>();
            var data = await PartnerDataService.ListSuppliersAsync(new PaginationSearchInput() { Page = 1, PageSize = 0 });
            foreach (var item in data.DataItems)
            {
                list.Add(new SelectListItem()
                {
                    Value = item.SupplierID.ToString(),
                    Text = item.SupplierName
                });
            }
            return list;
        }

        /// <summary>
        /// Danh sách tr?ng thái ðõn hàng
        /// </summary>
        public static List<SelectListItem> OrderStatuses()
        {
            var list = new List<SelectListItem>();
            foreach (OrderStatusEnum status in typeof(OrderStatusEnum).GetEnumValues())
            {
                list.Add(new SelectListItem()
                {
                    Value = ((int)status).ToString(),
                    Text = status.GetDescription()
                });
            }
            return list;
        }
    }
}
