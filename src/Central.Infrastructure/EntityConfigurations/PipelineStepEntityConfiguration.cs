using Central.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Central.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity type configuration for PipelineStepEntity.
/// </summary>
public sealed class PipelineStepEntityConfiguration : IEntityTypeConfiguration<PipelineStepEntity>
{
    public void Configure(EntityTypeBuilder<PipelineStepEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.PipelineId).IsRequired();
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.StepType).IsRequired();
        builder.Property(e => e.Order).IsRequired();
        builder.Property(e => e.TaskId);
        builder.Property(e => e.WaitDurationSeconds);

        // Configure relationship with pipeline
        builder.HasOne(e => e.Pipeline)
            .WithMany(p => p.Steps)
            .HasForeignKey(e => e.PipelineId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure optional relationship with task
        builder.HasOne(e => e.Task)
            .WithMany()
            .HasForeignKey(e => e.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.PipelineId);
        builder.HasIndex(e => new { e.PipelineId, e.Order });
    }
}
