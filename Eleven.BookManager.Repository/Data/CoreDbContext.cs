using Eleven.BookManager.Entity;
using Eleven.BookManager.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Eleven.BookManager.Repository.Data
{
    public partial class CoreDbContext : DbContext
    {
        private readonly IRepositoryConfiguration _configuration;

        public virtual DbSet<Author> Authors { get; set; } = null!;
        public virtual DbSet<AuthorBook> AuthorBooks { get; set; } = null!;
        public virtual DbSet<Book> Books { get; set; } = null!;

        public CoreDbContext(IRepositoryConfiguration configuration)
        {
            _configuration = configuration;

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var connectionString = _configuration.GetConnectionString();

            optionsBuilder.UseSqlite(connectionString, opt => { });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.NormalizedName, "IX_Author_NormalizedName");
            });

            modelBuilder.Entity<AuthorBook>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.AuthorBooks)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.AuthorBooks)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.NormalizedTitle, "IX_Book_NormalizedTitle");
            });
        }
    }
}
