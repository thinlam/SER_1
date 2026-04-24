using QLDA.Application.DuAnCongViecs.Commands;
using QLDA.Application.DuAnCongViecs.DTOs;
using QLDA.Application.DuAnCongViecs.Queries;
using QLDA.Domain.Constants;

namespace QLDA.WebApi.Controllers;

[Tags("Dự án - Công việc")]
[Route("api/du-an-cong-viec")]
public class DuAnCongViecController(IServiceProvider serviceProvider) : AggregateRootController(serviceProvider) {

    /// <summary>
    /// Danh sách liên kết dự án - công việc
    /// </summary>
    [HttpGet("danh-sach")]
    [ProducesResponseType<ResultApi<PaginatedList<DuAnCongViecDto>>>(StatusCodes.Status200OK)]
    public async Task<ResultApi> GetDanhSach([FromQuery] DuAnCongViecSearchDto searchDto) {
        var res = await Mediator.Send(new DuAnCongViecGetDanhSachQuery(searchDto));
        return ResultApi.Ok(res);
    }

    /// <summary>
    /// Chi tiết liên kết dự án - công việc
    /// </summary>
    [HttpGet("{duAnId}/{congViecId}/chi-tiet")]
    [ProducesResponseType<ResultApi<DuAnCongViecDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResultApi>(StatusCodes.Status400BadRequest)]
    public async Task<ResultApi> GetChiTiet(Guid duAnId, long congViecId) {
        var res = await Mediator.Send(new DuAnCongViecGetChiTietQuery(duAnId, congViecId));
        return ResultApi.Ok(res);
    }

    /// <summary>
    /// Tạo liên kết dự án - công việc
    /// </summary>
    [HttpPost("them-moi")]
    [Authorize(Roles = RoleConstants.GroupAdminOrManager)]
    [ProducesResponseType<ResultApi<DuAnCongViec>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResultApi>(StatusCodes.Status400BadRequest)]
    public async Task<ResultApi> Insert([FromBody] DuAnCongViecInsertDto dto) {
        var res = await Mediator.Send(new DuAnCongViecInsertCommand(dto));
        return ResultApi.Ok(res);
    }

    /// <summary>
    /// Cập nhật liên kết dự án - công việc
    /// </summary>
    [HttpPut("cap-nhat")]
    [Authorize(Roles = RoleConstants.GroupAdminOrManager)]
    [ProducesResponseType<ResultApi<DuAnCongViec>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResultApi>(StatusCodes.Status400BadRequest)]
    public async Task<ResultApi> Update([FromBody] DuAnCongViecUpdateDto dto) {
        var res = await Mediator.Send(new DuAnCongViecUpdateCommand(dto));
        return ResultApi.Ok(res);
    }

    /// <summary>
    /// Xóa liên kết dự án - công việc
    /// </summary>
    [HttpDelete("{duAnId}/{congViecId}/xoa-tam")]
    [Authorize(Roles = RoleConstants.GroupAdminOrManager)]
    [ProducesResponseType<ResultApi<bool>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResultApi>(StatusCodes.Status400BadRequest)]
    public async Task<ResultApi> Delete(Guid duAnId, long congViecId) {
        var res = await Mediator.Send(new DuAnCongViecDeleteCommand(duAnId, congViecId));
        return ResultApi.Ok(res);
    }
}