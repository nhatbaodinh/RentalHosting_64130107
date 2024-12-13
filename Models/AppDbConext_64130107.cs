using Microsoft.EntityFrameworkCore;
using RentalHosting_64130107.Models;

namespace RentalHosting_64130107.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<NguoiDungModel_64130107> NguoiDung { get; set; }
    public DbSet<LoaiHostingModel_64130107> LoaiHosting { get; set; }
    public DbSet<HostingModel_64130107> Hosting { get; set; }
    public DbSet<HopDongModel_64130107> HopDong { get; set; }
    public DbSet<ChiTietHopDongModel_64130107> ChiTietHopDong { get; set; }
    
}