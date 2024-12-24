namespace RentalHosting_64130107.Models;

public class ThanhToanModel_64130107
{
    public int UserId { get; set; }
    public int ContractId { get; set; }
    public int HopDongId { get; set; }
    public string TenHosting { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public string QRCodeImage { get; set; }
}