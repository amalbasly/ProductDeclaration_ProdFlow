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
        public DbSet<FlanDecoupe> FlanDecoupes { get; set; }
        public DbSet<FlanPartie> FlanParties { get; set; }

        // Assemblage-related
        public DbSet<Assemblage> Assemblages { get; set; }
        public DbSet<AssemblageProduit> AssemblageProduits { get; set; }

        // For stored procedure results
        public DbSet<StoredProcedureResult> StoredProcedureResults { get; set; }
        public DbSet<VerificationToken> VerificationTokens { get; set; }

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

            // Assemblage config
            modelBuilder.Entity<Assemblage>(entity =>
            {
                entity.HasKey(a => a.AssemblageId);

                entity.Property(a => a.NomAssemblage)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(a => a.MainProduitPtNum)
                    .IsRequired()
                    .HasMaxLength(18);

                entity.Property(a => a.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(a => a.MainProduit)
                    .WithMany()
                    .HasForeignKey(a => a.MainProduitPtNum)
                    .HasPrincipalKey(p => p.PtNum)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Gallia)
                    .WithMany()
                    .HasForeignKey(a => a.GalliaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(a => a.SecondaryProduits)
                    .WithOne(ap => ap.Assemblage)
                    .HasForeignKey(ap => ap.AssemblageId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // AssemblageProduit config
            modelBuilder.Entity<AssemblageProduit>(entity =>
            {
                entity.HasKey(ap => ap.AssemblageProduitId);

                entity.Property(ap => ap.ProduitPtNum)
                    .IsRequired()
                    .HasMaxLength(18);

                entity.HasOne(ap => ap.Produit)
                    .WithMany()
                    .HasForeignKey(ap => ap.ProduitPtNum)
                    .HasPrincipalKey(p => p.PtNum)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ap => ap.Assemblage)
                    .WithMany(a => a.SecondaryProduits)
                    .HasForeignKey(ap => ap.AssemblageId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ClientReferenceData config
            modelBuilder.Entity<ClientReferenceData>(entity =>
            {
                entity.HasKey(e => new { e.ClientReference, e.PtNum });

                entity.Property(e => e.ClientReference)
                    .HasMaxLength(255);

                entity.Property(e => e.ClientIndex)
                    .HasMaxLength(255);

                entity.Property(e => e.Client)
                    .HasMaxLength(255);

                entity.Property(e => e.Referentiel)
                    .HasMaxLength(255);

                entity.Property(e => e.PtNum)
                    .HasColumnName("pt_num")
                    .HasMaxLength(18);
            });

            // GalliaImage config
            modelBuilder.Entity<GalliaImage>(entity =>
            {
                entity.HasKey(gi => gi.GalliaImageId);

                entity.Property(gi => gi.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(gi => gi.UpdatedAt)
                    .IsRequired(false);

                entity.HasOne(gi => gi.Gallia)
                    .WithMany(g => g.Images)
                    .HasForeignKey(gi => gi.GalliaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // VerificationToken config
            modelBuilder.Entity<VerificationToken>(entity =>
            {
                entity.HasKey(vt => vt.Id);

                entity.Property(vt => vt.PtNum)
                    .HasColumnName("pt_num")
                    .HasMaxLength(18);

                entity.Property(vt => vt.Token)
                    .HasMaxLength(100); // Matches StringLength in entity

                entity.Property(vt => vt.TraceabilityManagerId)
                    .HasMaxLength(50);

                entity.Property(vt => vt.ExpiryDate)
                    .IsRequired();

                entity.Property(vt => vt.CreatedDate)
                    .HasColumnName("CreatedDate")
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne<Produit>()
                    .WithMany()
                    .HasForeignKey(vt => vt.PtNum)
                    .HasPrincipalKey(p => p.PtNum)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // FlanDecoupe config
            modelBuilder.Entity<FlanDecoupe>()
                .HasMany(f => f.Parts)
                .WithOne(p => p.FlanDecoupe)
                .HasForeignKey(p => p.FlanDecoupeId)
                .OnDelete(DeleteBehavior.Cascade);

            // FlanPartie config
            modelBuilder.Entity<FlanPartie>()
                .HasKey(p => p.CodePartie);

            modelBuilder.Entity<FlanPartie>()
                .HasOne(p => p.Produit)
                .WithMany()
                .HasForeignKey(p => p.pt_numOriginal);
        }
    }
}