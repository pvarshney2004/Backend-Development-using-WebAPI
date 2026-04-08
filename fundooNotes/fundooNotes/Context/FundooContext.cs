using fundooNotes.Models;
using Microsoft.EntityFrameworkCore;

namespace fundooNotes.Context
{
    public class FundooContext : DbContext
    {
        public FundooContext(DbContextOptions<FundooContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<Collaborator> Collaborators { get; set; }
        public DbSet<NoteLabel> NoteLabels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Collaborator Configuration
            modelBuilder.Entity<Collaborator>()
                .HasOne(c => c.User)
                .WithMany(u => u.Collaborators)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Collaborator>()
                .HasOne(c => c.Note)
                .WithMany(n => n.Collaborators)
                .HasForeignKey(c => c.NoteId)
                .OnDelete(DeleteBehavior.Cascade);

            //  Label → User
            modelBuilder.Entity<Label>()
                .HasOne(l => l.User)
                .WithMany(u => u.Labels)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Label>()
                .HasIndex(l => new { l.LabelName, l.UserId })
                .IsUnique();

            // Many-to-Many via NoteLabel
            modelBuilder.Entity<NoteLabel>()
                .HasKey(nl => new { nl.NoteId, nl.LabelId });

            modelBuilder.Entity<NoteLabel>()
                .HasOne(nl => nl.Note)
                .WithMany(n => n.NoteLabels)
                .HasForeignKey(nl => nl.NoteId);

            modelBuilder.Entity<NoteLabel>()
                .HasOne(nl => nl.Label)
                .WithMany(l => l.NoteLabels)
                .HasForeignKey(nl => nl.LabelId);


            modelBuilder.Entity<UserSession>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
