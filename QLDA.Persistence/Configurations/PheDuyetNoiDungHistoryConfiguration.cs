using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QLDA.Domain.Entities;

namespace QLDA.Persistence.Configurations;

public class PheDuyetNoiDungHistoryConfiguration : AggregateRootConfiguration<PheDuyetNoiDungHistory> {
    public override void Configure(EntityTypeBuilder<PheDuyetNoiDungHistory> builder) {
        builder.ToTable(nameof(PheDuyetNoiDungHistory));
        builder.ConfigureForBase();

        builder.HasOne(e => e.PheDuyetNoiDung)
            .WithMany(e => e.Histories)
            .HasForeignKey(e => e.PheDuyetNoiDungId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.DuAn)
            .WithMany()
            .HasForeignKey(e => e.DuAnId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
