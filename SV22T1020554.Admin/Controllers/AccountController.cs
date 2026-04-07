using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SV22T1020554.BusinessLayers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SV22T1020554.Admin.Controllers
{
    [Authorize] 
    [Route("Account")]
    public class AccountController : Controller
    {
        /// <summary>
    /// Hiển thị trang đăng nhập
    /// </summary>
        [AllowAnonymous]
        [HttpGet]
        [Route("Login")] 
        public IActionResult Login(string returnUrl = "/")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
    /// Xử lý đăng nhập người dùng
    /// </summary>
    /// <param name="username">Tên đăng nhập</param>
    /// <param name="password">Mật khẩu</param>
    /// <return></return>
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = "/")
        {
            ViewBag.Username = username;
            if (string.IsNullOrWhiteSpace(username))
            {
                ModelState.AddModelError("username", "Vui lòng nhập tên đăng nhập");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("password", "Vui lòng nhập mật khẩu");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }

            var userAccount = await UserAccountService.AuthorizeAsync(username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View();
            }

            var userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Email = userAccount.Email,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
            };

            await HttpContext.SignInAsync(userData.CreatePrincipal());
            return Redirect(returnUrl);
        }

        /// <summary>
    /// Hiển thị form thay đổi mật khẩu
    /// </summary>
    [HttpGet]
        [Route("ChangePassword")] 
        public IActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
    /// Xử lý thay đổi mật khẩu người dùng
    /// </summary>
    /// <param name="oldPassword">Mật khẩu cũ</param>
    /// <param name="newPassword">Mật khẩu mới</param>
    /// <return></return>
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ mật khẩu cũ và mới");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("confirmPassword", "Mật khẩu xác nhận không khớp");
                return View();
            }

            // Lấy thông tin User hiện tại từ Principal
            var userData = User.GetUserData();
            if (userData == null || string.IsNullOrWhiteSpace(userData.Email))
            {
                ModelState.AddModelError("", "Không tìm thấy thông tin định danh của người dùng.");
                return View();
            }

            string username = userData.Email;

            // Kiểm tra mật khẩu cũ để xác thực
            var account = await UserAccountService.AuthorizeAsync(username, oldPassword);
            if (account == null)
            {
                ModelState.AddModelError("oldPassword", "Mật khẩu cũ không chính xác");
                return View();
            }
            
            // Đổi mật khẩu
            bool result = await UserAccountService.ChangePasswordAsync(username, newPassword);
            if (result)
            {
                TempData["SuccessMessage"] = "Thay đổi mật khẩu tài khoản của bạn thành công!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Đã có lỗi xảy ra, vui lòng thử lại sau.");
                return View();
            }
        }

        /// <summary>
    /// Đăng xuất người dùng
    /// </summary>
    [Route("Logout")] 
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        /// <summary>
    /// Trang chính của tài khoản
    /// </summary>
    public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Trang hiển thị thông báo khi người dùng không có quyền truy cập
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

