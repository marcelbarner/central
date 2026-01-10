using Central.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Central.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity type configuration for TaskExecutionEntity.
/// </summary>
public sealed class TaskExecutionEntityConfiguration : IEntityTypeConfiguration<TaskExecutionEntity>
{
    public void Configure(EntityTypeBuilder<TaskExecutionEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.TaskId).IsRequired();
        builder.Property(e => e.DocumentId).IsRequired();
        builder.Property(e => e.PipelineExecutionId);
        builder.Property(e => e.Status).IsRequired();
        builder.Property(e => e.StartedAt);
        builder.Property(e => e.CompletedAt);
        builder.Property(e => e.ErrorMessage).HasColumnType("TEXT");
        builder.Property(e => e.Result).HasColumnType("TEXT");

        // Configure relationship with task
        builder.HasOne(e => e.Task)
            .WithMany()
            .HasForeignKey(e => e.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship with document
        builder.HasOne(e => e.Document)
            .WithMany()
            .HasForeignKey(e => e.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure optional relationship with pipeline execution
        builder.HasOne(e => e.PipelineExecution)
            .WithMany(pe => pe.TaskExecutions)
            .HasForeignKey(e => e.PipelineExecutionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.TaskId);
        builder.HasIndex(e => e.DocumentId);
        builder.HasIndex(e => e.PipelineExecutionId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.StartedAt);
    }
}
