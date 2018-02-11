using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PalestreGoGo.DataModel;

namespace PalestreGoGo.DataAccess
{
    public partial class PalestreGoGoDbContext : DbContext
    {
        public PalestreGoGoDbContext() : base()
        {

        }

        public PalestreGoGoDbContext(DbContextOptions<PalestreGoGoDbContext> options) : base(options)
        {

        }

        public virtual DbSet<AbbonamentiUtenti> AbbonamentiUtenti { get; set; }
        public virtual DbSet<Appuntamenti> Appuntamenti { get; set; }
        public virtual DbSet<Clienti> Clienti { get; set; }
        public virtual DbSet<ClientiImmagini> ClientiImmagini { get; set; }
        public virtual DbSet<ClientiMetadati> ClientiMetadati { get; set; }
        public virtual DbSet<ClientiUtenti> ClientiUtenti { get; set; }
        public virtual DbSet<Locations> Locations { get; set; }
        public virtual DbSet<Schedules> Schedules { get; set; }
        public virtual DbSet<TipologieAbbonamenti> TipologieAbbonamenti { get; set; }
        public virtual DbSet<TipologiaCliente> TipologieClienti { get; set; }
        public virtual DbSet<TipologieImmagini> TipologieImmagini { get; set; }
        public virtual DbSet<TipologieLezioni> TipologieLezioni { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AbbonamentiUtenti>(entity =>
            {
                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.AbbonamentiUtenti)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AbbonamentiUtenti_Clienti");

                entity.HasOne(d => d.IdTipoAbbonamentoNavigation)
                    .WithMany(p => p.AbbonamentiUtenti)
                    .HasForeignKey(d => d.IdTipoAbbonamento)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AbbonamentiUtenti_TipoAbbonamento");
            });

            modelBuilder.Entity<Appuntamenti>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.ScheduleId })
                    .HasName("IDX_Appuntamenti_UserSched")
                    .IsUnique()
                    .HasFilter("([DataCancellazione] IS NULL)");

                entity.HasOne(d => d.Schedule)
                    .WithMany(p => p.Appuntamenti)
                    .HasForeignKey(d => d.ScheduleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Appuntamenti_Schedules");

                entity.HasOne(d => d.Cliente)
                                 .WithMany(p => p.Appuntamenti)
                                 .HasForeignKey(d => d.IdCliente)
                                 .OnDelete(DeleteBehavior.ClientSetNull)
                                 .HasConstraintName("FK_Appuntamenti_Clienti");
            });

            modelBuilder.Entity<Clienti>(entity =>
            {
                entity.Property(e => e.DataCreazione).HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.NumTelefono).IsUnicode(false);

                entity.Property(e => e.SecurityToken).HasDefaultValueSql("(CONVERT([nvarchar](100),newid()))");

                entity.HasIndex(e => new { e.Email })
                    .HasName("[UQ_Clienti_Email]")
                    .IsUnique();

                entity.HasIndex(e => new { e.UrlRoute })
                    .HasName("[UQ_Clienti_UrlRoute]")
                    .IsUnique();

                entity.HasIndex(e => new { e.SecurityToken })
                    .HasName("[UQ_Clienti_SecurityToken]")
                    .IsUnique();

                entity.HasOne(d => d.IdTipologiaNavigation)
                    .WithMany(p => p.Clienti)
                    .HasForeignKey(d => d.IdTipologia)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Clienti_Tipologia");
            });

            modelBuilder.Entity<ClientiImmagini>(entity =>
            {
                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.ClientiImmagini)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ClientiImmagini_Clienti");

                entity.HasOne(d => d.IdTipoImmagineNavigation)
                    .WithMany(p => p.ClientiImmagini)
                    .HasForeignKey(d => d.IdTipoImmagine)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ClientiImmagini_TipologiaImmagini");
            });
            modelBuilder.Entity<ClientiUtenti>(entity =>
            {
                entity.Property(e => e.DataCreazione).HasDefaultValueSql("(sysdatetime())");

                entity.HasKey(e => new { e.IdCliente, e.IdUtente });

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.ClientiUtenti)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ClientiUtenti_Clienti");
            });
            modelBuilder.Entity<ClientiMetadati>(entity =>
            {
                entity.HasKey(e => new { e.IdCliente, e.Key });

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.ClientiMetadati)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ClientiTags_Clienti");
            });

            modelBuilder.Entity<Locations>(entity =>
            {
                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.Locations)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Locations_Clienti");
            });

            modelBuilder.Entity<Schedules>(entity =>
            {
                //entity.Property(e => e.Timestamp).IsRowVersion();

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Schedules_Clienti");

                entity.HasOne(d => d.IdLocationNavigation)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.IdLocation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Schedules_Locations");

                entity.HasOne(d => d.TipoLezione)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.IdTipoLezione)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Schedules_TipoLezioni");

            });

            modelBuilder.Entity<TipologieAbbonamenti>(entity =>
            {
                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.TipologieAbbonamenti)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TipologieAbbonamenti_Clienti");
            });

            modelBuilder.Entity<TipologiaCliente>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<TipologieImmagini>(entity =>
            {
                entity.HasIndex(e => e.Codice)
                    .HasName("UQ_TipologieImmagini_Codice")
                    .IsUnique();

                entity.Property(e => e.Codice).IsUnicode(false);

                entity.Property(e => e.DataCreazione).HasDefaultValueSql("(sysdatetime())");
            });

            modelBuilder.Entity<TipologieLezioni>(entity =>
            {
                entity.Property(e => e.Livello).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.TipologieLezioni)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TipologieLezioni_Clienti");
            });
        }
    }
}
