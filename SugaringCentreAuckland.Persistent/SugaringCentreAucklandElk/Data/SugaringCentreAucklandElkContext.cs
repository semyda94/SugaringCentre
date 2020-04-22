
using System;
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

        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        public virtual DbSet<ProductImage> ProductImage { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<StaffImage> StaffImage { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServiceCategory> ServiceCategory { get; set; }
        public virtual DbSet<ServiceStaff> ServiceStaff { get; set; }
        public virtual DbSet<Leave> Leave { get; set; }

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

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.BookingId);

                entity.Property(e => e.BookingId).ValueGeneratedOnAdd();
                
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Message).IsUnicode(false);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.ServiceNavigation)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.ServiceId);

                entity.HasOne(d => d.StaffNavigation)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.StaffId);
            });
            
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryId);

                entity.Property(e => e.CategoryId).ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });
            
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId);

                entity.Property(e => e.OrderId).ValueGeneratedOnAdd();

                entity.Property(e => e.Client)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });
            
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);
                
                entity.Property(e => e.ProductId).ValueGeneratedOnAdd();

                entity.Property(e => e.Title)
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
            
            modelBuilder.Entity<ProductOrder>(entity =>
            {
                entity.HasKey(e => e.ProductOrderId);

                entity.Property(e => e.ProductOrderId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.OrderNavigation)
                    .WithMany(p => p.ProductOrders)
                    .HasForeignKey(d => d.OrderId);

                entity.HasOne(d => d.ProductNavigation)
                    .WithMany(p => p.ProductOrders)
                    .HasForeignKey(d => d.ProductId);
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
            
            modelBuilder.Entity<Leave>(entity =>
            {
                entity.HasKey(e => e.LeaveId);

                entity.Property(e => e.LeaveId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.StaffNavigation)
                    .WithMany(p => p.Leaves)
                    .HasForeignKey(d => d.StaffId);
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
                
                entity.Property(e => e.ServiceId).ValueGeneratedOnAdd();

                entity.Property(e => e.Desc).IsUnicode(false);

                entity.Property(e => e.Price);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.ServiceCategoryNavigation)
                    .WithMany(p => p.Services)
                    .HasForeignKey(d => d.ServiceCategoryId);
            });

            modelBuilder.Entity<ServiceCategory>(entity =>
            {
                entity.HasKey(e => e.ServiceCategoryId);

                entity.Property(e => e.ServiceCategoryId).ValueGeneratedOnAdd();

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

            });
            
            modelBuilder.Entity<ServiceStaff>(entity =>
            {
                entity.HasKey(e => e.ServiceStaffId);

                entity.Property(e => e.ServiceStaffId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.ServiceNavigation)
                    .WithMany(p => p.ServiceStaff)
                    .HasForeignKey(d => d.ServiceId);

                entity.HasOne(d => d.StaffNavigation)
                    .WithMany(p => p.ServiceStaff)
                    .HasForeignKey(d => d.StaffId);
            });
        }
    }
}
