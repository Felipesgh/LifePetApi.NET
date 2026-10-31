using LifePet.Application.DTOs;
using LifePet.Application.DTOs.Common;
using LifePet.Application.Services;
using LifePetApi.Hateoas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LifePetApi.Controllers;

[ApiController]
[Authorize]
[Route("api/notas-clinicas")]
[Produces("application/json")]
public class NotasClinicasController(INotaClinicaService notas, HateoasFactory hateoas) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponseDto<NotaClinicaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<NotaClinicaDto>>> Get([FromQuery] NotaClinicaQueryParameters query, CancellationToken cancellationToken)
    {
        var result = await notas.GetAsync(query, cancellationToken);
        return Ok(hateoas.Page(result, nameof(Get), "NotasClinicas", query, item => ItemLinks(item.Id)));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ResourceDto<NotaClinicaDto>>> GetById(string id, CancellationToken cancellationToken)
        => Ok(hateoas.Wrap(await notas.GetByIdAsync(id, cancellationToken), ItemLinks(id)));

    [HttpPost]
    public async Task<ActionResult<ResourceDto<NotaClinicaDto>>> Create([FromBody] CreateNotaClinicaRequest request, CancellationToken cancellationToken)
    {
        var nota = await notas.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = nota.Id }, hateoas.Wrap(nota, ItemLinks(nota.Id)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateNotaClinicaRequest request, CancellationToken cancellationToken)
    {
        await notas.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        await notas.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    private LinkDto[] ItemLinks(string id) =>
    [
        hateoas.Action("self", nameof(GetById), "NotasClinicas", new { id }),
        hateoas.Action("collection", nameof(Get), "NotasClinicas", null)
    ];
}
