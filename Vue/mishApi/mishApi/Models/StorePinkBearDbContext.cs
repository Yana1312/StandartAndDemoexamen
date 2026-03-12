using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace mishApi.Models;

public partial class StorePinkBearDbContext : DbContext
{
    public StorePinkBearDbContext()
    {
    }

    public StorePinkBearDbContext(DbContextOptions<StorePinkBearDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Basket> Baskets { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Manufactur> Manufacturs { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Point> Points { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Tovar> Tovars { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;user=root;password=1234;database=store_pink_bear_db", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.43-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Basket>(entity =>
        {
            entity.HasKey(e => new { e.TovarId, e.UserId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("basket");

            entity.HasIndex(e => e.UserId, "fk_user_idx");

            entity.Property(e => e.TovarId)
                .ValueGeneratedOnAdd()
                .HasColumnName("tovar_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.BasketCount).HasColumnName("basket_count");

            entity.HasOne(d => d.Tovar).WithMany(p => p.Baskets)
                .HasForeignKey(d => d.TovarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tovar");

            entity.HasOne(d => d.User).WithMany(p => p.Baskets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user");
        });

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
            entity.HasKey(e => new { e.OrderId, e.ItemId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("item");

            entity.HasIndex(e => e.ItemId, "tovar_fk_idx");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ItemCount).HasColumnName("item_count");

            entity.HasOne(d => d.ItemNavigation).WithMany(p => p.Items)
                .HasForeignKey(d => d.ItemId)
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
            entity.Property(e => e.ManufacturTitle)
                .HasMaxLength(45)
                .HasColumnName("manufactur_title");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.OrderPoint, "order_point_fk_idx");

            entity.HasIndex(e => e.OrderUser, "order_user_fk_idx");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.OrderCode).HasColumnName("order_code");
            entity.Property(e => e.OrderDate).HasColumnName("order_date");
            entity.Property(e => e.OrderDateDelivery).HasColumnName("order_date_delivery");
            entity.Property(e => e.OrderItem).HasColumnName("order_item");
            entity.Property(e => e.OrderPoint).HasColumnName("order_point");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(15)
                .HasColumnName("order_status");
            entity.Property(e => e.OrderUser).HasColumnName("order_user");

            entity.HasOne(d => d.OrderPointNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderPoint)
                .HasConstraintName("order_point_fk");

            entity.HasOne(d => d.OrderUserNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderUser)
                .HasConstraintName("order_user_fk");
        });

        modelBuilder.Entity<Point>(entity =>
        {
            entity.HasKey(e => e.PointId).HasName("PRIMARY");

            entity.ToTable("point");

            entity.Property(e => e.PointId).HasColumnName("point_id");
            entity.Property(e => e.PointCity)
                .HasMaxLength(65)
                .HasColumnName("point_city");
            entity.Property(e => e.PointHouse)
                .HasMaxLength(5)
                .HasColumnName("point_house");
            entity.Property(e => e.PointIndex)
                .HasMaxLength(6)
                .HasColumnName("point_index");
            entity.Property(e => e.PointStreet)
                .HasMaxLength(65)
                .HasColumnName("point_street");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PRIMARY");

            entity.ToTable("supplier");

            entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
            entity.Property(e => e.SupplierTitle)
                .HasMaxLength(45)
                .HasColumnName("supplier_title");
        });

        modelBuilder.Entity<Tovar>(entity =>
        {
            entity.HasKey(e => e.TovarId).HasName("PRIMARY");

            entity.ToTable("tovar");

            entity.HasIndex(e => e.TovarCategory, "tovar_category_fk_idx");

            entity.HasIndex(e => e.TovarManufactur, "tovar_manufactu_fkr_idx");

            entity.HasIndex(e => e.TovarSupplier, "tovar_supplier_fk_idx");

            entity.Property(e => e.TovarId).HasColumnName("tovar_id");
            entity.Property(e => e.TovarCategory).HasColumnName("tovar_category");
            entity.Property(e => e.TovarCost)
                .HasPrecision(8, 2)
                .HasColumnName("tovar_cost");
            entity.Property(e => e.TovarCount).HasColumnName("tovar_count");
            entity.Property(e => e.TovarDescription)
                .HasMaxLength(65)
                .HasColumnName("tovar_description");
            entity.Property(e => e.TovarManufactur).HasColumnName("tovar_manufactur");
            entity.Property(e => e.TovarPhoto)
                .HasMaxLength(255)
                .HasColumnName("tovar_photo");
            entity.Property(e => e.TovarSale).HasColumnName("tovar_sale");
            entity.Property(e => e.TovarSupplier).HasColumnName("tovar_supplier");
            entity.Property(e => e.TovarTitle)
                .HasMaxLength(45)
                .HasColumnName("tovar_title");

            entity.HasOne(d => d.TovarCategoryNavigation).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.TovarCategory)
                .HasConstraintName("tovar_category_fk");

            entity.HasOne(d => d.TovarManufacturNavigation).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.TovarManufactur)
                .HasConstraintName("tovar_manufactu_fkr");

            entity.HasOne(d => d.TovarSupplierNavigation).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.TovarSupplier)
                .HasConstraintName("tovar_supplier_fk");
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
                .HasMaxLength(85)
                .HasColumnName("user_login");
            entity.Property(e => e.UserName)
                .HasMaxLength(45)
                .HasColumnName("user_name");
            entity.Property(e => e.UserPassword)
                .HasMaxLength(85)
                .HasColumnName("user_password");
            entity.Property(e => e.UserRole)
                .HasColumnType("enum('администратор','менеджер','авторизованный клиент')")
                .HasColumnName("user_role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
