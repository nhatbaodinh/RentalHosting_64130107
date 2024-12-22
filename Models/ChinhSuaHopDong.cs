namespace RentalHosting_64130107.Models;

public class ChinhSuaHopDong
{
    public int HopDongId { get; set; }
    public int NguoiDungId { get; set; }
    public int NhanVienId { get; set; }
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public int TrangThai { get; set; }
    public int SelectedHostingId { get; set; }
    public decimal DonGia { get; set; }
    
    public List<HostingModel_64130107>? Hosting { get; set; }
}