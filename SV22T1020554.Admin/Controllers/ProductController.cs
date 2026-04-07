using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using SV22T1020554.BusinessLayers;
using SV22T1020554.Models.Common;
using SV22T1020554.Models.Catalog;
using SV22T1020554.Models.Partner;

namespace SV22T1020554.Admin.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string SEARCH_CONDITION = "ProductSearchCondition";

        // --- QUẢN LÝ MẶT HÀNG ---
        /// <summary>
        /// Hiển thị danh sách mặt hàng
        /// </summary>
        public async Task<IActionResult> Index(int page = 1, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(SEARCH_CONDITION);
            if (input == null)
            {
                input = new ProductSearchInput
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0
                };
            }

            if (Request.Query.ContainsKey("searchValue") || Request.Query.ContainsKey("categoryID") || Request.Query.ContainsKey("supplierID"))
            {
                input.SearchValue = searchValue ?? "";
                input.CategoryID = categoryID;
                input.SupplierID = supplierID;
                input.MinPrice = minPrice;
                input.MaxPrice = maxPrice;
                input.Page = 1;
            }
            else if (page > 0)
            {
                input.Page = page;
            }

            ApplicationContext.SetSessionData(SEARCH_CONDITION, input);

            ViewBag.Categories = (await CatalogDataService.ListCategoriesAsync(new PaginationSearchInput { PageSize = 0 })).DataItems;
            ViewBag.Suppliers = (await PartnerDataService.ListSuppliersAsync(new PaginationSearchInput { PageSize = 0 })).DataItems;
            ViewBag.SearchInput = input;

            var data = await CatalogDataService.ListProductsAsync(input);
            return View(data);
        }

        /// <summary>
        /// Hiển thị form bổ sung mặt hàng mới
        /// </summary>
        public async Task<IActionResult> Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            ViewBag.Categories = (await CatalogDataService.ListCategoriesAsync(new PaginationSearchInput { PageSize = 0 })).DataItems;
            ViewBag.Suppliers = (await PartnerDataService.ListSuppliersAsync(new PaginationSearchInput { PageSize = 0 })).DataItems;
            var data = new Product()
            {
                ProductID = 0,
                IsSelling = true,
                Photo = "noproduct.png"
            };
            return View("Edit", data);
        }

        /// <summary>
        /// Cập nhật thông tin mặt hàng
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật mặt hàng";
            ViewBag.Categories = (await CatalogDataService.ListCategoriesAsync(new PaginationSearchInput { PageSize = 0 })).DataItems;
            ViewBag.Suppliers = (await PartnerDataService.ListSuppliersAsync(new PaginationSearchInput { PageSize = 0 })).DataItems;
            
            var data = await CatalogDataService.GetProductAsync(id);
            if (data == null)
                return RedirectToAction("Index");

            ViewBag.Photos = await CatalogDataService.ListPhotosAsync(id);
            ViewBag.Attributes = await CatalogDataService.ListAttributesAsync(id);

            return View(data);
        }

        /// <summary>
        /// Lưu dữ liệu mặt hàng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Save(Product data, string priceInput, IFormFile? uploadPhoto)
        {
            // Xử lý giá
            if (!string.IsNullOrWhiteSpace(priceInput))
            {
                if (decimal.TryParse(priceInput, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                    data.Price = price;
            }
            else
            {
                data.Price = 0;
            }

            // Kiểm tra dữ liệu
            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được để trống");
            if (data.CategoryID <= 0)
                ModelState.AddModelError(nameof(data.CategoryID), "Vui lòng chọn loại hàng");
            if (data.SupplierID <= 0)
                ModelState.AddModelError(nameof(data.SupplierID), "Vui lòng chọn nhà cung cấp");
            if (string.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError(nameof(data.Unit), "Đơn vị tính không được để trống");
            if (data.Price < 0)
                ModelState.AddModelError(nameof(data.Price), "Giá mặt hàng không hợp lệ");

            if (!ModelState.IsValid)
            {
                ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật mặt hàng";
                ViewBag.Categories = (await CatalogDataService.ListCategoriesAsync(new PaginationSearchInput { PageSize = 0 })).DataItems;
                ViewBag.Suppliers = (await PartnerDataService.ListSuppliersAsync(new PaginationSearchInput { PageSize = 0 })).DataItems;
                return View("Edit", data);
            }

            // Xử lý ảnh
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string folder = Path.Combine(ApplicationContext.WWWRootPath, "images/products");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string filePath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadPhoto.CopyToAsync(stream);
                }
                data.Photo = fileName;

                // Đồng bộ sang Shop
                string shopFolder = Path.GetFullPath(Path.Combine(ApplicationContext.WWWRootPath, @"../../SV22T1020554.Shop/wwwroot/images/products"));
                if (!Directory.Exists(shopFolder))
                    Directory.CreateDirectory(shopFolder);
                string shopFilePath = Path.Combine(shopFolder, fileName);
                System.IO.File.Copy(filePath, shopFilePath, true);
            }

            if (data.ProductID == 0)
            {
                await CatalogDataService.AddProductAsync(data);
                TempData["Message"] = $"Đã bổ sung mặt hàng <strong>{data.ProductName}</strong> thành công";
            }
            else
            {
                await CatalogDataService.UpdateProductAsync(data);
                TempData["Message"] = $"Đã cập nhật mặt hàng <strong>{data.ProductName}</strong> thành công";
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Xóa mặt hàng
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                bool result = await CatalogDataService.DeleteProductAsync(id);
                if (!result)
                    TempData["Message"] = "Không thể xóa mặt hàng này vì đang có dữ liệu liên quan";
                return RedirectToAction("Index");
            }

            var data = await CatalogDataService.GetProductAsync(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }

        // --- QUẢN LÝ THUỘC TÍNH SẢN PHẨM ---
        /// <summary>
        /// Hiển thị form thêm/sửa thuộc tính cho mặt hàng
        /// </summary>
        public async Task<IActionResult> Attribute(int id, string method, long attributeId = 0)
        {
            var product = await CatalogDataService.GetProductAsync(id);
            if (product == null) return RedirectToAction("Index");

            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính";
                    var dataAdd = new ProductAttribute() { ProductID = id, AttributeID = 0 };
                    return View("EditAttribute", dataAdd);
                case "edit":
                    ViewBag.Title = "Cập nhật thuộc tính";
                    var dataEdit = await CatalogDataService.GetAttributeAsync(attributeId);
                    if (dataEdit == null) return RedirectToAction("Edit", new { id = id });
                    return View("EditAttribute", dataEdit);
                case "delete":
                    await CatalogDataService.DeleteAttributeAsync(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Edit", new { id = id });
            }
        }

        /// <summary>
        /// Lưu thuộc tính sản phẩm
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SaveAttribute(ProductAttribute data)
        {
            if (string.IsNullOrWhiteSpace(data.AttributeName))
                ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được để trống");
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
                ModelState.AddModelError(nameof(data.AttributeValue), "Giá trị thuộc tính không được để trống");
            if (data.DisplayOrder <= 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị không hợp lệ");

            if (!ModelState.IsValid)
            {
                ViewBag.Title = data.AttributeID == 0 ? "Bổ sung thuộc tính" : "Cập nhật thuộc tính";
                return View("EditAttribute", data);
            }

            if (data.AttributeID == 0)
                await CatalogDataService.AddAttributeAsync(data);
            else
                await CatalogDataService.UpdateAttributeAsync(data);

            return RedirectToAction("Edit", new { id = data.ProductID });
        }

        // --- QUẢN LÝ THƯ VIỆN ẢNH ---
        /// <summary>
        /// Hiển thị form thêm/sửa ảnh cho mặt hàng
        /// </summary>
        public async Task<IActionResult> Photo(int id, string method, long photoId = 0)
        {
            var product = await CatalogDataService.GetProductAsync(id);
            if (product == null) return RedirectToAction("Index");

            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh";
                    var dataAdd = new ProductPhoto() { ProductID = id, PhotoID = 0, Photo = "noproduct.png" };
                    return View("EditPhoto", dataAdd);
                case "edit":
                    ViewBag.Title = "Cập nhật ảnh";
                    var dataEdit = await CatalogDataService.GetPhotoAsync(photoId);
                    if (dataEdit == null) return RedirectToAction("Edit", new { id = id });
                    return View("EditPhoto", dataEdit);
                case "delete":
                    await CatalogDataService.DeletePhotoAsync(photoId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Edit", new { id = id });
            }
        }

        /// <summary>
        /// Lưu ảnh sản phẩm
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {
            if (data.DisplayOrder <= 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị không hợp lệ");
            if (data.PhotoID == 0 && uploadPhoto == null)
                ModelState.AddModelError(nameof(data.Photo), "Vui lòng chọn ảnh");

            if (!ModelState.IsValid)
            {
                ViewBag.Title = data.PhotoID == 0 ? "Bổ sung ảnh" : "Cập nhật ảnh";
                return View("EditPhoto", data);
            }

            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string folder = Path.Combine(ApplicationContext.WWWRootPath, "images/products");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                string filePath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadPhoto.CopyToAsync(stream);
                }
                data.Photo = fileName;

                // Đồng bộ sang Shop
                string shopFolder = Path.GetFullPath(Path.Combine(ApplicationContext.WWWRootPath, @"../../SV22T1020554.Shop/wwwroot/images/products"));
                if (!Directory.Exists(shopFolder))
                    Directory.CreateDirectory(shopFolder);
                string shopFilePath = Path.Combine(shopFolder, fileName);
                System.IO.File.Copy(filePath, shopFilePath, true);
            }

            if (data.PhotoID == 0)
                await CatalogDataService.AddPhotoAsync(data);
            else
                await CatalogDataService.UpdatePhotoAsync(data);

            return RedirectToAction("Edit", new { id = data.ProductID });
        }
    }
}

