using Central.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Central.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity configuration for <see cref="WebhookEntity"/>.
/// </summary>
public class WebhookEntityConfiguration : IEntityTypeConfiguration<WebhookEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<WebhookEntity> builder)
    {
        builder.ToTable("Webhooks");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EventType)
            .IsRequired();

        builder.HasIndex(e => e.EventType);

        builder.Property(e => e.Url)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(e => e.Created)
            .IsRequired();

        builder.Property(e => e.Updated)
            .IsRequired();
    }
}
