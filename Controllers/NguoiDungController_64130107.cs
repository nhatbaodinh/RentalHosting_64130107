using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RentalHosting_64130107.Models;
using Microsoft.EntityFrameworkCore;

namespace RentalHosting_64130107.Controllers
{
    public class NguoiDungController_64130107 : Controller
    {
        private readonly AppDbContext _context;

        public NguoiDungController_64130107(AppDbContext context)
        {
            _context = context;
        }

        // GET: Đăng ký
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Đăng ký
        [HttpPost]
        public async Task<IActionResult> Register(NguoiDungModel_64130107 model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem email đã tồn tại chưa
                if (_context.NguoiDung.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng.");
                    return View(model);
                }

                // Lưu thông tin người dùng mới vào cơ sở dữ liệu
                _context.NguoiDung.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // GET: Đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Đăng nhập
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Kiểm tra email và mật khẩu
            var user = await _context.NguoiDung.FirstOrDefaultAsync(u => u.Email == email && u.MatKhau == password);
            if (user != null)
            {
                // Tạo claims cho người dùng
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.HoTen ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim(ClaimTypes.NameIdentifier, user.NguoiDungId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString()) // Lưu role (0: khách hàng, 1: nhân viên, 2: admin)
                };

                // Tạo claims identity
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Đăng nhập và lưu thông tin vào cookie
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            // Thông báo lỗi nếu đăng nhập không thành công
            ViewBag.Error = "Email hoặc mật khẩu không chính xác.";
            return View();
        }

        // POST: Đăng xuất
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Đăng xuất và xóa cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            // Lấy UserID từ Claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userId, out int userIdInt))
            {
                // Tìm người dùng trong cơ sở dữ liệu
                var user = await _context.NguoiDung.FirstOrDefaultAsync(u => u.NguoiDungId == userIdInt);

                if (user == null)
                {
                    return NotFound();
                }

                return View(user); // Trả về View với dữ liệu người dùng
            }

            return RedirectToAction("Login", "NguoiDungController_64130107");
        }

        // POST: EditProfile
        [HttpPost]
        public async Task<IActionResult> EditProfile(NguoiDungModel_64130107 model)
        {
            if (ModelState.IsValid)
            {
                // Lấy UserID từ Claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userId, out int userIdInt))
                {
                    // Kiểm tra xem email mới có bị trùng không với người khác
                    var existingUser = await _context.NguoiDung
                        .FirstOrDefaultAsync(u => u.Email == model.Email && u.NguoiDungId != userIdInt); // Lọc trùng với người khác, không phải với chính người dùng này

                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Email", "Email này đã được sử dụng. Vui lòng chọn email khác.");
                        return View(model);
                    }

                    // Tìm người dùng trong cơ sở dữ liệu
                    var user = await _context.NguoiDung.FirstOrDefaultAsync(u => u.NguoiDungId == userIdInt);

                    if (user == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin người dùng
                    user.HoTen = model.HoTen;
                    user.Email = model.Email;
                    user.SoDienThoai = model.SoDienThoai;
                    user.DiaChi = model.DiaChi;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    // Sau khi lưu, chuyển hướng lại trang chỉnh sửa thông tin cá nhân
                    TempData["SuccessMessage"] = "Thông tin của bạn đã được cập nhật thành công!";
                    return RedirectToAction("EditProfile");
                }
            }
            // Nếu ModelState không hợp lệ, quay lại trang chỉnh sửa
            return View(model);
        }
        
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            // Lấy UserID từ Claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userId, out int userIdInt))
            {
                // Tìm người dùng trong cơ sở dữ liệu
                var user = await _context.NguoiDung.FirstOrDefaultAsync(u => u.NguoiDungId == userIdInt);

                if (user == null)
                {
                    return NotFound();
                }

                return View();
            }

            return RedirectToAction("Login", "NguoiDungController_64130107");
        }
        
        // POST: ChangePassword
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            // Lấy UserID từ Claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userId, out int userIdInt))
            {
                // Tìm người dùng trong cơ sở dữ liệu
                var user = await _context.NguoiDung.FirstOrDefaultAsync(u => u.NguoiDungId == userIdInt);

                if (user == null)
                {
                    return NotFound();
                }

                // Kiểm tra mật khẩu hiện tại
                if (user.MatKhau != currentPassword)
                {
                    ModelState.AddModelError("currentPassword", "Mật khẩu hiện tại không chính xác.");
                    return View();
                }

                // Kiểm tra mật khẩu mới và xác nhận mật khẩu
                if (newPassword != confirmPassword)
                {
                    ModelState.AddModelError("confirmPassword", "Mật khẩu xác nhận không khớp.");
                    return View();
                }

                // Cập nhật mật khẩu mới
                user.MatKhau = newPassword;

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.Update(user);
                await _context.SaveChangesAsync();

                // Sau khi lưu, chuyển hướng lại trang đổi mật khẩu
                TempData["SuccessMessage"] = "Mật khẩu của bạn đã được thay đổi thành công!";
                return RedirectToAction("ChangePassword");
            }

            return RedirectToAction("Login", "NguoiDungController_64130107");
        }
    }
}