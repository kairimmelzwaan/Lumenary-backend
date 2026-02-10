using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using backend.Models;

namespace backend.Data.Configurations;

public class AppointmentConfig : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(a => a.ClientId)
            .IsRequired();

        builder.Property(a => a.TherapistUserId)
            .IsRequired();

        builder.Property(a => a.CreatedByUserId)
            .IsRequired();

        builder.Property(a => a.StartsAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(a => a.EndsAt)
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(a => a.Timezone)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(a => a.Status)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(a => a.Mode)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(a => a.Location)
            .HasMaxLength(256);

        builder.Property(a => a.Notes)
            .HasColumnType("text");

        builder.Property(a => a.CanceledAt)
            .HasColumnType("timestamptz");

        builder.Property(a => a.CancellationReason)
            .HasMaxLength(256);

        builder.Property(a => a.CreatedAt)
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("now()");

        builder.Property(a => a.UpdatedAt)
            .HasColumnType("timestamptz")
            .HasDefaultValueSql("now()");

        builder.HasIndex(a => new { a.TherapistUserId, a.StartsAt });
        builder.HasIndex(a => new { a.ClientId, a.StartsAt });

        builder.HasOne(a => a.Client)
            .WithMany()
            .HasForeignKey(a => a.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.TherapistUser)
            .WithMany()
            .HasForeignKey(a => a.TherapistUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.CreatedByUser)
            .WithMany()
            .HasForeignKey(a => a.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
