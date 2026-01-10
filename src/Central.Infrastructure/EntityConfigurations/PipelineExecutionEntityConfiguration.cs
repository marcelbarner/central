using Central.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Central.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity type configuration for PipelineExecutionEntity.
/// </summary>
public sealed class PipelineExecutionEntityConfiguration : IEntityTypeConfiguration<PipelineExecutionEntity>
{
    public void Configure(EntityTypeBuilder<PipelineExecutionEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.PipelineId).IsRequired();
        builder.Property(e => e.DocumentId).IsRequired();
        builder.Property(e => e.Status).IsRequired();
        builder.Property(e => e.StartedAt);
        builder.Property(e => e.CompletedAt);
        builder.Property(e => e.ErrorMessage).HasColumnType("TEXT");

        // Configure relationship with pipeline
        builder.HasOne(e => e.Pipeline)
            .WithMany()
            .HasForeignKey(e => e.PipelineId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship with document
        builder.HasOne(e => e.Document)
            .WithMany()
            .HasForeignKey(e => e.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship with task executions
        builder.HasMany(e => e.TaskExecutions)
            .WithOne(te => te.PipelineExecution)
            .HasForeignKey(te => te.PipelineExecutionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.PipelineId);
        builder.HasIndex(e => e.DocumentId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.StartedAt);
    }
}
