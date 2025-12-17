using Central.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Central.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity type configuration for DocumentTypeEntity.
/// </summary>
public sealed class DocumentTypeEntityConfiguration : IEntityTypeConfiguration<DocumentTypeEntity>
{
    public void Configure(EntityTypeBuilder<DocumentTypeEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.Created).IsRequired();
        builder.Property(e => e.Updated).IsRequired();

        // Unique constraint on document type name
        builder.HasIndex(e => e.Name).IsUnique();

        // One-to-many relationship with documents
        builder.HasMany(e => e.Documents)
            .WithOne(e => e.DocumentType)
            .HasForeignKey(e => e.DocumentTypeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}