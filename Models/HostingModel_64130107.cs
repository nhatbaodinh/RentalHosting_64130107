using System.ComponentModel.DataAnnotations;

namespace RentalHosting_64130107.Models;

public class HostingModel_64130107
{
    [Key]
    public int HostingId { get; set; }
    public string? TenHosting { get; set; }
    public string? MoTa { get; set; }
    public decimal DonGia { get; set; }
}