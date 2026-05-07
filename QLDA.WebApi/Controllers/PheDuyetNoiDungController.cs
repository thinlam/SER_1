using System.Net.Mime;
using QLDA.Application.PheDuyetNoiDungs.Commands;
using QLDA.Application.PheDuyetNoiDungs.DTOs;
using QLDA.Application.PheDuyetNoiDungs.Queries;
using QLDA.WebApi.Models.PheDuyetNoiDungs;

namespace QLDA.WebApi.Controllers;

[Tags("Phê duyệt nội dung trình duyệt")]
[Route("api/phe-duyet-noi-dung")]
public class PheDuyetNoiDungController : AggregateRootController {
    public PheDuyetNoiDungController(IServiceProvider serviceProvider) : base(serviceProvider) {
    }

    /// <summary>
    /// Danh sách nội dung trình duyệt (phân trang)
    /// </summary>
    [ProducesResponseType<ResultApi<PaginatedList<PheDuyetNoiDungDto>>>(StatusCodes.Status200OK)]
    [HttpGet("danh-sach")]
    public async Task<ResultApi> GetDanhSach([FromQuery] PheDuyetNoiDungSearchModel model) {
        var res = await Mediator.Send(new PheDuyetNoiDungGetDanhSachQuery {
            DuAnId = model.DuAnId,
            BuocId = model.BuocId,
            TrangThai = model.TrangThai,
            LoaiVanBan = model.LoaiVanBan,
            GlobalFilter = model.GlobalFilter,
            PageIndex = model.PageIndex,
            PageSize = model.PageSize
        });
        return ResultApi.Ok(res);
    }

    /// <summary>
    /// Chi tiết nội dung phê duyệt
    /// </summary>
    [ProducesResponseType<ResultApi<PheDuyetNoiDungChiTietDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResultApi>(StatusCodes.Status400BadRequest)]
    [HttpGet("{id}/chi-tiet")]
    public async Task<ResultApi> GetChiTiet(Guid id) {
        var res = await Mediator.Send(new PheDuyetNoiDungGetChiTietQuery(id));
        return ResultApi.Ok(res);
    }

    /// <summary>
    /// Lịch sử xử lý nội dung phê duyệt
    /// </summary>
    [ProducesResponseType<ResultApi<List<PheDuyetNoiDungLichSuDto>>>(StatusCodes.Status200OK)]
    [HttpGet("{id}/lich-su")]
    public async Task<ResultApi> GetLichSu(Guid id) {
        var res = await Mediator.Send(new PheDuyetNoiDungGetLichSuQuery(id));
        return ResultApi.Ok(res);
    }

    /// <summary>
    /// Trình nội dung từ tiến độ
    /// </summary>
    [ProducesResponseType<ResultApi<int>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResultApi>(StatusCodes.Status400BadRequest)]
    [HttpPost("{vanBanQuyetDinhId}/trinh")]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ResultApi> Trinh(Guid vanBanQuyetDinhId, [FromBody] PheDuyetNoiDungTrinhModel model) {
        var res = await Mediator.Send(new PheDuyetNoiDungTrinhCommand(
            vanBanQuyetDinhId, model.DuAnId, model.BuocId, model.NoiDung));
        return ResultApi.Ok(res);
    }

    /// <summary>
    /// Duyệt nội dung - BGĐ
    /// </summary>
    [ProducesResponseType<ResultApi<int>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResultApi>(StatusCodes.Status400BadRequest)]
    [HttpPost("{id}/duyet")]
    public async Task<ResultApi> Duyet(Guid id) {
        var res = await Mediator.Send(new PheDuyetNoiDungDuyetCommand(id));
        return ResultApi.Ok(res);
    }

    /// <summary>
    /// Từ chối nội dung - BGĐ, cần lý do
    /// </summary>
    [ProducesResponseType<ResultApi<int>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResultApi>(StatusCodes.Status400BadRequest)]
    [HttpPost("{id}/tu-choi")]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ResultApi> TuChoi(Guid id, [FromBody] PheDuyetNoiDungTuChoiModel model) {
        var res = await Mediator.Send(new PheDuyetNoiDungTuChoiCommand(id, model.NoiDung));
        return ResultApi.Ok(res);
    }

    /// <summary>
    /// Trả lại nội dung - BGĐ, cần lý do
    /// </summary>
    [ProducesResponseType<ResultApi<int>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResultApi>(StatusCodes.Status400BadRequest)]
    [HttpPost("{id}/tra-lai")]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ResultApi> TraLai(Guid id, [FromBody] PheDuyetNoiDungTraLaiModel model) {
        var res = await Mediator.Send(new PheDuyetNoiDungTraLaiCommand(id, model.NoiDung));
        return ResultApi.Ok(res);
    }

    /// <summary>
    /// Ký số nội dung - BGĐ
    /// </summary>
    [ProducesResponseType<ResultApi<int>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResultApi>(StatusCodes.Status400BadRequest)]
    [HttpPost("{id}/ky-so")]
    public async Task<ResultApi> KySo(Guid id) {
        var res = await Mediator.Send(new PheDuyetNoiDungKySoCommand(id));
        return ResultApi.Ok(res);
    }

    /// <summary>
    /// Chuyển QLVB - BGĐ
    /// </summary>
    [ProducesResponseType<ResultApi<int>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResultApi>(StatusCodes.Status400BadRequest)]
    [HttpPost("{id}/chuyen-qlvb")]
    public async Task<ResultApi> ChuyenQLVB(Guid id) {
        var res = await Mediator.Send(new PheDuyetNoiDungChuyenQLVBCommand(id));
        return ResultApi.Ok(res);
    }

    /// <summary>
    /// Phát hành - P.HC-TH
    /// </summary>
    [ProducesResponseType<ResultApi<int>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResultApi>(StatusCodes.Status400BadRequest)]
    [HttpPost("{id}/phat-hanh")]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ResultApi> PhatHanh(Guid id, [FromBody] PheDuyetNoiDungPhatHanhModel model) {
        var res = await Mediator.Send(new PheDuyetNoiDungPhatHanhCommand(
            id, model.SoPhatHanh, model.NgayPhatHanh));
        return ResultApi.Ok(res);
    }

    /// <summary>
    /// Gửi lại sau khi bị trả lại - CB/LĐ
    /// </summary>
    [ProducesResponseType<ResultApi<int>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResultApi>(StatusCodes.Status400BadRequest)]
    [HttpPost("{id}/gui-lai")]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ResultApi> GuiLai(Guid id, [FromBody] PheDuyetNoiDungGuiLaiModel? model) {
        var res = await Mediator.Send(new PheDuyetNoiDungGuiLaiCommand(id, model?.NoiDung));
        return ResultApi.Ok(res);
    }
}
