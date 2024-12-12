using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RentalHosting_64130107.Models;
using Microsoft.EntityFrameworkCore;

namespace RentalHosting_64130107.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: EditProfile
        [HttpGet]
        public IActionResult EditProfile()
        {
            // Lấy CustomerID từ Claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Lấy CustomerID
            if (int.TryParse(userId, out int userIdInt))
            {
                // Tìm người dùng trong cơ sở dữ liệu theo CustomerID
                var user = _context.KhachHang.FirstOrDefault(u => u.CustomerID == userIdInt);

                if (user == null)
                {
                    return NotFound();
                }

                // Trả về View để hiển thị thông tin người dùng
                return View(user);
            }

            return BadRequest("Invalid user ID");
        }
        
        // POST: EditProfile
        [HttpPost]
        public async Task<IActionResult> EditProfile(KhachHangModel_64130107 model, string oldPassword, string newPassword, string confirmPassword)
        {
            if (ModelState.IsValid)
            {
                // Lấy CustomerID từ Claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Lấy CustomerID từ Claims
                if (int.TryParse(userId, out int userIdInt))
                {
                    // Tìm người dùng trong cơ sở dữ liệu theo CustomerID
                    var user = await _context.KhachHang.FirstOrDefaultAsync(u => u.CustomerID == userIdInt);

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

                    // Sau khi lưu, đặt thông báo thành công vào TempData
                    TempData["SuccessMessage"] = "Thông tin của bạn đã được cập nhật thành công!";

                    // Chuyển hướng lại trang chỉnh sửa thông tin cá nhân
                    return RedirectToAction("EditProfile");
                }
            }
            else
            {
                // Kiểm tra lỗi ModelState nếu không hợp lệ
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            // Nếu ModelState không hợp lệ, quay lại trang chỉnh sửa
            return View(model);
        }
        
        // GET: ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: ChangePassword
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            // Kiểm tra điều kiện hợp lệ
            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu mới và mật khẩu xác nhận không khớp.";
                return View();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userId, out int userIdInt))
            {
                var user = await _context.KhachHang.FirstOrDefaultAsync(u => u.CustomerID == userIdInt);

                if (user == null)
                {
                    return NotFound();
                }

                // Kiểm tra mật khẩu cũ
                if (user.MatKhau != oldPassword)
                {
                    TempData["ErrorMessage"] = "Mật khẩu cũ không đúng.";
                    return View();
                }

                // Cập nhật mật khẩu mới
                user.MatKhau = newPassword;

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.Update(user);
                await _context.SaveChangesAsync();

                // Thông báo thành công
                TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công!";
                return RedirectToAction("ChangePassword");
            }

            return BadRequest("Invalid user ID");
        }
    }
}