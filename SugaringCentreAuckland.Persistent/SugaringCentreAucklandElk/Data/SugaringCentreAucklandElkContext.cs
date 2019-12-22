
using Microsoft.EntityFrameworkCore;
using SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Models;

namespace SugaringCentreAuckland.Persistent.SugaringCentreAucklandElk.Data
{
    public partial class SugaringCentreAucklandElkContext : DbContext
    {
        public SugaringCentreAucklandElkContext()
        {
        }

        public SugaringCentreAucklandElkContext(DbContextOptions<SugaringCentreAucklandElkContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        public virtual DbSet<Subscription> Subscription { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryId).ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);
                
                entity.Property(e => e.ProductId).ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Desc)
                    .IsUnicode(false);

                entity.Property(e => e.Price);
            });
            
            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(e => e.ProductCategoryId);
                
                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.ProductCategory)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.ProductNavigation)
                    .WithMany(p => p.ProductCategory)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.Property(e => e.SubscriptionId).ValueGeneratedOnAdd();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);
            });
        }
    }
}
