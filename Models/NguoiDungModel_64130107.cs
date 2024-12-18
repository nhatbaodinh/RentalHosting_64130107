using System.ComponentModel.DataAnnotations;

namespace RentalHosting_64130107.Models;

public class NguoiDungModel_64130107
{
    [Key]
    public int NguoiDungId { get; set; }
    public string? HoTen { get; set; }
    public string? Email { get; set; }
    public string? MatKhau { get; set; }
    public string? SoDienThoai { get; set; }
    public string? DiaChi { get; set; }
    public int Role { get; set; }
}