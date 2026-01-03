using Central.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Central.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity type configuration for ProcessDefinitionEntity.
/// </summary>
public sealed class ProcessDefinitionEntityConfiguration : IEntityTypeConfiguration<ProcessDefinitionEntity>
{
    public void Configure(EntityTypeBuilder<ProcessDefinitionEntity> builder)
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
            .WithOne(s => s.ProcessDefinition)
            .HasForeignKey(s => s.ProcessDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.Name);
        builder.HasIndex(e => e.Enabled);
        builder.HasIndex(e => e.TriggerState);
    }
}