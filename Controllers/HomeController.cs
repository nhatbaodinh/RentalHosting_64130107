using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using RentalHosting_64130107.Models;
using VietQRHelper;

namespace RentalHosting_64130107.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult KhuyenMai()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult ThanhToan()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ThanhToan(decimal amount, string purpose)
        {
            if (amount <= 0 || string.IsNullOrWhiteSpace(purpose))
            {
                ViewBag.Error = "Vui lòng nhập số tiền và nội dung chuyển khoản hợp lệ!";
                return View();
            }

            try
            {
                string bankName = "TPBank";
                string bankBin = "970423";
                string bankAccount = "44414022004";

                // Tạo mã QR VietQR
                var qrPay = QRPay.InitVietQR(
                    bankBin: bankBin,
                    bankNumber: bankAccount,
                    amount: amount.ToString("F0"),
                    purpose: purpose
                );

                // Xây dựng nội dung QR
                var qrContent = qrPay.Build();

                // Tạo hình ảnh mã QR
                string qrCodeImage;
                using (var qrGenerator = new QRCodeGenerator())
                {
                    var qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
                    using (var qrCode = new PngByteQRCode(qrCodeData))
                    {
                        qrCodeImage = "data:image/png;base64," + Convert.ToBase64String(qrCode.GetGraphic(20));
                    }
                }

                // Truyền thông tin vào ViewModel
                var viewModel = new ThanhToanModel_64130107
                {
                    BankName = bankName,
                    BankAccount = bankAccount,
                    Amount = amount,
                    Purpose = purpose,
                    QRCodeImage = qrCodeImage
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo mã QR");
                ViewBag.Error = "Có lỗi xảy ra trong quá trình tạo mã QR!";
                return View();
            }
        }
    }
}