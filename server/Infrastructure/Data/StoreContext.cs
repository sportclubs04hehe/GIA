using Core.Entities.Domain.DanhMuc;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }

        // Trong quá trình chuyển đổi, vẫn giữ lại DbSet cũ
        public DbSet<Dm_NhomHangHoa> NhomHangHoas { get; set; }
        public DbSet<Dm_HangHoa> HangHoas { get; set; }
        public DbSet<Dm_DonViTinh> DonViTinhs { get; set; }
        public DbSet<Dm_HangHoaThiTruong> MatHangs { get; set; }
        public DbSet<Dm_ThuocTinh> ThuocTinhs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình cũ - giữ lại trong quá trình chuyển đổi
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
                
            modelBuilder.Entity<Dm_HangHoa>()
                .HasOne(h => h.DonViTinh)
                .WithMany(d => d.HangHoas)
                .HasForeignKey(h => h.DonViTinhId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mới cho Dm_MatHang
            // Quan hệ tự tham chiếu (cha-con)
            modelBuilder.Entity<Dm_HangHoaThiTruong>()
                .HasOne(m => m.MatHangCha)
                .WithMany(m => m.MatHangCon)
                .HasForeignKey(m => m.MatHangChaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Dm_HangHoaThiTruong>()
                .HasOne(m => m.DonViTinh)
                .WithMany(d => d.MatHangs)
                .HasForeignKey(m => m.DonViTinhId)
                .OnDelete(DeleteBehavior.Restrict);

            // Chỉ mục
            modelBuilder.Entity<Dm_HangHoaThiTruong>()
                .HasIndex(m => new { m.Ma, m.MatHangChaId })
                .IsUnique();

            modelBuilder.Entity<Dm_HangHoaThiTruong>()
                .HasIndex(m => m.LoaiMatHang);

            modelBuilder.Entity<Dm_HangHoaThiTruong>()
                .HasIndex(m => m.MatHangChaId);
            
            modelBuilder.Entity<Dm_HangHoaThiTruong>()
                .HasIndex(m => m.IsDelete);
        }
    }
}
