
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

        public virtual DbSet<ShopCategory> ShopCategory { get; set; }
        public virtual DbSet<ShopItem> ShopItem { get; set; }
        public virtual DbSet<Subscription> Subscription { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<ShopCategory>(entity =>
            {
                entity.Property(e => e.ShopCategoryId).ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasMany(t => t.ShopItems)
                    .WithOne(i => i.NavigationShopCategoryId);
            });

            modelBuilder.Entity<ShopItem>(entity =>
            {
                entity.Property(e => e.ShopItemId).ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Desc)
                    .IsUnicode(false);

                entity.Property(e => e.Price);

                entity.HasOne(i => i.NavigationShopCategoryId)
                    .WithMany(t => t.ShopItems)
                    .HasForeignKey(i => i.CategoryId);
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);
            });
        }
    }
}
