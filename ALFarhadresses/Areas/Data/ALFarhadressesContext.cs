using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ALFarhadresses.Models
{
    public partial class ALFarhadressesContext : DbContext
    {
        public ALFarhadressesContext()
        {
        }

        public ALFarhadressesContext(DbContextOptions<ALFarhadressesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<Orderitem> Orderitem { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<Rating> Rating { get; set; }
        public virtual DbSet<Contactus> Contactus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categories>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Customers>(entity =>
            {
                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Email).IsRequired();

                entity.Property(e => e.Fname)
                    .IsRequired()
                    .HasColumnName("FName")
                    .HasMaxLength(50);

                entity.Property(e => e.Gender)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Lname)
                    .IsRequired()
                    .HasColumnName("LName")
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Orderitem>(entity =>
            {
                entity.Property(e => e.OrderId).HasColumnName("orderId");

                entity.HasOne(d => d.CatNavigation)
                    .WithMany(p => p.Orderitem)
                    .HasForeignKey(d => d.Cat)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orderitem_ToCategories");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Orderitem)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orderitem_ToOrders");

                entity.HasOne(d => d.ProNavigation)
                    .WithMany(p => p.Orderitem)
                    .HasForeignKey(d => d.Pro)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orderitem_ToProducts");
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Payment).HasMaxLength(50);

                entity.Property(e => e.ShipAddress).IsRequired();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50);
                

                entity.HasOne(d => d.CustNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.Cust)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orders_ToCustomers");
            });

            modelBuilder.Entity<Products>(entity =>
            {
                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.Ephoto).HasColumnName("EPhoto");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.CatNavigation)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.Cat)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Products_ToCategories");
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("date");

                entity.HasOne(d => d.CatNavigation)
                    .WithMany(p => p.Rating)
                    .HasForeignKey(d => d.Cat)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Rating_ToCategories");

                entity.HasOne(d => d.CustNavigation)
                    .WithMany(p => p.Rating)
                    .HasForeignKey(d => d.Cust)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Rating_ToCustomers");

                entity.HasOne(d => d.ProNavigation)
                    .WithMany(p => p.Rating)
                    .HasForeignKey(d => d.Pro)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Rating_ToProducts");
            });
        }
    }
}
