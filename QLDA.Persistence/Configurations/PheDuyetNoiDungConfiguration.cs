using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QLDA.Domain.Entities;

namespace QLDA.Persistence.Configurations;

public class PheDuyetNoiDungConfiguration : AggregateRootConfiguration<PheDuyetNoiDung> {
    public override void Configure(EntityTypeBuilder<PheDuyetNoiDung> builder) {
        builder.ToTable(nameof(PheDuyetNoiDung));
        builder.ConfigureForBase();

        builder.HasOne(e => e.VanBanQuyetDinh)
            .WithMany()
            .HasForeignKey(e => e.VanBanQuyetDinhId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.DuAn)
            .WithMany()
            .HasForeignKey(e => e.DuAnId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.DuAnBuoc)
            .WithMany()
            .HasForeignKey(e => e.BuocId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Histories)
            .WithOne(e => e.PheDuyetNoiDung)
            .HasForeignKey(e => e.PheDuyetNoiDungId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
