using LifePet.Application.DTOs;
using LifePet.Application.DTOs.Common;
using LifePet.Application.Services;
using LifePetApi.Hateoas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LifePetApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
public class HistoricosController(IHistoricoService historicos, HateoasFactory hateoas) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponseDto<HistoricoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<HistoricoDto>>> Get([FromQuery] HistoricoQueryParameters query, CancellationToken cancellationToken)
    {
        var result = await historicos.GetAsync(query, cancellationToken);
        return Ok(hateoas.Page(result, nameof(Get), "Historicos", query, item => ItemLinks(item.Id)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResourceDto<HistoricoDto>>> GetById(int id, CancellationToken cancellationToken)
        => Ok(hateoas.Wrap(await historicos.GetByIdAsync(id, cancellationToken), ItemLinks(id)));

    [HttpGet("pet/{petId:int}")]
    public Task<ActionResult<PagedResponseDto<HistoricoDto>>> GetByPet(int petId, [FromQuery] QueryParameters query, CancellationToken cancellationToken)
        => Get(new HistoricoQueryParameters { Page = query.Page, PageSize = query.PageSize, SortBy = query.SortBy, SortDirection = query.SortDirection, Search = query.Search, PetId = petId }, cancellationToken);

    [HttpGet("tipo/{tipo}")]
    public Task<ActionResult<PagedResponseDto<HistoricoDto>>> GetByTipo(string tipo, [FromQuery] QueryParameters query, CancellationToken cancellationToken)
        => Get(new HistoricoQueryParameters { Page = query.Page, PageSize = query.PageSize, SortBy = query.SortBy, SortDirection = query.SortDirection, Search = query.Search, Tipo = tipo }, cancellationToken);

    [HttpPost]
    public async Task<ActionResult<ResourceDto<HistoricoDto>>> Create([FromBody] CreateHistoricoRequest request, CancellationToken cancellationToken)
    {
        var historico = await historicos.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = historico.Id }, hateoas.Wrap(historico, ItemLinks(historico.Id)));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateHistoricoRequest request, CancellationToken cancellationToken)
    {
        await historicos.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await historicos.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    private LinkDto[] ItemLinks(int id) =>
    [
        hateoas.Action("self", nameof(GetById), "Historicos", new { id }),
        hateoas.Action("collection", nameof(Get), "Historicos", null)
    ];
}
