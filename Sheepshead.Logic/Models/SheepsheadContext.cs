using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sheepshead.Logic.Models
{
    public partial class SheepsheadContext : DbContext
    {
        public virtual DbSet<Coin> Coin { get; set; }
        public virtual DbSet<Game> Game { get; set; }
        public virtual DbSet<Hand> Hand { get; set; }
        public virtual DbSet<Participant> Participant { get; set; }
        public virtual DbSet<ParticipantRefusingPick> ParticipantRefusingPick { get; set; }
        public virtual DbSet<Point> Point { get; set; }
        public virtual DbSet<Trick> Trick { get; set; }
        public virtual DbSet<TrickPlay> TrickPlay { get; set; }

        public SheepsheadContext(DbContextOptions<SheepsheadContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //optionsBuilder.UseSqlServer(@"data source=DESKTOP-SVNAB17\SQLEXPRESS;initial catalog=Sheepshead;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Coin>(entity =>
            {
                entity.HasIndex(e => e.HandId)
                    .HasName("IX_FK_Hand_Coin");

                entity.HasIndex(e => e.ParticipantId)
                    .HasName("IX_FK_Coin_Participant");

                entity.HasOne(d => d.Hand)
                    .WithMany(p => p.Coin)
                    .HasForeignKey(d => d.HandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Hand_Coin");

                entity.HasOne(d => d.Participant)
                    .WithMany(p => p.Coin)
                    .HasForeignKey(d => d.ParticipantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Coin_Participant");
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.PartnerMethod)
                    .IsRequired()
                    .HasColumnType("char(1)");
            });

            modelBuilder.Entity<Hand>(entity =>
            {
                entity.HasIndex(e => e.GameId)
                    .HasName("IX_FK_Hand_Game");

                entity.HasIndex(e => e.PartnerParticipantId)
                    .HasName("IX_FK_Hand_Partner");

                entity.HasIndex(e => e.PickerParticipantId)
                    .HasName("IX_FK_Hand_Picker");

                entity.HasIndex(e => e.StartingParticipantId)
                    .HasName("IX_FK_Hand_StartingParticipant");

                entity.Property(e => e.BlindCards)
                    .IsRequired()
                    .HasColumnType("char(6)");

                entity.Property(e => e.BuriedCards)
                    .IsRequired()
                    .HasColumnType("char(6)");

                entity.Property(e => e.PartnerCardEnum).HasColumnType("char(2)");

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.Hand)
                    .HasForeignKey(d => d.GameId)
                    .HasConstraintName("FK_Hand_Game");

                entity.HasOne(d => d.PartnerParticipant)
                    .WithMany(p => p.HandPartnerParticipant)
                    .HasForeignKey(d => d.PartnerParticipantId)
                    .HasConstraintName("FK_Hand_Partner");

                entity.HasOne(d => d.PickerParticipant)
                    .WithMany(p => p.HandPickerParticipant)
                    .HasForeignKey(d => d.PickerParticipantId)
                    .HasConstraintName("FK_Hand_Picker");

                entity.HasOne(d => d.StartingParticipant)
                    .WithMany(p => p.HandStartingParticipant)
                    .HasForeignKey(d => d.StartingParticipantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Hand_StartingParticipant");
            });

            modelBuilder.Entity<Participant>(entity =>
            {
                entity.HasIndex(e => e.GameId)
                    .HasName("IX_FK_Player_Game");

                entity.Property(e => e.Cards)
                    .IsRequired()
                    .HasColumnType("char(36)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.Type).IsRequired();

                entity.HasOne(d => d.Game)
                    .WithMany(p => p.Participant)
                    .HasForeignKey(d => d.GameId)
                    .HasConstraintName("FK_Player_Game");
            });

            modelBuilder.Entity<ParticipantRefusingPick>(entity =>
            {
                entity.HasKey(e => new { e.HandId, e.ParticipantId });

                entity.HasOne(d => d.Hand)
                    .WithMany(p => p.ParticipantRefusingPick)
                    .HasForeignKey(d => d.HandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ParticipantRefusingPick_Hand");

                entity.HasOne(d => d.Participant)
                    .WithMany(p => p.ParticipantRefusingPick)
                    .HasForeignKey(d => d.ParticipantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ParticipantRefusingPick_Participant");
            });

            modelBuilder.Entity<Point>(entity =>
            {
                entity.HasOne(d => d.Hand)
                    .WithMany(p => p.Point)
                    .HasForeignKey(d => d.HandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PointHand");

                entity.HasOne(d => d.Participant)
                    .WithMany(p => p.Point)
                    .HasForeignKey(d => d.ParticipantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Point_Participant");
            });

            modelBuilder.Entity<Trick>(entity =>
            {
                entity.HasIndex(e => e.HandId)
                    .HasName("IX_FK_Trick_Hand");

                entity.HasIndex(e => e.ParticipantId)
                    .HasName("IX_FK_Trick_Participant");

                entity.HasOne(d => d.Hand)
                    .WithMany(p => p.Trick)
                    .HasForeignKey(d => d.HandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Trick_Hand");

                entity.HasOne(d => d.Participant)
                    .WithMany(p => p.Trick)
                    .HasForeignKey(d => d.ParticipantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Trick_Participant");
            });

            modelBuilder.Entity<TrickPlay>(entity =>
            {
                entity.HasKey(e => new { e.ParticipantId, e.TrickId });

                entity.HasIndex(e => e.TrickId)
                    .HasName("IX_FK_TrickPlay_Trick");

                entity.Property(e => e.Card)
                    .IsRequired()
                    .HasColumnType("char(2)");

                entity.HasOne(d => d.Participant)
                    .WithMany(p => p.TrickPlay)
                    .HasForeignKey(d => d.ParticipantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrickPlay_Participant");

                entity.HasOne(d => d.Trick)
                    .WithMany(p => p.TrickPlay)
                    .HasForeignKey(d => d.TrickId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrickPlay_Trick");
            });
        }
    }
}
