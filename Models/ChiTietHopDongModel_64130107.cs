using System.ComponentModel.DataAnnotations;

namespace RentalHosting_64130107.Models
{
    public class ChiTietHopDongModel_64130107
    {
        public int HopDongId { get; set; }
        public int HostingId { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }

        public HopDongModel_64130107 HopDong { get; set; } = null!;
        public HostingModel_64130107 Hosting { get; set; } = null!;
    }
}