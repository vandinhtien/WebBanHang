using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020742.BusinessLayers;
using SV21T1020742.DomainModels;
using SV21T1020742.Web.AppCodes;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020742.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewBag.Username = username;

            //Kiểm tra thông tin đầu vào
            if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập đầy đủ tên và mật khẩu");
                return View();
            }

            //TODO: Kiểm tra xem username và password (của Employee) có đúng hay không?
            var userAccount = UserAccountService.Authorize(UserTypes.Employee, username, password);
            if(userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }

            //ĐĂNG NHẬP THÀNH CÔNG

            //1. Tạo ra thông tin "định danh" người dùng
            WebUserData userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList()

            };

            //2. Ghi nhận trạng thái nhập
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userData.CreatePrincipal());


            //3. Quay về trang chủ
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (Request.Method == "GET")
                return View();

            if (string.IsNullOrWhiteSpace(oldPassword))
                ModelState.AddModelError("oldPassword", "Vui lòng nhập mật khẩu hiện tại");

            if (UserAccountService.Authorize(UserTypes.Employee, User.FindFirstValue("UserName") ?? "", oldPassword) == null)
                ModelState.AddModelError("oldPassword", "Mật khẩu hiện tại không đúng");
            else
                ViewBag.oldPassword = oldPassword;

            if (string.IsNullOrWhiteSpace(newPassword))
                ModelState.AddModelError("newPassword", "Vui lòng nhập mật khẩu mới");

            if (newPassword.Length < 6)
                ModelState.AddModelError("newPassword", "Mật khẩu mới phải có độ dài tối thiểu 6 ký tự. Vui lòng nhập lại");

            if (newPassword != confirmPassword)
                ModelState.AddModelError("confirmPassword", "Vui lòng đảm bảo mật khẩu mới và xác nhận mật khẩu khớp nhau");

            if (!ModelState.IsValid)
                return View();

            UserAccountService.ChangePassword(UserTypes.Employee, User.FindFirstValue("UserName") ?? "", newPassword);

            return Redirect("~/");
        }

        public IActionResult AccessDenined()
        {
            return View();
        }
    }
}
