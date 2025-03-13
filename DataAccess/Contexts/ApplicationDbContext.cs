using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BusinessObject.Models; // Đảm bảo import namespace này

namespace DataAccess.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<AspNetUsers, AspNetRoles, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public ApplicationDbContext() { }

        public DbSet<AspNetUsers> AspNetUsers { get; set; }
        public DbSet<AspNetRoles> AspNetRoles { get; set; }
        public DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-29BOTTN;Database=assignment03;User Id=sa;Password=lienminh;TrustServerCertificate=True");
            }
        }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình Identity tables
            modelBuilder.Entity<AspNetUserRoles>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.AspNetUserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Giữ CASCADE vì User bị xóa thì UserRoles cũng nên xóa

            modelBuilder.Entity<AspNetUserRoles>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.AspNetUserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict); // Đổi thành RESTRICT để tránh xung đột

            modelBuilder.Entity<AspNetRoleClaims>()
                .HasOne(rc => rc.Role)
                .WithMany(r => r.AspNetRoleClaims)
                .HasForeignKey(rc => rc.RoleId)
                .OnDelete(DeleteBehavior.Restrict); // Đổi thành RESTRICT để tránh xung đột

            modelBuilder.Entity<AspNetUserClaims>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.AspNetUserClaims)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Giữ CASCADE

            modelBuilder.Entity<AspNetUserLogins>()
                .HasOne(ul => ul.User)
                .WithMany(u => u.AspNetUserLogins)
                .HasForeignKey(ul => ul.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Giữ CASCADE

            modelBuilder.Entity<AspNetUserTokens>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.AspNetUserTokens)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Giữ CASCADE

            // Cấu hình các bảng khác
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Member)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderId, od.ProductId });

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}