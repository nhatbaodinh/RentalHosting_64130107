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
            // Lấy UserID từ Claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userId, out int userIdInt))
            {
                // Lấy danh sách hợp đồng của người dùng hiện tại
                var contracts = await _context.HopDong
                    .Where(h => h.NguoiDungId == userIdInt) // Chỉ lấy hợp đồng của người dùng hiện tại
                    .Select(h => new
                    {
                        h.HopDongId,
                        HostingName = h.ChiTietHopDong
                            .Select(c => c.Hosting.TenHosting)
                            .FirstOrDefault(), // Lấy tên hosting từ bảng ChiTietHopDong
                        h.NgayBatDau,
                        h.NgayKetThuc,
                        TrangThai = h.TrangThai == 0 ? "Đang chờ kích hoạt" :
                            h.TrangThai == 1 ? "Đang hoạt động" : "Đã hủy", // Hiển thị trạng thái hợp đồng
                        SupportPerson = _context.NguoiDung
                            .Where(n => n.NguoiDungId == h.NhanVienId)
                            .Select(n => n.HoTen)
                            .FirstOrDefault() // Lấy tên nhân viên hỗ trợ
                    })
                    .ToListAsync();

                // Trả về view với dữ liệu hợp đồng
                return View(contracts);
            }

            return BadRequest("Không tìm thấy ID người dùng");
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
    }
}