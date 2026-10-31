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
public class MedicamentosController(IMedicamentoService medicamentos, HateoasFactory hateoas) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponseDto<MedicamentoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<MedicamentoDto>>> Get([FromQuery] MedicamentoQueryParameters query, CancellationToken cancellationToken)
    {
        var result = await medicamentos.GetAsync(query, cancellationToken);
        return Ok(hateoas.Page(result, nameof(Get), "Medicamentos", query, item => ItemLinks(item.Id)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResourceDto<MedicamentoDto>>> GetById(int id, CancellationToken cancellationToken)
        => Ok(hateoas.Wrap(await medicamentos.GetByIdAsync(id, cancellationToken), ItemLinks(id)));

    [HttpGet("pet/{petId:int}")]
    public Task<ActionResult<PagedResponseDto<MedicamentoDto>>> GetByPet(int petId, [FromQuery] QueryParameters query, CancellationToken cancellationToken)
        => Get(new MedicamentoQueryParameters { Page = query.Page, PageSize = query.PageSize, SortBy = query.SortBy, SortDirection = query.SortDirection, Search = query.Search, PetId = petId }, cancellationToken);

    [HttpGet("ativos")]
    public Task<ActionResult<PagedResponseDto<MedicamentoDto>>> GetAtivos([FromQuery] QueryParameters query, CancellationToken cancellationToken)
        => Get(new MedicamentoQueryParameters { Page = query.Page, PageSize = query.PageSize, SortBy = query.SortBy, SortDirection = query.SortDirection, Search = query.Search, Ativos = true }, cancellationToken);

    [HttpPost]
    public async Task<ActionResult<ResourceDto<MedicamentoDto>>> Create([FromBody] CreateMedicamentoRequest request, CancellationToken cancellationToken)
    {
        var medicamento = await medicamentos.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = medicamento.Id }, hateoas.Wrap(medicamento, ItemLinks(medicamento.Id)));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMedicamentoRequest request, CancellationToken cancellationToken)
    {
        await medicamentos.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await medicamentos.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    private LinkDto[] ItemLinks(int id) =>
    [
        hateoas.Action("self", nameof(GetById), "Medicamentos", new { id }),
        hateoas.Action("collection", nameof(Get), "Medicamentos", null)
    ];
}
