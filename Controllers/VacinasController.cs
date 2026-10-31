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
public class VacinasController(IVacinaService vacinas, HateoasFactory hateoas) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponseDto<VacinaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<VacinaDto>>> Get([FromQuery] VacinaQueryParameters query, CancellationToken cancellationToken)
    {
        var result = await vacinas.GetAsync(query, cancellationToken);
        return Ok(hateoas.Page(result, nameof(Get), "Vacinas", query, item => ItemLinks(item.Id)));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ResourceDto<VacinaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResourceDto<VacinaDto>>> GetById(int id, CancellationToken cancellationToken)
        => Ok(hateoas.Wrap(await vacinas.GetByIdAsync(id, cancellationToken), ItemLinks(id)));

    [HttpGet("pet/{petId:int}")]
    public Task<ActionResult<PagedResponseDto<VacinaDto>>> GetByPet(int petId, [FromQuery] QueryParameters query, CancellationToken cancellationToken)
        => Get(new VacinaQueryParameters { Page = query.Page, PageSize = query.PageSize, SortBy = query.SortBy, SortDirection = query.SortDirection, Search = query.Search, PetId = petId }, cancellationToken);

    [HttpGet("atrasadas")]
    public Task<ActionResult<PagedResponseDto<VacinaDto>>> GetAtrasadas([FromQuery] QueryParameters query, CancellationToken cancellationToken)
        => Get(new VacinaQueryParameters { Page = query.Page, PageSize = query.PageSize, SortBy = query.SortBy, SortDirection = query.SortDirection, Search = query.Search, Atrasadas = true }, cancellationToken);

    [HttpPost]
    [ProducesResponseType(typeof(ResourceDto<VacinaDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ResourceDto<VacinaDto>>> Create([FromBody] CreateVacinaRequest request, CancellationToken cancellationToken)
    {
        var vacina = await vacinas.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = vacina.Id }, hateoas.Wrap(vacina, ItemLinks(vacina.Id)));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateVacinaRequest request, CancellationToken cancellationToken)
    {
        await vacinas.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await vacinas.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    private LinkDto[] ItemLinks(int id) =>
    [
        hateoas.Action("self", nameof(GetById), "Vacinas", new { id }),
        hateoas.Action("collection", nameof(Get), "Vacinas", null)
    ];
}
