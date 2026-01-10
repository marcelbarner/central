using Central.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Central.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity type configuration for PipelineEntity.
/// </summary>
public sealed class PipelineEntityConfiguration : IEntityTypeConfiguration<PipelineEntity>
{
    public void Configure(EntityTypeBuilder<PipelineEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.Enabled).IsRequired();
        builder.Property(e => e.TriggerState).IsRequired();
        builder.Property(e => e.Created).IsRequired();
        builder.Property(e => e.Updated).IsRequired();

        // Configure relationship with steps
        builder.HasMany(e => e.Steps)
            .WithOne(s => s.Pipeline)
            .HasForeignKey(s => s.PipelineId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.Name).IsUnique();
        builder.HasIndex(e => e.Enabled);
        builder.HasIndex(e => e.TriggerState);
    }
}
