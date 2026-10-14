using DishManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace DishManagement.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Dish_BIT240118> Dishes => Set<Dish_BIT240118>();
    public DbSet<DishCategory_BIT240118> DishCategories => Set<DishCategory_BIT240118>();
    public DbSet<DishImage_BIT240118> DishImages => Set<DishImage_BIT240118>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DishCategory_BIT240118>(entity =>
        {
            entity.ToTable("BIT240118_DishCategories");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(100).IsRequired();
            entity.Property(c => c.Description).HasMaxLength(500);
            entity.Property(c => c.Id).HasColumnName("BIT240118_Id");
            entity.Property(c => c.Name).HasColumnName("BIT240118_Name");
            entity.Property(c => c.Description).HasColumnName("BIT240118_Description");
            entity.HasIndex(c => c.Name).IsUnique();
        });

        modelBuilder.Entity<Dish_BIT240118>(entity =>
        {
            entity.ToTable("BIT240118_Dishes");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).HasMaxLength(150).IsRequired();
            entity.Property(d => d.Price).HasPrecision(18, 2);
            entity.Property(d => d.Description).HasMaxLength(1000);
            entity.Property(d => d.Id).HasColumnName("BIT240118_Id");
            entity.Property(d => d.Name).HasColumnName("BIT240118_Name");
            entity.Property(d => d.Price).HasColumnName("BIT240118_Price");
            entity.Property(d => d.PreparationTime).HasColumnName("BIT240118_PreparationTime");
            entity.Property(d => d.IsAvailable).HasColumnName("BIT240118_IsAvailable");
            entity.Property(d => d.Description).HasColumnName("BIT240118_Description");
            entity.Property(d => d.DishCategoryId).HasColumnName("BIT240118_DishCategoryId");
            entity.HasOne(d => d.DishCategory).WithMany(c => c.Dishes)
                .HasForeignKey(d => d.DishCategoryId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DishImage_BIT240118>(entity =>
        {
            entity.ToTable("BIT240118_DishImages");
            entity.HasKey(i => i.Id);
            entity.Property(i => i.ImageUrl).HasMaxLength(1000).IsRequired();
            entity.Property(i => i.Id).HasColumnName("BIT240118_Id");
            entity.Property(i => i.ImageUrl).HasColumnName("BIT240118_ImageUrl");
            entity.Property(i => i.IsThumbnail).HasColumnName("BIT240118_IsThumbnail");
            entity.Property(i => i.DishId).HasColumnName("BIT240118_DishId");
            entity.HasOne(i => i.Dish).WithMany(d => d.Images)
                .HasForeignKey(i => i.DishId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
