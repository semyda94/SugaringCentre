
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
        public virtual DbSet<ProductImage> ProductImage { get; set; }
        public virtual DbSet<Subscription> Subscription { get; set; }
        
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<StaffImage> StaffImage { get; set; }
        
        public virtual DbSet<Service> Service { get; set; }
        public virtual DbSet<ServiceImage> ServiceImage { get; set; }
        public virtual DbSet<ServiceType> ServiceType { get; set; }
        public virtual DbSet<ServiceTypeStaff> ServiceTypeStaff { get; set; }

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
            
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.ProductImageId);

                /*entity.HasOne(d => d.ProductNavigation)
                    .WithMany(p => p.ProductImage)
                    .HasForeignKey(d => d.ProductId);*/
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.Property(e => e.SubscriptionId).ValueGeneratedOnAdd();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);
            });
            
            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasKey(e => e.StaffId);
                
                entity.Property(e => e.StaffId).ValueGeneratedOnAdd();

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });
            
            modelBuilder.Entity<StaffImage>(entity =>
            {
                entity.HasKey(e => e.StaffImageId);

                entity.HasOne(d => d.StaffNavigation)
                    .WithMany(p => p.StaffImage)
                    .HasForeignKey(d => d.StaffId);
            });
            
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.ServiceId);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ServiceImage>(entity =>
            {
                entity.HasKey(e => e.ServiceImageId);

                entity.Property(e => e.ServiceImageId);

                entity.HasOne(d => d.ServiceNavigation)
                    .WithMany(p => p.ServiceImage)
                    .HasForeignKey(d => d.Service);
            });

            modelBuilder.Entity<ServiceType>(entity =>
            {
                entity.HasKey(e => e.ServiceTypeId);
                
                entity.Property(e => e.Description)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });
            
            modelBuilder.Entity<ServiceTypeStaff>(entity =>
            {
                entity.HasKey(e => e.ServiceTypeStaffId);

                entity.Property(e => e.ServiceTypeStaffId);

                entity.HasOne(d => d.ServiceTypeNavigation)
                    .WithMany(p => p.ServiceTypeStaff)
                    .HasForeignKey(d => d.ServiceType)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.StaffNavigation)
                    .WithMany(p => p.ServiceTypeStaff)
                    .HasForeignKey(d => d.Staff)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }
    }
}
