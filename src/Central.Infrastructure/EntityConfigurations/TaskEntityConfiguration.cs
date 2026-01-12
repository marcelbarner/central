using Central.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Central.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity type configuration for TaskEntity.
/// </summary>
public sealed class TaskEntityConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.TaskType).IsRequired();
        builder.Property(e => e.Enabled).IsRequired();
        builder.Property(e => e.AzureEndpoint).HasMaxLength(500);
        builder.Property(e => e.AzureApiKey).HasMaxLength(500);
        builder.Property(e => e.AzureModelOrDeployment).HasMaxLength(200);
        builder.Property(e => e.Prompt).HasColumnType("TEXT");
        builder.Property(e => e.Temperature);
        builder.Property(e => e.MaxTokens);
        builder.Property(e => e.Capabilities);
        builder.Property(e => e.DocumentIntelligenceOptions).HasColumnType("TEXT");
        builder.Property(e => e.Created).IsRequired();
        builder.Property(e => e.Updated).IsRequired();

        // Configure owned collection for AllowedTools
        builder.OwnsMany(e => e.AllowedTools, tools =>
        {
            tools.Property(t => t.Tool).IsRequired();
        });

        builder.HasIndex(e => e.Name).IsUnique();
        builder.HasIndex(e => e.Enabled);
        builder.HasIndex(e => e.TaskType);
    }
}
