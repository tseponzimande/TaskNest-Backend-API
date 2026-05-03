namespace TaskNest.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        #region Database tables

        public DbSet<Board> Boards { get; set; } = null!;
        public DbSet<BoardColumn> BoardColumns { get; set; } = null!;
        public DbSet<TaskItem> Tasks { get; set; } = null!;
        public DbSet<BoardUser> BoardUsers { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<ActivityLog> ActivityLogs { get; set; } = null!;
        public DbSet<TaskAttachment> TaskAttachments { get; set; } = null!;

        #endregion


        #region entity
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Board>()
                .Ignore(b => b.ApplicationUserId);

            #region Board entity

            modelBuilder.Entity<Board>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Name)
                      .IsRequired()
                      .HasMaxLength(200);
                entity.Property(b => b.Description)
                      .HasMaxLength(500);

                entity.HasOne(b => b.User)
                      .WithMany(u => u.Boards)
                      .HasForeignKey(b => b.ApplicationUserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            #endregion

            #region BoardColumn entity

            modelBuilder.Entity<BoardColumn>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(100);


                entity.HasOne(c => c.Board)
                      .WithMany(b => b.BoardColumns)
                      .HasForeignKey(c => c.BoardId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            #endregion

            #region TaskItem entity
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Title)
                      .IsRequired()
                      .HasMaxLength(300);
                entity.Property(t => t.Description)
                      .HasMaxLength(1000);


                entity.HasOne(t => t.Board)
                      .WithMany(b => b.Tasks)
                      .HasForeignKey(t => t.BoardId)
                      .OnDelete(DeleteBehavior.Restrict);

      
                entity.HasOne(t => t.Column)
                      .WithMany(c => c.Tasks)
                      .HasForeignKey(t => t.ColumnId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            #endregion

            #region BoardUser entity

            modelBuilder.Entity<BoardUser>(entity =>
            {
                entity.HasKey(bu => bu.Id);
                entity.HasOne(bu => bu.Board)
                      .WithMany(b => b.BoardUsers)
                      .HasForeignKey(bu => bu.BoardId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(bu => bu.ApplicationUser)
                      .WithMany(u => u.BoardUsers)
                      .HasForeignKey(bu => bu.ApplicationUserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            #endregion

            #region Comment entity

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Content)
                      .IsRequired()
                      .HasMaxLength(2000);

                entity.HasOne(c => c.TaskItem)
                      .WithMany()
                      .HasForeignKey(c => c.TaskItemId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.User)
                      .WithMany()
                      .HasForeignKey(c => c.ApplicationUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion

            #region ActivityLog entity

            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Action)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(a => a.Details)
                      .HasMaxLength(500);

                entity.HasOne(a => a.TaskItem)
                      .WithMany()
                      .HasForeignKey(a => a.TaskItemId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.User)
                      .WithMany()
                      .HasForeignKey(a => a.ApplicationUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region File Attachement

            modelBuilder.Entity<TaskAttachment>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.FileName)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(a => a.OriginalFileName)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(a => a.ContentType)
                      .HasMaxLength(200);

                entity.Property(a => a.FilePath)
                      .IsRequired()
                      .HasMaxLength(1000);

                entity.HasOne(a => a.TaskItem)
                      .WithMany()
                      .HasForeignKey(a => a.TaskItemId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.User)
                      .WithMany()
                      .HasForeignKey(a => a.ApplicationUserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion

        }

        #endregion
    }
}