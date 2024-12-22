using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalHosting_64130107.Models;
using System.Security.Claims;
using System.Text;
using QRCoder;
using VietQRHelper;

namespace RentalHosting_64130107.Controllers
{
    public class HopDongController_64130107 : Controller
    {
        private readonly AppDbContext _context;

        public HopDongController_64130107(AppDbContext context, ILogger<HopDongController_64130107> logger)
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
                    return RedirectToAction("Login", "NguoiDungController_64130107");
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
                        .FirstOrDefault(),
                    SupportPersonEmail = _context.NguoiDung
                        .Where(n => n.NguoiDungId == h.NhanVienId)
                        .Select(n => n.Email)
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
            // Lấy UserID và Role từ Claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role); // Role = "2" là Admin

            // Tìm hợp đồng theo ID
            var contractDetailsQuery = from hd in _context.HopDong
                                        join cthd in _context.ChiTietHopDong on hd.HopDongId equals cthd.HopDongId
                                        join h in _context.Hosting on cthd.HostingId equals h.HostingId
                                        join nd in _context.NguoiDung on hd.NhanVienId equals nd.NguoiDungId
                                        where hd.HopDongId == id
                                        select new
                                        {
                                            HopDongId = hd.HopDongId,
                                            TenHosting = h.TenHosting,
                                            MoTa = h.MoTa, // Mô tả dịch vụ hosting
                                            NgayBatDau = hd.NgayBatDau,
                                            NgayKetThuc = hd.NgayKetThuc,
                                            TrangThai = hd.TrangThai == 0 ? "Đang chờ kích hoạt" :
                                                hd.TrangThai == 1 ? "Đang hoạt động" : "Đã hủy",
                                            Gia = cthd.DonGia, // Giá của dịch vụ trong hợp đồng
                                            NhanVienHoTen = nd.HoTen,
                                            EmailNhanVien = nd.Email // Email nhân viên hỗ trợ
                                        };

            if (role != "2") // Nếu không phải admin, chỉ lấy hợp đồng của người dùng hiện tại
            {
                if (!int.TryParse(userId, out int userIdInt))
                {
                    return RedirectToAction("Login", "NguoiDungController_64130107");
                }

                contractDetailsQuery = contractDetailsQuery.Where(hd => hd.HopDongId == id);
            }

            var contractDetails = await contractDetailsQuery.FirstOrDefaultAsync();

            if (contractDetails == null)
            {
                return NotFound("Không tìm thấy hợp đồng hoặc bạn không có quyền truy cập.");
            }

            return View(contractDetails); // Trả về view hiển thị chi tiết hợp đồng
        }
        
        [HttpGet]
        public async Task<IActionResult> RegisterContract()
        {
            var model = new RegisterContractViewModel
            {
                Hosting = await _context.Hosting.ToListAsync()
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
                    model.Hosting = await _context.Hosting.ToListAsync();
                    return View(model);
                }

                // Tìm một nhân viên ngẫu nhiên có Role = 2
                var nhanVien = await _context.NguoiDung
                    .Where(n => n.Role == 2)
                    .OrderBy(n => Guid.NewGuid()) // Chọn ngẫu nhiên
                    .FirstOrDefaultAsync();

                if (nhanVien == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy nhân viên hợp lệ.");
                    model.Hosting = await _context.Hosting.ToListAsync();
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

            return RedirectToAction("Login", "NguoiDungController_64130107");
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
        
        [HttpGet]
        public async Task<IActionResult> EditContract(int id)
        {
            // Tìm hợp đồng theo ID
            var contract = await _context.HopDong
                .Include(h => h.ChiTietHopDong)
                .ThenInclude(c => c.Hosting)
                .FirstOrDefaultAsync(h => h.HopDongId == id);

            if (contract == null)
            {
                return NotFound("Hợp đồng không tồn tại.");
            }

            // Chuẩn bị ViewModel
            var model = new ChinhSuaHopDong()
            {
                HopDongId = contract.HopDongId,
                NguoiDungId = contract.NguoiDungId,
                NhanVienId = contract.NhanVienId,
                NgayBatDau = contract.NgayBatDau,
                NgayKetThuc = contract.NgayKetThuc,
                TrangThai = contract.TrangThai,
                Hosting = await _context.Hosting.ToListAsync(),
                SelectedHostingId = contract.ChiTietHopDong.FirstOrDefault()?.HostingId ?? 0,
                DonGia = contract.ChiTietHopDong.FirstOrDefault()?.DonGia ?? 0
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditContract(ChinhSuaHopDong model)
        {
            if (!ModelState.IsValid)
            {
                model.Hosting = await _context.Hosting.ToListAsync();
                return View(model);
            }

            // Tìm hợp đồng để chỉnh sửa
            var contract = await _context.HopDong
                .Include(h => h.ChiTietHopDong)
                .FirstOrDefaultAsync(h => h.HopDongId == model.HopDongId);

            if (contract == null)
            {
                return NotFound("Hợp đồng không tồn tại.");
            }

            // Cập nhật thông tin hợp đồng
            contract.NguoiDungId = model.NguoiDungId;
            contract.NhanVienId = model.NhanVienId;
            contract.NgayBatDau = model.NgayBatDau;
            contract.NgayKetThuc = model.NgayKetThuc;
            contract.TrangThai = model.TrangThai;

            // Cập nhật chi tiết hợp đồng
            var contractDetail = contract.ChiTietHopDong.FirstOrDefault();
            if (contractDetail != null)
            {
                contractDetail.HostingId = model.SelectedHostingId;
                contractDetail.DonGia = model.DonGia;
            }

            // Lưu thay đổi
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật hợp đồng thành công.";
            return RedirectToAction(nameof(EditContract));
        }
        
        [HttpGet]
        public async Task<IActionResult> SearchContract(string contractId)
        {
            // Kiểm tra vai trò admin
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role != "2") // Role "2" là Admin
            {
                return Forbid("Bạn không có quyền truy cập tính năng này.");
            }

            // Tìm hợp đồng theo ID
            if (!int.TryParse(contractId, out int id))
            {
                TempData["ErrorMessage"] = "ID hợp đồng không hợp lệ.";
                return RedirectToAction(nameof(GetContracts));
            }

            var contract = await _context.HopDong
                .Where(h => h.HopDongId == id)
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

            if (contract == null || contract.Count == 0)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hợp đồng với ID này.";
                return RedirectToAction(nameof(GetContracts));
            }

            ViewBag.IsAdmin = true;
            return View("GetContracts", contract); // Hiển thị kết quả tìm kiếm
        }
        
        [HttpGet]
        public async Task<IActionResult> ExportContractsToCsv()
        {
            // Lấy UserID và Role từ Claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role); // Role "2" là Admin

            IQueryable<HopDongModel_64130107> contractsQuery;

            if (role == "2") // Admin
            {
                contractsQuery = _context.HopDong;
            }
            else if (int.TryParse(userId, out int userIdInt)) // Người dùng thường
            {
                contractsQuery = _context.HopDong.Where(h => h.NguoiDungId == userIdInt);
            }
            else
            {
                return RedirectToAction("Login", "NguoiDungController_64130107");
            }

            // Lấy danh sách hợp đồng
            var contracts = await contractsQuery
                .Select(h => new
                {
                    h.HopDongId,
                    HostingName = h.ChiTietHopDong
                        .Select(c => c.Hosting.TenHosting)
                        .FirstOrDefault(),
                    NgayBatDau = h.NgayBatDau.ToString("dd/MM/yyyy"),
                    NgayKetThuc = h.NgayKetThuc.ToString("dd/MM/yyyy"),
                    TrangThai = h.TrangThai == 0 ? "Đang chờ kích hoạt" :
                                h.TrangThai == 1 ? "Đang hoạt động" : "Đã hủy",
                    SupportPerson = _context.NguoiDung
                        .Where(n => n.NguoiDungId == h.NhanVienId)
                        .Select(n => n.HoTen)
                        .FirstOrDefault()
                })
                .ToListAsync();

            // Nếu không có hợp đồng
            if (!contracts.Any())
            {
                TempData["ErrorMessage"] = "Không có hợp đồng nào để xuất.";
                return RedirectToAction(nameof(GetContracts));
            }

            // Tạo dữ liệu CSV
            var csvData = new StringBuilder();

            // BOM để khắc phục lỗi font
            csvData.AppendLine("\uFEFFID Hợp Đồng,Tên Hosting,Ngày Bắt Đầu,Ngày Kết Thúc,Trạng Thái,Người Hỗ Trợ");

            foreach (var contract in contracts)
            {
                csvData.AppendLine($"{contract.HopDongId}," +
                                   $"{contract.HostingName}," +
                                   $"{contract.NgayBatDau}," +
                                   $"{contract.NgayKetThuc}," +
                                   $"{contract.TrangThai}," +
                                   $"{contract.SupportPerson}");
            }

            // Trả về file CSV với UTF-8 BOM
            var fileName = "DanhSachHopDong.csv";
            var fileBytes = Encoding.UTF8.GetBytes(csvData.ToString());
            return File(fileBytes, "text/csv; charset=utf-8", fileName);
        }
        
        [HttpGet]
        public async Task<IActionResult> Payment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userId, out int userIdInt))
            {
                var contract = await _context.HopDong
                    .Include(h => h.ChiTietHopDong)
                    .ThenInclude(c => c.Hosting)
                    .FirstOrDefaultAsync(h => h.HopDongId == id);

                if (contract == null)
                {
                    return NotFound("Hợp đồng không tồn tại.");
                }

                // Kiểm tra xem hợp đồng đã thanh toán chưa
                if (contract.TrangThai == 1)
                {
                    TempData["ErrorMessage"] = "Hợp đồng đã được thanh toán.";
                    return RedirectToAction(nameof(GetContracts));
                }

                // Tính toán số tiền cần thanh toán
                decimal totalPrice = contract.ChiTietHopDong.Sum(c => c.DonGia);

                // Tạo nội dung mã QR thanh toán
                var qrPay = QRPay.InitVietQR(
                    bankBin: BankApp.BanksObject[BankKey.TPBANK].bin,
                    bankNumber: "44414022004", // Số tài khoản
                    amount: totalPrice.ToString(),
                    purpose: $"Thanh toán hợp đồng {contract.HopDongId}" // Nội dung thanh toán
                );
                var qrContent = qrPay.Build();

                // Tạo hình ảnh QR
                string qrCodeImage;
                using (var qrGenerator = new QRCodeGenerator())
                {
                    var qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
                    using (var qrCode = new PngByteQRCode(qrCodeData))
                    {
                        qrCodeImage = "data:image/png;base64," + Convert.ToBase64String(qrCode.GetGraphic(3));
                    }
                }

                // Chuẩn bị thông tin cho ViewModel
                var model = new ThanhToanModel_64130107()
                {
                    HopDongId = contract.HopDongId,
                    TenHosting = contract.ChiTietHopDong.FirstOrDefault()?.Hosting.TenHosting,
                    NgayBatDau = contract.NgayBatDau,
                    NgayKetThuc = contract.NgayKetThuc,
                    TotalPrice = totalPrice,
                    QRCodeImage = qrCodeImage
                };

                return View(model);
            }

            return RedirectToAction("Login", "NguoiDungController_64130107");
        }
    }
}