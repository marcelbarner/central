using Central.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Central.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity type configuration for ProcessingStepEntity.
/// </summary>
public sealed class ProcessingStepEntityConfiguration : IEntityTypeConfiguration<ProcessingStepEntity>
{
    public void Configure(EntityTypeBuilder<ProcessingStepEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ProcessDefinitionId).IsRequired();
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.StepType).IsRequired();
        builder.Property(e => e.Order).IsRequired();
        builder.Property(e => e.AzureEndpoint).HasMaxLength(500);
        builder.Property(e => e.AzureApiKey).HasMaxLength(500);
        builder.Property(e => e.AzureModelOrDeployment).HasMaxLength(200);

        builder.HasIndex(e => e.ProcessDefinitionId);
        builder.HasIndex(e => new { e.ProcessDefinitionId, e.Order });
    }
}