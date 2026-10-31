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
public class ConsultasController(IConsultaService consultas, HateoasFactory hateoas) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponseDto<ConsultaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<ConsultaDto>>> Get([FromQuery] ConsultaQueryParameters query, CancellationToken cancellationToken)
    {
        var result = await consultas.GetAsync(query, cancellationToken);
        return Ok(hateoas.Page(result, nameof(Get), "Consultas", query, item => ItemLinks(item.Id)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResourceDto<ConsultaDto>>> GetById(int id, CancellationToken cancellationToken)
        => Ok(hateoas.Wrap(await consultas.GetByIdAsync(id, cancellationToken), ItemLinks(id)));

    [HttpGet("pet/{petId:int}")]
    public Task<ActionResult<PagedResponseDto<ConsultaDto>>> GetByPet(int petId, [FromQuery] QueryParameters query, CancellationToken cancellationToken)
        => Get(new ConsultaQueryParameters { Page = query.Page, PageSize = query.PageSize, SortBy = query.SortBy, SortDirection = query.SortDirection, Search = query.Search, PetId = petId }, cancellationToken);

    [HttpGet("futuras")]
    public Task<ActionResult<PagedResponseDto<ConsultaDto>>> GetFuturas([FromQuery] QueryParameters query, CancellationToken cancellationToken)
        => Get(new ConsultaQueryParameters { Page = query.Page, PageSize = query.PageSize, SortBy = query.SortBy, SortDirection = query.SortDirection, Search = query.Search, Futuras = true }, cancellationToken);

    [HttpPost]
    public async Task<ActionResult<ResourceDto<ConsultaDto>>> Create([FromBody] CreateConsultaRequest request, CancellationToken cancellationToken)
    {
        var consulta = await consultas.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = consulta.Id }, hateoas.Wrap(consulta, ItemLinks(consulta.Id)));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateConsultaRequest request, CancellationToken cancellationToken)
    {
        await consultas.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await consultas.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    private LinkDto[] ItemLinks(int id) =>
    [
        hateoas.Action("self", nameof(GetById), "Consultas", new { id }),
        hateoas.Action("collection", nameof(Get), "Consultas", null)
    ];
}
