using Microsoft.EntityFrameworkCore;

namespace RentalHosting_64130107.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<NguoiDungModel_64130107> NguoiDung { get; set; }
    public DbSet<HostingModel_64130107> Hosting { get; set; }
    public DbSet<HopDongModel_64130107> HopDong { get; set; }
    public DbSet<ChiTietHopDongModel_64130107> ChiTietHopDong { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<ChiTietHopDongModel_64130107>()
            .HasOne(ct => ct.HopDong)
            .WithMany(h => h.ChiTietHopDong)
            .HasForeignKey(ct => ct.HopDongId);

        modelBuilder.Entity<ChiTietHopDongModel_64130107>()
            .HasOne(ct => ct.Hosting)
            .WithMany()
            .HasForeignKey(ct => ct.HostingId);
        
        modelBuilder.Entity<ChiTietHopDongModel_64130107>()
            .HasKey(c => new { c.HopDongId, c.HostingId });
    }
}