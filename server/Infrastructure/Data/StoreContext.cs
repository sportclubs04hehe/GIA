using Core.Entities.Domain.DanhMuc;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }

        public DbSet<Dm_NhomHangHoa> NhomHangHoas { get; set; }
        public DbSet<Dm_HangHoa> HangHoas { get; set; }
        public DbSet<Dm_DonViTinh> DonViTinhs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Dm_NhomHangHoa>()
            .HasOne(n => n.NhomCha)
            .WithMany(n => n.NhomCon)
            .HasForeignKey(n => n.NhomChaId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Dm_HangHoa>()
                .HasOne(h => h.NhomHangHoa)
                .WithMany(n => n.HangHoas)
                .HasForeignKey(h => h.NhomHangHoaId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Configure the relationship between HangHoa and DonViTinh
            modelBuilder.Entity<Dm_HangHoa>()
                .HasOne(h => h.DonViTinh)
                .WithMany(d => d.HangHoas)
                .HasForeignKey(h => h.DonViTinhId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
        }
    }
}
