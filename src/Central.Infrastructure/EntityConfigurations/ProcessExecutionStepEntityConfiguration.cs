using Central.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Central.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity type configuration for ProcessExecutionStepEntity.
/// </summary>
public sealed class ProcessExecutionStepEntityConfiguration : IEntityTypeConfiguration<ProcessExecutionStepEntity>
{
    public void Configure(EntityTypeBuilder<ProcessExecutionStepEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ProcessExecutionId).IsRequired();
        builder.Property(e => e.StepName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.StepType).IsRequired();
        builder.Property(e => e.Order).IsRequired();
        builder.Property(e => e.Status).IsRequired();

        builder.HasIndex(e => e.ProcessExecutionId);
        builder.HasIndex(e => new { e.ProcessExecutionId, e.Order });
        builder.HasIndex(e => e.Status);
    }
}