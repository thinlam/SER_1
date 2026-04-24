using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace QLDA.Persistence.Configurations;

public class DuAnBuocManHinhConfiguration : AggregateRootConfiguration<DuAnBuocManHinh> {
    public override void Configure(EntityTypeBuilder<DuAnBuocManHinh> builder) {
        builder.ToTable(nameof(DuAnBuocManHinh));

        builder.HasKey(e => new { e.LeftId, e.RightId });

        builder.Property(e => e.LeftId).HasColumnName("BuocId");
        builder.Property(e => e.RightId).HasColumnName("ManHinhId");

        builder.Property(e => e.Stt)
            .HasColumnType("int");

        builder.HasOne(e => e.DuAnBuoc)
            .WithMany(e => e.DuAnBuocManHinhs)
            .HasForeignKey(e => e.LeftId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.ManHinh)
            .WithMany(e => e.DuAnBuocManHinhs)
            .HasForeignKey(e => e.RightId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
