using Core.Entities.Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }

        public DbSet<NhomHangHoa> NhomHangHoas { get; set; }
        public DbSet<HangHoa> HangHoas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NhomHangHoa>()
            .HasOne(n => n.NhomCha)
            .WithMany(n => n.NhomCon)
            .HasForeignKey(n => n.NhomChaId)
            .OnDelete(DeleteBehavior.Restrict); // ✅ PostgreSQL sẽ sinh "ON DELETE RESTRICT"

            modelBuilder.Entity<HangHoa>()
                .HasOne(h => h.NhomHangHoa)
                .WithMany(n => n.HangHoas)
                .HasForeignKey(h => h.NhomHangHoaId)
                .OnDelete(DeleteBehavior.Restrict); // ✅ Ngăn xóa dây chuyền
        }
    }
}
