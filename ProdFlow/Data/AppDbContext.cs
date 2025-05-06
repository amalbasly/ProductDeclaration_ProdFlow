using Microsoft.EntityFrameworkCore;
using ProdFlow.Models.Entities;
using ProdFlow.Models.Responses;

namespace ProdFlow.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Employee-related
        public DbSet<PersonnelTraca> Employees { get; set; }
        public DbSet<Groupe> Groupes { get; set; }

        // Product-related
        public DbSet<Produit> Produits { get; set; }
        public DbSet<Mode> Modes { get; set; }
        public DbSet<SynoptiqueProd> SynoptiqueProd { get; set; }
        public DbSet<Justification> Justifications { get; set; }

        // Gallia-related
        public DbSet<Gallia> Gallias { get; set; }
        // public DbSet<ProductGallia> ProductGallias { get; set; }

        // Shared
        public DbSet<StoredProcedureResult> StoredProcedureResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Employee config
            modelBuilder.Entity<StoredProcedureResult>().HasNoKey().ToView(null);

            // Product config
            modelBuilder.Entity<Produit>(entity =>
            {
                entity.HasKey(e => e.PtNum);
                entity.Property(e => e.PtNum).HasColumnName("pt_num");
            });

            // Synoptique config
            modelBuilder.Entity<SynoptiqueProd>()
                .HasKey(s => new { s.ID, s.PtNum });

            modelBuilder.Entity<SynoptiqueProd>()
                .HasOne<Mode>()
                .WithMany()
                .HasForeignKey(s => s.ID);

            modelBuilder.Entity<SynoptiqueProd>()
                .HasOne<Produit>()
                .WithMany()
                .HasForeignKey(s => s.PtNum);

            // Justification config
            modelBuilder.Entity<Justification>(entity =>
            {
                entity.HasKey(j => j.JustificationID);

                entity.HasOne<Produit>()
                    .WithMany()
                    .HasForeignKey(j => j.ProductCode)
                    .HasPrincipalKey(p => p.PtNum)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(j => j.ProductCode)
                    .HasColumnName("ProductCode")
                    .HasMaxLength(18);

                entity.Property(j => j.UrgencyLevel)
                    .HasMaxLength(20);

                entity.Property(j => j.Status)
                    .HasDefaultValue("Pending")
                    .HasMaxLength(20);
            });

            // Gallia config
            modelBuilder.Entity<Gallia>(entity =>
            {
                entity.HasKey(g => g.GalliaId);

                entity.Property(g => g.PLIB1)
                    .HasMaxLength(50);

                entity.Property(g => g.QLIB3)
                    .HasMaxLength(50);

                entity.Property(g => g.LIB1)
                    .HasMaxLength(100);

                // Repeat for LIB2-LIB7
                entity.Property(g => g.LIB2).HasMaxLength(100);
                entity.Property(g => g.LIB3).HasMaxLength(100);
                entity.Property(g => g.LIB4).HasMaxLength(100);
                entity.Property(g => g.LIB5).HasMaxLength(100);
                entity.Property(g => g.LIB6).HasMaxLength(100);
                entity.Property(g => g.LIB7).HasMaxLength(100);

                entity.Property(g => g.SupplierName)
                    .HasMaxLength(100);

                entity.Property(g => g.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ProductGallia config (commented out)
            /*
            modelBuilder.Entity<ProductGallia>(entity =>
            {
                entity.HasKey(pg => pg.ProductGalliaId);

                // Relationship with Gallia
                entity.HasOne<Gallia>()
                    .WithMany()
                    .HasForeignKey(pg => pg.GalliaId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relationship with Produit
                entity.HasOne<Produit>()
                    .WithMany()
                    .HasForeignKey(pg => pg.Pt_Num)
                    .HasPrincipalKey(p => p.PtNum)
                    .OnDelete(DeleteBehavior.Restrict);

                // Column configurations
                entity.Property(pg => pg.Pt_Num)
                    .HasMaxLength(18);

                entity.Property(pg => pg.ProductName)
                    .HasMaxLength(100);

                entity.Property(pg => pg.SupplierReference)
                    .HasMaxLength(100);

                entity.Property(pg => pg.LabelNumber)
                    .HasMaxLength(100);

                entity.Property(pg => pg.Description)
                    .HasMaxLength(255);

                entity.Property(pg => pg.SupplierName)
                    .HasMaxLength(100);

                entity.Property(pg => pg.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });
            */
        }
    }
}
