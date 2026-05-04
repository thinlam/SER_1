using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QLDA.Domain.Entities.DanhMuc;

namespace QLDA.Persistence.Configurations.DanhMuc;

public class DmCapDoCnttConfiguration : AggregateRootConfiguration<DmCapDoCntt>
{
    public override void Configure(EntityTypeBuilder<DmCapDoCntt> builder)
    {
        builder.ToTable("DmCapDoCntt");
        builder.ConfigureForDanhMuc();
        builder.Property(e => e.MaMau).HasMaxLength(100);
        builder.HasMany(e => e.HoSoDeXuatCapDoCntts)
            .WithOne(e => e.CapDo)
            .HasForeignKey(e => e.CapDoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}