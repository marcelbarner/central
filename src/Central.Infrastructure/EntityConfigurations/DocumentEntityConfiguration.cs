using Central.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Central.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity type configuration for DocumentEntity.
/// </summary>
public sealed class DocumentEntityConfiguration : IEntityTypeConfiguration<DocumentEntity>
{
    public void Configure(EntityTypeBuilder<DocumentEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title).IsRequired().HasMaxLength(500);
        builder.Property(e => e.OriginalFileName).HasMaxLength(500);
        builder.Property(e => e.OriginalFilePath).HasMaxLength(2000);
        builder.Property(e => e.ArchiveFileName).HasMaxLength(500);
        builder.Property(e => e.ArchiveFilePath).HasMaxLength(2000);
        builder.Property(e => e.ThumbnailFileName).HasMaxLength(500);
        builder.Property(e => e.ThumbnailFilePath).HasMaxLength(2000);
        builder.Property(e => e.Added).IsRequired();
        builder.Property(e => e.Updated).IsRequired();
        builder.Property(e => e.State).IsRequired();

        // Configure user relationships with ON DELETE SET NULL
        builder.HasOne(e => e.AddedBy)
            .WithMany()
            .HasForeignKey(e => e.AddedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.UpdatedBy)
            .WithMany()
            .HasForeignKey(e => e.UpdatedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.Title);
        builder.HasIndex(e => e.DocumentDate);
        builder.HasIndex(e => e.AddedById);
        builder.HasIndex(e => e.UpdatedById);
    }
}