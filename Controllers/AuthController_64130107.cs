using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RentalHosting_64130107.Models;

namespace RentalHosting_64130107.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        public async Task<IActionResult> Register(KhachHangModel_64130107 model)
        {
            if (ModelState.IsValid)
            {
                if (_context.KhachHang.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng.");
                    return View(model);
                }

                // Lưu khách hàng vào cơ sở dữ liệu
                _context.KhachHang.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _context.KhachHang.FirstOrDefault(u => u.Email == email && u.MatKhau == password);
            if (user != null)
            {
                // Tạo Claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.HoTen ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim(ClaimTypes.NameIdentifier, user.CustomerID.ToString())
                };

                // Tạo ClaimsIdentity
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Đăng nhập và thiết lập Cookie
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email hoặc mật khẩu không chính xác.";
            return View();
        }

        // Action Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}