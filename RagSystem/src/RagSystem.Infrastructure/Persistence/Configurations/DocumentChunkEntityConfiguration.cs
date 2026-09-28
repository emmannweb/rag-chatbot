using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pgvector;
using RagSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace RagSystem.Infrastructure.Persistence.Configurations
{
    public class DocumentChunkEntityConfiguration : IEntityTypeConfiguration<DocumentChunk>
    {
        public void Configure(EntityTypeBuilder<DocumentChunk> builder)
        {
            builder.ToTable("document_chunks");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("file_name");

            builder.Property(e => e.Content)
                .IsRequired()
                .HasColumnName("content");

            // 1. Define the converter separately
            var converter = new ValueConverter<float[], Vector>(
                v => new Vector(v),
                v => v.ToArray()
            );

            // 2. Define the expression-safe value comparer separately
            var comparer = new ValueComparer<float[]>(
                (Expression<Func<float[]?, float[]?, bool>>) ((c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2)),
                (Expression<Func<float[], int>>) (c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode()))),
                (Expression<Func<float[], float[]>>) (c => c == null ? Array.Empty<float>() : c.ToArray())
            );

            // 3. Pass both into HasConversion
            builder.Property(e => e.Embedding)
                .HasColumnName("embedding")
                .HasColumnType("vector(768)") // Matches nomic-embed-text dimensions
                .HasConversion(converter, comparer);
        }
    }

}
