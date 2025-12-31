using Central.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Central.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity configuration for <see cref="ContractEntity"/>.
/// </summary>
public class ContractEntityConfiguration : IEntityTypeConfiguration<ContractEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ContractEntity> builder)
    {
        builder.ToTable("Contracts");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(e => e.Name)
            .IsUnique();

        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        builder.Property(e => e.CustomerId)
            .HasMaxLength(100);

        builder.Property(e => e.ContractId)
            .HasMaxLength(100);

        builder.Property(e => e.State)
            .IsRequired();

        builder.Property(e => e.Created)
            .IsRequired();

        builder.Property(e => e.Updated)
            .IsRequired();

        builder.HasOne(e => e.Correspondent)
            .WithMany()
            .HasForeignKey(e => e.CorrespondentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(e => e.Documents)
            .WithOne(d => d.Contract)
            .HasForeignKey(d => d.ContractId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}