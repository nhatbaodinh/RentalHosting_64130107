namespace RentalHosting_64130107.Models;

public class ThanhToanModel_64130107
{
    public int PaymentId { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentStatus { get; set; }
}