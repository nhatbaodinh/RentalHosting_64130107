using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RentalHosting_64130107.Models
{
    public class HopDongModel_64130107
    {
        [Key]
        public int HopDongId { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int TrangThai { get; set; }
        public int NguoiDungId { get; set; }
        public int NhanVienId { get; set; }
        public List<ChiTietHopDongModel_64130107>? ChiTietHopDong { get; set; }
    }
}