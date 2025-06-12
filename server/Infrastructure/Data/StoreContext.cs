using Core.Entities.Domain.DanhMuc;
using Core.Entities.Domain.NghiepVu;
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
        public DbSet<Dm_HangHoaThiTruong> MatHangs { get; set; }
        public DbSet<Dm_ThuocTinh> ThuocTinhs { get; set; }
        public DbSet<Dm_LoaiGia> LoaiGias { get; set; }
        public DbSet<ThuThapGiaThiTruong> ThuThapGiaThiTruongs { get; set; }


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

            modelBuilder.Entity<ThuThapGiaThiTruong>()
                .HasOne(t => t.HangHoa)
                .WithMany(h => h.ThuThapGiaThiTruongs)
                .HasForeignKey(t => t.HangHoaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ThuThapGiaThiTruong>()
                .HasOne(t => t.LoaiGia)
                .WithMany(l => l.ThuThapGiaThiTruongs)
                .HasForeignKey(t => t.LoaiGiaId)
                .OnDelete(DeleteBehavior.Restrict);
    
            // Cấu hình cho Dm_LoaiGia
            modelBuilder.Entity<Dm_LoaiGia>()
                .HasIndex(l => l.Ma)
                .IsUnique();

            // Chỉ mục cho ThuThapGiaThiTruong
            modelBuilder.Entity<ThuThapGiaThiTruong>()
                .HasIndex(t => new { t.NgayThuThap, t.HangHoaId, t.LoaiGiaId, t.LoaiNghiepVu })
                .IsUnique();

            modelBuilder.Entity<ThuThapGiaThiTruong>()
                .HasIndex(t => t.NgayThuThap);

            modelBuilder.Entity<ThuThapGiaThiTruong>()
                .HasIndex(t => t.HangHoaId);

            modelBuilder.Entity<ThuThapGiaThiTruong>()
                .HasIndex(t => t.LoaiGiaId);

            modelBuilder.Entity<ThuThapGiaThiTruong>()
                .HasIndex(t => t.LoaiNghiepVu);

            modelBuilder.Entity<ThuThapGiaThiTruong>()
                .HasIndex(t => t.IsDelete);

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
