namespace RentalHosting_64130107.Models;

public class ThanhToanModel_64130107
{
    public string BankName { get; set; }
    public string BankAccount { get; set; }
    public string Purpose { get; set; }
    public string QRCodeImage { get; set; }
    public int UserId { get; set; }
    public int ContractId { get; set; }
    public decimal TotalPrice { get; set; }
    public string PaymentStatus { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public int HopDongId { get; set; }
    public string TenHosting { get; set; }
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
}