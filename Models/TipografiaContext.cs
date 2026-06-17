using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using Tipografia3.Models;

namespace Tipografia3;

public partial class TipografiaContext : DbContext
{
    public TipografiaContext()
    {
        Database.EnsureCreated();
    }

    public TipografiaContext(DbContextOptions<TipografiaContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Dogovor> Dogovors { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<TipografCeh> TipografCehs { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=Tipografia.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.IdClient);

            entity.ToTable("Client");

            entity.Property(e => e.IdClient)
                .ValueGeneratedNever()
                .HasColumnName("Id_Client");
        });

        modelBuilder.Entity<Dogovor>(entity =>
        {
            entity.HasKey(e => e.IdDogovor);

            entity.ToTable("Dogovor");

            entity.Property(e => e.IdDogovor)
                .ValueGeneratedNever()
                .HasColumnName("Id_Dogovor");
            entity.Property(e => e.DateDue).HasColumnName("Date_Due");
            entity.Property(e => e.DateOforml).HasColumnName("Date_Oforml");
            entity.Property(e => e.IdClient).HasColumnName("Id_Client");
            entity.Property(e => e.NumberDogovor).HasColumnType("INT");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.IdOrder);

            entity.ToTable("Order");

            entity.Property(e => e.IdOrder)
                .ValueGeneratedNever()
                .HasColumnName("Id_Order");
            entity.Property(e => e.IdProduct).HasColumnName("Id_Product");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct);

            entity.ToTable("Product");

            entity.Property(e => e.IdProduct)
                .ValueGeneratedNever()
                .HasColumnName("Id_Product");
            entity.Property(e => e.NumberCeh).HasColumnType("INT");
            entity.Property(e => e.Price1sh).HasColumnName("Price_1sh");
        });

        modelBuilder.Entity<TipografCeh>(entity =>
        {
            entity.HasKey(e => e.IdTipograf);

            entity.ToTable("Tipograf_ceh");

            entity.Property(e => e.IdTipograf)
                .ValueGeneratedNever()
                .HasColumnName("Id_Tipograf");
            entity.Property(e => e.NumberCeh).HasColumnType("INT");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
