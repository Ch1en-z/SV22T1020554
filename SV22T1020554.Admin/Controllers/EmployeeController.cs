using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SV22T1020554.BusinessLayers;
using SV22T1020554.Models.Common;
using SV22T1020554.Models.HR;

namespace SV22T1020554.Admin.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {

        /// <summary>
        /// Hiển thị danh sách nhân viên
        /// </summary>
        private const int PAGE_SIZE = 20;
        private const string SEARCH_CONDITION = "EmployeeSearchCondition";

        /// <summary>
        /// Hiển thị danh sách nhân viên
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

            var data = await HRDataService.ListEmployeesAsync(input);
            ViewBag.SearchValue = input.SearchValue;
            return View(data);
        }

        /// <summary>
        /// Hiển thị form tạo nhân viên mới
        /// </summary>
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";
            var data = new Employee()
            {
                EmployeeID = 0,
                IsWorking = true,
                Photo = "nophoto.png"
            };
            return View("Edit", data);
        }

        /// <summary>
        /// Cập nhật thông tin nhân viên
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật nhân viên";
            var data = await HRDataService.GetEmployeeAsync(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }

        /// <summary>
        /// Lưu dữ liệu
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Save(Employee data, string birthDateInput, IFormFile? uploadPhoto)
        {
            // Xử lý ngày sinh
            if (!string.IsNullOrWhiteSpace(birthDateInput))
            {
                DateTime d = DateTime.ParseExact(birthDateInput, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                data.BirthDate = d;
            }

            // Kiểm tra dữ liệu
            if (string.IsNullOrWhiteSpace(data.FullName))
                ModelState.AddModelError(nameof(data.FullName), "Họ tên không được để trống");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email không được để trống");

            // Kiểm tra email trùng
            bool isEmailValid = await HRDataService.ValidateEmployeeEmailAsync(data.Email, data.EmployeeID);
            if (!isEmailValid)
                ModelState.AddModelError(nameof(data.Email), "Email đã được sử dụng bởi nhân viên khác");

            if (!ModelState.IsValid)
            {
                ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật nhân viên";
                return View("Edit", data);
            }

            // Xử lý ảnh
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string folder = Path.Combine(ApplicationContext.WWWRootPath, "images/employees");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string filePath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadPhoto.CopyToAsync(stream);
                }
                data.Photo = fileName;

                // Đồng bộ sang Shop
                string shopFolder = Path.GetFullPath(Path.Combine(ApplicationContext.WWWRootPath, @"../../SV22T1020554.Shop/wwwroot/images/employees"));
                if (!Directory.Exists(shopFolder))
                    Directory.CreateDirectory(shopFolder);
                string shopFilePath = Path.Combine(shopFolder, fileName);
                System.IO.File.Copy(filePath, shopFilePath, true);
            }

            if (data.EmployeeID == 0)
            {
                await HRDataService.AddEmployeeAsync(data);
            }
            else
            {
                await HRDataService.UpdateEmployeeAsync(data);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Xóa nhân viên
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            if (Request.Method == "POST")
            {
                bool result = await HRDataService.DeleteEmployeeAsync(id);
                if (!result)
                    TempData["Message"] = "Không thể xóa nhân viên này vì đang có dữ liệu liên quan";
                return RedirectToAction("Index");
            }

            var data = await HRDataService.GetEmployeeAsync(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }
        /// <summary>
        /// Hiển thị trang thay đổi quyền hạn cho nhân viên
        /// </summary>
        /// <param name="id">Mã nhân viên</param>
        /// <return></return>
        public async Task<IActionResult> ChangeRole(int id)
        {
            var data = await HRDataService.GetEmployeeAsync(id);
            if (data == null)
                return RedirectToAction("Index");

            // Danh sách tất cả các quyền hạn có trong hệ thống
            var allRoles = new List<RoleItem>()
            {
                new RoleItem { Name = "Quản lý khách hàng", Desc = "Thêm, sửa, xóa và xem danh sách khách hàng" },
                new RoleItem { Name = "Quản lý mặt hàng", Desc = "Quản lý thông tin sản phẩm, giá bán, trạng thái" },
                new RoleItem { Name = "Quản lý đơn hàng", Desc = "Tạo, xử lý và theo dõi đơn hàng" },
                new RoleItem { Name = "Quản trị hệ thống", Desc = "Quản lý người dùng, phân quyền, cấu hình hệ thống" }
            };

            // Tách chuỗi RoleNames hiện tại của nhân viên thành mảng để so khớp
            var currentRoles = (data.RoleNames ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                      .Select(r => r.Trim())
                                                      .ToList();

            foreach (var r in allRoles)
            {
                r.Selected = currentRoles.Contains(r.Name);
            }

            ViewBag.AllRoles = allRoles;

            return View(data);
        }

        /// <summary>
        /// Xử lý lưu thay đổi quyền hạn nhân viên
        /// </summary>
        /// <param name="EmployeeID">Mã nhân viên</param>
        /// <param name="roleNames">Danh sách quyền được chọn</param>
        /// <return></return>
        [HttpPost]
        public async Task<IActionResult> ChangeRole(int id, string[] roleNames)
        {
            string roles = roleNames != null ? string.Join(";", roleNames) : "";
            await HRDataService.ChangeRoleNamesAsync(id, roles);
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Hiển thị form đổi mật khẩu cho nhân viên
        /// </summary>
        /// <param name="id">Mã nhân viên</param>
        /// <return></return>
        public async Task<IActionResult> ChangePassword(int id)
        {
            var data = await HRDataService.GetEmployeeAsync(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }

        /// <summary>
        /// Xử lý lưu mật khẩu mới cho nhân viên
        /// </summary>
        /// <param name="id">Mã nhân viên</param>
        /// <param name="newPassword">Mật khẩu mới</param>
        /// <param name="confirmPassword">Nhập lại mật khẩu mới</param>
        /// <return></return>
        [HttpPost]
        public async Task<IActionResult> ChangePassword(int id, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                ModelState.AddModelError("newPassword", "Vui lòng nhập mật khẩu mới");
            }
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("confirmPassword", "Mật khẩu xác nhận không khớp");
            }

            if (!ModelState.IsValid)
            {
                var data = await HRDataService.GetEmployeeAsync(id);
                return View(data);
            }

            await HRDataService.ChangePasswordAsync(id, newPassword);
            return RedirectToAction("Index");
        }
    }

    /// <summary>
    /// Đối tượng dùng để hiển thị danh sách quyền trên giao diện
    /// </summary>
    public class RoleItem
    {
        public string Name { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public bool Selected { get; set; }
    }
}

