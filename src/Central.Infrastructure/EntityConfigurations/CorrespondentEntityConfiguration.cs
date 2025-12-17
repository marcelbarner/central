using Central.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Central.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity configuration for <see cref="CorrespondentEntity"/>.
/// </summary>
public class CorrespondentEntityConfiguration : IEntityTypeConfiguration<CorrespondentEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<CorrespondentEntity> builder)
    {
        builder.ToTable("Correspondents");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(e => e.Name)
            .IsUnique();

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.Created)
            .IsRequired();

        builder.Property(e => e.Updated)
            .IsRequired();

        builder.HasMany(e => e.Documents)
            .WithOne(d => d.Correspondent)
            .HasForeignKey(d => d.CorrespondentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}