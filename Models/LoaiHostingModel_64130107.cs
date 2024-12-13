using System.ComponentModel.DataAnnotations;

namespace RentalHosting_64130107.Models;

public class LoaiHostingModel_64130107
{
    [Key]
    public int LoaiHostingId { get; set; }
    public string? TenLoai { get; set; }
}