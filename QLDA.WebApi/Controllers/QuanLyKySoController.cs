using System.Net.Mime;
using QLDA.Application.KySos.Commands;
using QLDA.Application.KySos.DTOs;
using QLDA.Application.KySos.Queries;

namespace QLDA.WebApi.Controllers;

[Tags("Quản lý ký số")]
[Route("api/quan-ly-ky-so")]
public class QuanLyKySoController(IServiceProvider serviceProvider) : AggregateRootController(serviceProvider) {

    [HttpGet("{id}/chi-tiet")]
    [ProducesResponseType<ResultApi<KySoDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResultApi>(StatusCodes.Status400BadRequest)]
    public async Task<ResultApi> Get(Guid id) {
        var entity = await Mediator.Send(new KySoGetQuery { Id = id });
        return ResultApi.Ok(entity.ToDto());
    }

    [HttpGet("danh-sach")]
    [ProducesResponseType<ResultApi<PaginatedList<KySoDto>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResultApi>(StatusCodes.Status400BadRequest)]
    public async Task<ResultApi> GetList([FromQuery] KySoSearchDto searchDto,
        [FromQuery] AggregateRootPagination pagination) {
        var res = await Mediator.Send(new KySoGetDanhSachQuery(searchDto) {
            PageIndex = pagination.PageIndex,
            PageSize = pagination.PageSize,
            GlobalFilter = searchDto.GlobalFilter
        });
        return ResultApi.Ok(res);
    }

    [HttpPost("them-moi")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType<ResultApi<KySoDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResultApi>(StatusCodes.Status400BadRequest)]
    public async Task<ResultApi> Insert([FromBody] KySoInsertDto dto) {
        var entity = await Mediator.Send(new KySoInsertCommand(dto));
        return ResultApi.Ok(entity.ToDto());
    }

    [HttpPut("cap-nhat")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType<ResultApi<KySoDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResultApi>(StatusCodes.Status400BadRequest)]
    public async Task<ResultApi> Update([FromBody] KySoUpdateModel model) {
        var entity = await Mediator.Send(new KySoUpdateCommand(model));
        return ResultApi.Ok(entity.ToDto());
    }

    [HttpDelete("{id}/xoa-tam")]
    [ProducesResponseType<ResultApi<int>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResultApi>(StatusCodes.Status400BadRequest)]
    public async Task<ResultApi> SoftDelete(Guid id) {
        await Mediator.Send(new KySoDeleteCommand(id));
        return ResultApi.Ok(1);
    }
}
