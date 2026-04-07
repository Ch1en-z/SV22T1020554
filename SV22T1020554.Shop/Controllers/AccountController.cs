using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020554.BusinessLayers;
using SV22T1020554.Models.DataDictionary;
using System.Security.Claims;

namespace SV22T1020554.Shop.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        #region Login / Logout

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewBag.UserName = username;

            if (string.IsNullOrWhiteSpace(username))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập email";
                return View();
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập mật khẩu";
                return View();
            }

            var userAccount = await UserAccountService.AuthorizeAsync(username, password);
            if (userAccount == null)
            {
                ViewBag.ErrorMessage = "Đăng nhập thất bại. Email hoặc mật khẩu không đúng.";
                return View();
            }

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userAccount.UserId),
                new Claim(ClaimTypes.Name, userAccount.DisplayName),
                new Claim(ClaimTypes.Email, userAccount.Email),
                new Claim(ClaimTypes.Role, userAccount.RoleNames)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        #endregion

        #region Register

        [HttpGet("Register")]
        public async Task<IActionResult> Register()
        {
            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
            return View();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(string customerName, string contactName, string email,
            string province, string address, string phone, string password, string confirmPassword)
        {
            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
            ViewBag.CustomerName = customerName;
            ViewBag.ContactName = contactName;
            ViewBag.Email = email;
            ViewBag.Province = province;
            ViewBag.Address = address;
            ViewBag.Phone = phone;

            // Validation
            if (string.IsNullOrWhiteSpace(customerName))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập tên khách hàng";
                return View();
            }
            if (string.IsNullOrWhiteSpace(contactName))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập tên giao dịch";
                return View();
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập email";
                return View();
            }
            if (string.IsNullOrWhiteSpace(phone))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập số điện thoại";
                return View();
            }
            if (string.IsNullOrWhiteSpace(province))
            {
                ViewBag.ErrorMessage = "Vui lòng chọn tỉnh/thành";
                return View();
            }
            if (string.IsNullOrWhiteSpace(address))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập địa chỉ";
                return View();
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập mật khẩu";
                return View();
            }
            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                ViewBag.ErrorMessage = "Vui lòng xác nhận mật khẩu";
                return View();
            }
            if (password != confirmPassword)
            {
                ViewBag.ErrorMessage = "Xác nhận mật khẩu không khớp";
                return View();
            }

            int result = await UserAccountService.RegisterAsync(customerName, contactName, email, province, address, phone, password);
            if (result == -1)
            {
                ViewBag.ErrorMessage = "Email đã được sử dụng bởi tài khoản khác";
                return View();
            }
            if (result <= 0)
            {
                ViewBag.ErrorMessage = "Đăng ký thất bại. Vui lòng thử lại.";
                return View();
            }

            TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        #endregion

        #region Profile

        [Authorize]
        [HttpGet("Profile")]
        public async Task<IActionResult> Profile()
        {
            int customerID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var customer = await PartnerDataService.GetCustomerAsync(customerID);
            if (customer == null)
                return RedirectToAction("Login");

            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();
            return View(customer);
        }

        [Authorize]
        [HttpPost("Profile")]
        public async Task<IActionResult> Profile(string customerName, string contactName, string email,
            string province, string address, string phone)
        {
            int customerID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var customer = await PartnerDataService.GetCustomerAsync(customerID);
            if (customer == null)
                return RedirectToAction("Login");

            ViewBag.Provinces = await DictionaryDataService.ListProvincesAsync();

            customer.CustomerName = customerName ?? "";
            customer.ContactName = contactName ?? "";
            customer.Province = province;
            customer.Address = address;
            customer.Phone = phone;

            if (string.IsNullOrWhiteSpace(customerName))
            {
                ViewBag.ErrorMessage = "Tên khách hàng không được để trống";
                return View(customer);
            }

            bool result = await PartnerDataService.UpdateCustomerAsync(customer);
            if (result)
            {
                ViewBag.SuccessMessage = "Cập nhật thông tin thành công!";
            }
            else
            {
                ViewBag.ErrorMessage = "Cập nhật thất bại. Vui lòng thử lại.";
            }

            return View(customer);
        }

        #endregion

        #region Change Password

        [Authorize]
        [HttpGet("ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập đầy đủ thông tin";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.ErrorMessage = "Xác nhận mật khẩu mới không khớp";
                return View();
            }

            string email = User.FindFirstValue(ClaimTypes.Email) ?? "";

            // Verify old password
            var userAccount = await UserAccountService.AuthorizeAsync(email, oldPassword);
            if (userAccount == null)
            {
                ViewBag.ErrorMessage = "Mật khẩu cũ không đúng";
                return View();
            }

            bool result = await UserAccountService.ChangePasswordAsync(email, newPassword);
            if (result)
            {
                ViewBag.SuccessMessage = "Đổi mật khẩu thành công!";
            }
            else
            {
                ViewBag.ErrorMessage = "Đổi mật khẩu thất bại. Vui lòng thử lại.";
            }

            return View();
        }

        #endregion
    }
}

