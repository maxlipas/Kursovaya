using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace KursMVVM.Models;

public partial class KursContext : DbContext
{
    public KursContext()
    {
        Database.EnsureCreated();
    }

    public KursContext(DbContextOptions<KursContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public virtual DbSet<Workshop> Workshops { get; set; }

    public virtual DbSet<SpecialClothing> SpecialClothings { get; set; }

    public virtual DbSet<Worker> Workers { get; set; }

    public virtual DbSet<Receipt> Receipts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=kurs_new.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Workshop>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Workshop");
            entity.Property(e => e.Id).HasColumnName("id_workshop");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Manager).HasColumnName("manager").IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<SpecialClothing>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("SpecialClothing", t =>
            {
                t.HasCheckConstraint("CK_SpecialClothing_WearPeriod", "wear_period > 0");
                t.HasCheckConstraint("CK_SpecialClothing_UnitCost", "unit_cost > 0");
            });
            entity.Property(e => e.Id).HasColumnName("id_clothing");
            entity.Property(e => e.Type).HasColumnName("type").IsRequired().HasMaxLength(100);
            entity.Property(e => e.WearPeriod).HasColumnName("wear_period");
            entity.Property(e => e.UnitCost).HasColumnName("unit_cost");
        });

        modelBuilder.Entity<Worker>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Worker", t =>
            {
                t.HasCheckConstraint("CK_Worker_Discount", "discount >= 0 AND discount <= 100");
            });
            entity.Property(e => e.Id).HasColumnName("id_worker");
            entity.Property(e => e.FIO).HasColumnName("fio").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Position).HasColumnName("position").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Discount).HasColumnName("discount");
            entity.Property(e => e.WorkshopId).HasColumnName("id_workshop");

            entity.HasOne(d => d.Workshop).WithMany(p => p.Workers)
                .HasForeignKey(d => d.WorkshopId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Receipt>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Receipt");
            entity.Property(e => e.Id).HasColumnName("id_receipt");
            entity.Property(e => e.WorkerId).HasColumnName("id_worker");
            entity.Property(e => e.ClothingId).HasColumnName("id_clothing");
            entity.Property(e => e.DateReceived).HasColumnName("date_received");
            entity.Property(e => e.Signature).HasColumnName("signature").HasMaxLength(100);

            entity.HasOne(d => d.Worker).WithMany(p => p.Receipts)
                .HasForeignKey(d => d.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Clothing).WithMany(p => p.Receipts)
                .HasForeignKey(d => d.ClothingId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
