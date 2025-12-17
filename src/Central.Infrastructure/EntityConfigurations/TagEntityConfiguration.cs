using Central.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Central.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity type configuration for TagEntity.
/// </summary>
public sealed class TagEntityConfiguration : IEntityTypeConfiguration<TagEntity>
{
    public void Configure(EntityTypeBuilder<TagEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.Created).IsRequired();
        builder.Property(e => e.Updated).IsRequired();

        // Unique constraint on tag name
        builder.HasIndex(e => e.Name).IsUnique();

        // Many-to-many relationship with documents
        builder.HasMany(e => e.Documents)
            .WithMany(e => e.Tags)
            .UsingEntity<Dictionary<string, object>>(
                "DocumentTags",
                j => j.HasOne<DocumentEntity>().WithMany().HasForeignKey("DocumentId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<TagEntity>().WithMany().HasForeignKey("TagId").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("DocumentId", "TagId");
                    j.ToTable("DocumentTags");
                });
    }
}
