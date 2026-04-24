using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace QLDA.Persistence.Configurations;

public class DuAnNguonVonConfiguration : AggregateRootConfiguration<DuAnNguonVon> {
    public override void Configure(EntityTypeBuilder<DuAnNguonVon> builder) {
        builder.ToTable(nameof(DuAnNguonVon));

        builder.HasKey(e => new { e.LeftId, e.RightId });

        builder.Property(e => e.LeftId).HasColumnName("DuAnId");
        builder.Property(e => e.RightId).HasColumnName("NguonVonId");

        builder.HasOne(e => e.DuAn)
            .WithMany(e => e.DuAnNguonVons)
            .HasForeignKey(e => e.LeftId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.NguonVon)
            .WithMany(e => e.DuAnNguonVons)
            .HasForeignKey(e => e.RightId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
