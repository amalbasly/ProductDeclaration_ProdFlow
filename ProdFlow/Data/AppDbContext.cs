using Microsoft.EntityFrameworkCore;
using ProdFlow.DTOs;
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

        // For stored procedure results
        public DbSet<StoredProcedureResult> StoredProcedureResults { get; set; }
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Employee config
            modelBuilder.Entity<StoredProcedureResult>().HasNoKey().ToView(null);

            // Product config
            modelBuilder.Entity<Produit>(entity =>
            {
                entity.HasKey(e => e.PtNum);
                entity.Property(e => e.PtNum)
                    .HasColumnName("pt_num")
                    .HasMaxLength(18);
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
                entity.Property(g => g.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            

            // Stored procedure DTO config
            
            modelBuilder.Entity<ClientReferenceData>(entity =>
            {
                entity.HasKey(e => new { e.ClientReference, e.PtNum }); // Composite key if needed

                entity.Property(e => e.ClientReference)
                    .HasMaxLength(255);

                entity.Property(e => e.ClientIndex)
                    .HasMaxLength(255);

                entity.Property(e => e.Client)
                    .HasMaxLength(255);

                entity.Property(e => e.Referentiel)
                    .HasMaxLength(255);

                entity.Property(e => e.PtNum)
                    .HasMaxLength(18)
                    .HasColumnName("pt_num");
            });
            modelBuilder.Entity<GalliaImage>(entity =>
            {
                entity.HasKey(gi => gi.GalliaImageId);

                entity.Property(gi => gi.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(gi => gi.UpdatedAt)
                      .IsRequired(false);  // Make UpdatedAt optional

                entity.HasOne(gi => gi.Gallia)
                      .WithMany(g => g.Images)
                      .HasForeignKey(gi => gi.GalliaId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
