using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalHosting_64130107.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RentalHosting_64130107.Controllers
{
    public class HopDongController_64130107 : Controller
    {
        private readonly AppDbContext _context;

        public HopDongController_64130107(AppDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách hợp đồng của người dùng
        [HttpGet]
        public async Task<IActionResult> GetContracts()
        {
            // Lấy UserID và vai trò từ Claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role); // Giả sử vai trò được lưu trong Claim

            IQueryable<HopDongModel_64130107> contractsQuery;

            if (role == "2")
            {
                // Nếu là Admin, lấy toàn bộ hợp đồng
                contractsQuery = _context.HopDong;
            }
            else
            {
                // Nếu không phải Admin, chỉ lấy hợp đồng của người dùng hiện tại
                if (!int.TryParse(userId, out int userIdInt))
                {
                    return BadRequest("Không tìm thấy ID người dùng");
                }
                contractsQuery = _context.HopDong.Where(h => h.NguoiDungId == userIdInt);
            }

            // Dựng danh sách hợp đồng
            var contracts = await contractsQuery
                .Select(h => new
                {
                    h.HopDongId,
                    HostingName = h.ChiTietHopDong
                        .Select(c => c.Hosting.TenHosting)
                        .FirstOrDefault(),
                    h.NgayBatDau,
                    h.NgayKetThuc,
                    TrangThai = h.TrangThai == 0 ? "Đang chờ kích hoạt" :
                        h.TrangThai == 1 ? "Đang hoạt động" : "Đã hủy",
                    SupportPerson = _context.NguoiDung
                        .Where(n => n.NguoiDungId == h.NhanVienId)
                        .Select(n => n.HoTen)
                        .FirstOrDefault()
                })
                .ToListAsync();

            // Truyền dữ liệu hợp đồng và vai trò người dùng tới view
            ViewBag.IsAdmin = role == "2";
            return View(contracts);
        }

        // Lấy chi tiết hợp đồng
        [HttpGet]
        public async Task<IActionResult> ContractDetails(int id)
        {
            // Lấy UserID từ Claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userId, out int userIdInt))
            {
                // Tìm hợp đồng thuộc về người dùng hiện tại và có ID khớp
                var contractDetails = await (from hd in _context.HopDong
                    join cthd in _context.ChiTietHopDong on hd.HopDongId equals cthd.HopDongId
                    join h in _context.Hosting on cthd.HostingId equals h.HostingId
                    join nd in _context.NguoiDung on hd.NhanVienId equals nd.NguoiDungId
                    where hd.HopDongId == id && hd.NguoiDungId == userIdInt
                    select new
                    {
                        HopDongId = hd.HopDongId,
                        TenHosting = h.TenHosting,
                        MoTa = h.MoTa, // Thêm mô tả dịch vụ hosting
                        NgayBatDau = hd.NgayBatDau,
                        NgayKetThuc = hd.NgayKetThuc,
                        TrangThai = hd.TrangThai == 0 ? "Đang chờ kích hoạt" :
                            hd.TrangThai == 1 ? "Đang hoạt động" : "Đã hủy",
                        Gia = cthd.DonGia, // Giá của dịch vụ trong hợp đồng
                        NhanVienHoTen = nd.HoTen,
                        EmailNhanVien = nd.Email // Email nhân viên hỗ trợ
                    }).FirstOrDefaultAsync();

                if (contractDetails == null)
                {
                    return NotFound("Không tìm thấy hợp đồng hoặc không có quyền truy cập.");
                }

                return View(contractDetails); // Trả về view hiển thị chi tiết
            }

            return BadRequest("Không tìm thấy ID người dùng");
        }
        
        [HttpGet]
        public async Task<IActionResult> RegisterContract()
        {
            var model = new RegisterContractViewModel
            {
                Hostings = await _context.Hosting.ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterContract(RegisterContractViewModel model)
        {
            // Lấy UserID từ Claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userId, out int userIdInt))
            {
                // Kiểm tra xem HostingId có hợp lệ không
                var hosting = await _context.Hosting.FindAsync(model.HostingId);
                if (hosting == null)
                {
                    ModelState.AddModelError("", "Hosting không hợp lệ.");
                    model.Hostings = await _context.Hosting.ToListAsync();
                    return View(model);
                }

                // Tìm một nhân viên ngẫu nhiên có Role = 1
                var nhanVien = await _context.NguoiDung
                    .Where(n => n.Role == 1)
                    .OrderBy(n => Guid.NewGuid()) // Chọn ngẫu nhiên
                    .FirstOrDefaultAsync();

                if (nhanVien == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy nhân viên hợp lệ.");
                    model.Hostings = await _context.Hosting.ToListAsync();
                    return View(model);
                }

                // Tính toán ngày bắt đầu và kết thúc
                DateTime startDate = DateTime.Now; // Ngày bắt đầu mặc định là hôm nay
                DateTime endDate = startDate.AddDays(30 * model.Months); // Cộng 30 ngày cho mỗi tháng

                // Tính số tiền phải trả
                decimal totalPrice = hosting.DonGia * model.Months;

                // Tạo hợp đồng mới
                var contract = new HopDongModel_64130107
                {
                    NguoiDungId = userIdInt,
                    NhanVienId = nhanVien.NguoiDungId, // Gán NhanVienId hợp lệ
                    NgayBatDau = startDate,
                    NgayKetThuc = endDate,
                    TrangThai = 0, // Trạng thái ban đầu là đang chờ kích hoạt
                    ChiTietHopDong = new List<ChiTietHopDongModel_64130107>
                    {
                        new ChiTietHopDongModel_64130107
                        {
                            HostingId = model.HostingId,
                            SoLuong = 1, // Đăng ký 1 dịch vụ hosting
                            DonGia = hosting.DonGia
                        }
                    }
                };

                // Thêm hợp đồng vào cơ sở dữ liệu
                _context.HopDong.Add(contract);
                await _context.SaveChangesAsync();

                // Trả về thông tin hợp đồng đã đăng ký cho người dùng (ví dụ: thông báo thành công và thông tin hợp đồng)
                TempData["SuccessMessage"] = $"Đăng ký dịch vụ thành công! Số tiền phải trả: {totalPrice} VND. Ngày kết thúc: {endDate.ToString("dd/MM/yyyy")}.";
                return RedirectToAction("RegisterContract"); // Redirect đến trang danh sách hợp đồng
            }

            return BadRequest("Không tìm thấy ID người dùng");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteContract(int id)
        {
            // Lấy UserID và Role từ Claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role); // Role = "2" là Admin

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Không tìm thấy ID người dùng.");
            }

            // Tìm hợp đồng theo ID
            var contract = await _context.HopDong
                .Include(h => h.ChiTietHopDong) // Bao gồm chi tiết hợp đồng
                .FirstOrDefaultAsync(h => h.HopDongId == id);

            if (contract == null)
            {
                return NotFound("Hợp đồng không tồn tại.");
            }

            // Kiểm tra quyền: Admin hoặc chủ sở hữu hợp đồng
            if (role != "2" && contract.NguoiDungId.ToString() != userId)
            {
                return Forbid("Bạn không có quyền xóa hợp đồng này.");
            }

            // Xóa chi tiết hợp đồng trước
            _context.ChiTietHopDong.RemoveRange(contract.ChiTietHopDong);

            // Xóa hợp đồng chính
            _context.HopDong.Remove(contract);

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa hợp đồng thành công.";
            return RedirectToAction(nameof(GetContracts));
        }
    }
}