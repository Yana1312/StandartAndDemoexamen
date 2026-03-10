using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace AppShoes.Models;

public partial class ShoesContext : DbContext
{
    public ShoesContext()
    {
    }

    public ShoesContext(DbContextOptions<ShoesContext> options)
        : base(options)
    {
    }

    public static ShoesContext context { get; } = new();

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Manufactur> Manufacturs { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<PickPoint> PickPoints { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Tovar> Tovars { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=shoes;user=root;password=1234", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.43-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.ToTable("category");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryTitle)
                .HasMaxLength(45)
                .HasColumnName("category_title");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ItemArticul })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("item");

            entity.HasIndex(e => e.ItemArticul, "tovar_fk_idx");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ItemArticul)
                .HasMaxLength(7)
                .HasColumnName("item_articul");
            entity.Property(e => e.ItemCount).HasColumnName("item_count");

            entity.HasOne(d => d.ItemArticulNavigation).WithMany(p => p.Items)
                .HasForeignKey(d => d.ItemArticul)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tovar_fk");

            entity.HasOne(d => d.Order).WithMany(p => p.Items)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_fk");
        });

        modelBuilder.Entity<Manufactur>(entity =>
        {
            entity.HasKey(e => e.ManufacturId).HasName("PRIMARY");

            entity.ToTable("manufactur");

            entity.Property(e => e.ManufacturId).HasColumnName("manufactur_id");
            entity.Property(e => e.ManufacturName)
                .HasMaxLength(45)
                .HasColumnName("manufactur_name");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PRIMARY");

            entity.ToTable("order");

            entity.HasIndex(e => e.OrderAdress, "pick_point_fk_idx");

            entity.HasIndex(e => e.OrderUser, "user_fk_idx");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.OrderAdress).HasColumnName("order_adress");
            entity.Property(e => e.OrderCode).HasColumnName("order_code");
            entity.Property(e => e.OrderDate).HasColumnName("order_date");
            entity.Property(e => e.OrderDeliveryDate).HasColumnName("order_delivery_date");
            entity.Property(e => e.OrderItem).HasColumnName("order_item");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(15)
                .HasColumnName("order_status");
            entity.Property(e => e.OrderUser).HasColumnName("order_user");

            entity.HasOne(d => d.OrderAdressNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderAdress)
                .HasConstraintName("pick_point_fk");

            entity.HasOne(d => d.OrderUserNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderUser)
                .HasConstraintName("user_fk");
        });

        modelBuilder.Entity<PickPoint>(entity =>
        {
            entity.HasKey(e => e.PickPointId).HasName("PRIMARY");

            entity.ToTable("pick_point");

            entity.Property(e => e.PickPointId).HasColumnName("pick_point_id");
            entity.Property(e => e.PickPointCity)
                .HasMaxLength(65)
                .HasColumnName("pick_point_city");
            entity.Property(e => e.PickPointHouse).HasColumnName("pick_point_house");
            entity.Property(e => e.PickPointIndex)
                .HasMaxLength(6)
                .HasColumnName("pick_point_index");
            entity.Property(e => e.PickPointStreet)
                .HasMaxLength(85)
                .HasColumnName("pick_point_street");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PRIMARY");

            entity.ToTable("supplier");

            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.SupplierTitle)
                .HasMaxLength(15)
                .HasColumnName("supplier_title");
        });

        modelBuilder.Entity<Tovar>(entity =>
        {
            entity.HasKey(e => e.TovarArticul).HasName("PRIMARY");

            entity.ToTable("tovar");

            entity.HasIndex(e => e.TovarArticul, "Item_fk_idx");

            entity.HasIndex(e => e.TovarCategory, "category_fk_idx");

            entity.HasIndex(e => e.TovarManufactor, "manufactur_fk_idx");

            entity.HasIndex(e => e.TovarSupplier, "supplier_fk_idx");

            entity.Property(e => e.TovarArticul)
                .HasMaxLength(45)
                .HasColumnName("tovar_articul");
            entity.Property(e => e.TovarCategory).HasColumnName("tovar_category");
            entity.Property(e => e.TovarCost)
                .HasPrecision(8, 2)
                .HasColumnName("tovar_cost");
            entity.Property(e => e.TovarCount).HasColumnName("tovar_count");
            entity.Property(e => e.TovarDescription)
                .HasMaxLength(225)
                .HasColumnName("tovar_description");
            entity.Property(e => e.TovarManufactor).HasColumnName("tovar_manufactor");
            entity.Property(e => e.TovarPhoto)
                .HasMaxLength(8)
                .HasColumnName("tovar_photo");
            entity.Property(e => e.TovarSale).HasColumnName("tovar_sale");
            entity.Property(e => e.TovarSupplier).HasColumnName("tovar_supplier");
            entity.Property(e => e.TovarTitle)
                .HasMaxLength(45)
                .HasColumnName("tovar_title");
            entity.Property(e => e.TovarUnit)
                .HasMaxLength(5)
                .HasColumnName("tovar_unit");

            entity.HasOne(d => d.TovarCategoryNavigation).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.TovarCategory)
                .HasConstraintName("category_fk");

            entity.HasOne(d => d.TovarManufactorNavigation).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.TovarManufactor)
                .HasConstraintName("manufactur_fk");

            entity.HasOne(d => d.TovarSupplierNavigation).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.TovarSupplier)
                .HasConstraintName("supplier_fk");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("user");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserFirstname)
                .HasMaxLength(45)
                .HasColumnName("user_firstname");
            entity.Property(e => e.UserLastname)
                .HasMaxLength(45)
                .HasColumnName("user_lastname");
            entity.Property(e => e.UserLogin)
                .HasMaxLength(225)
                .HasColumnName("user_login");
            entity.Property(e => e.UserName)
                .HasMaxLength(45)
                .HasColumnName("user_name");
            entity.Property(e => e.UserPassword)
                .HasMaxLength(255)
                .HasColumnName("user_password");
            entity.Property(e => e.UserRole)
                .HasMaxLength(65)
                .HasColumnName("user_role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
