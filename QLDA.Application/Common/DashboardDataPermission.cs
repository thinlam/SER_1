using Microsoft.EntityFrameworkCore;

namespace QLDA.Application.Common;

/// <summary>
/// Phạm vi dữ liệu của user hiện tại cho các dashboard vốn / giải ngân.
/// - trinh.vo → xem toàn bộ dự án thuộc trung tâm (IsTrinhVo = true, không filter LanhDaoPhuTrachId).
/// - user khác → chỉ dự án do mình phụ trách (LanhDaoPhuTrachId == UserId).
/// </summary>
public readonly record struct DashboardDataScope(bool IsTrinhVo, long UserId);

public static class DashboardDataPermission
{
    public static async Task<DashboardDataScope> ResolveAsync(
        IServiceProvider serviceProvider, CancellationToken ct = default)
    {
        var userProvider = serviceProvider.GetRequiredService<IUserProvider>();
        var userMaster = serviceProvider.GetRequiredService<IRepository<UserMaster, long>>();

        var userId = userProvider.Info.UserID;

        var userName = await userMaster.GetQueryableSet()
            .Where(u => u.UserPortalId == userId)
            .Select(u => u.UserName)
            .FirstOrDefaultAsync(ct);

        return new DashboardDataScope(
            IsTrinhVo: string.Equals(userName, "trinh.vo", StringComparison.OrdinalIgnoreCase),
            UserId: userId);
    }
}
