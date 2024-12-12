using Microsoft.EntityFrameworkCore;
using RentalHosting_64130107.Models;

namespace RentalHosting_64130107.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<KhachHangModel_64130107> KhachHang { get; set; }
    
}