using Central.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Central.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity type configuration for ProcessExecutionEntity.
/// </summary>
public sealed class ProcessExecutionEntityConfiguration : IEntityTypeConfiguration<ProcessExecutionEntity>
{
    public void Configure(EntityTypeBuilder<ProcessExecutionEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ProcessDefinitionId).IsRequired();
        builder.Property(e => e.DocumentId).IsRequired();
        builder.Property(e => e.Status).IsRequired();

        // Configure relationships
        builder.HasOne(e => e.ProcessDefinition)
            .WithMany()
            .HasForeignKey(e => e.ProcessDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Document)
            .WithMany()
            .HasForeignKey(e => e.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Steps)
            .WithOne(s => s.ProcessExecution)
            .HasForeignKey(s => s.ProcessExecutionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.ProcessDefinitionId);
        builder.HasIndex(e => e.DocumentId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.StartedAt);
    }
}