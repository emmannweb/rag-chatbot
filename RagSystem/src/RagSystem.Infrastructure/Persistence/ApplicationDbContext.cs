using Microsoft.EntityFrameworkCore;
using RagSystem.Domain.Entities;
using RagSystem.Infrastructure.Persistence.Configurations;


namespace RagSystem.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<DocumentChunk> DocumentChunks { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Automatically runs "CREATE EXTENSION IF NOT EXISTS vector;" during migrations
            modelBuilder.HasPostgresExtension("vector");

            // Explicit configuration registration convention
            modelBuilder.ApplyConfiguration(new DocumentChunkEntityConfiguration());
        }
    }

}

