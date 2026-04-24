using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QLDA.Domain.Entities.DanhMuc;

namespace QLDA.Persistence.Configurations.DanhMuc;

public class DanhMucBuocManHinhConfiguration : AggregateRootConfiguration<DanhMucBuocManHinh> {
    public override void Configure(EntityTypeBuilder<DanhMucBuocManHinh> builder) {
        builder.ToTable("DmBuocManHinh");
        builder.HasKey(e => new { e.LeftId, e.RightId });

        builder.Property(e => e.LeftId).HasColumnName("BuocId");
        builder.Property(e => e.RightId).HasColumnName("ManHinhId");

        builder.Property(e => e.Stt)
            .HasColumnType("int");

        builder.HasOne(e => e.Buoc)
            .WithMany(e => e.BuocManHinhs)
            .HasForeignKey(e => e.LeftId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
