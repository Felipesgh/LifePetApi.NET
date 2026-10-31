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
public class PetsController(IPetService pets, HateoasFactory hateoas) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponseDto<PetDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<PetDto>>> Get([FromQuery] PetQueryParameters query, CancellationToken cancellationToken)
    {
        var result = await pets.GetAsync(query, cancellationToken);
        return Ok(hateoas.Page(result, nameof(Get), "Pets", query, pet => PetLinks(pet.Id)));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ResourceDto<PetDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResourceDto<PetDto>>> GetById(int id, CancellationToken cancellationToken)
        => Ok(hateoas.Wrap(await pets.GetByIdAsync(id, cancellationToken), PetLinks(id)));

    [HttpGet("especie/{especie}")]
    [ProducesResponseType(typeof(PagedResponseDto<PetDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<PetDto>>> GetByEspecie(string especie, [FromQuery] QueryParameters query, CancellationToken cancellationToken)
        => await Get(new PetQueryParameters
        {
            Page = query.Page,
            PageSize = query.PageSize,
            SortBy = query.SortBy,
            SortDirection = query.SortDirection,
            Search = query.Search,
            Especie = especie
        }, cancellationToken);

    [HttpGet("tutor/{tutorId:int}")]
    [ProducesResponseType(typeof(PagedResponseDto<PetDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<PetDto>>> GetByTutor(int tutorId, [FromQuery] QueryParameters query, CancellationToken cancellationToken)
        => await Get(new PetQueryParameters
        {
            Page = query.Page,
            PageSize = query.PageSize,
            SortBy = query.SortBy,
            SortDirection = query.SortDirection,
            Search = query.Search,
            TutorId = tutorId
        }, cancellationToken);

    [HttpPost]
    [ProducesResponseType(typeof(ResourceDto<PetDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ResourceDto<PetDto>>> Create([FromBody] CreatePetRequest request, CancellationToken cancellationToken)
    {
        var pet = await pets.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = pet.Id }, hateoas.Wrap(pet, PetLinks(pet.Id)));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePetRequest request, CancellationToken cancellationToken)
    {
        await pets.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await pets.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    private LinkDto[] PetLinks(int id) =>
    [
        hateoas.Action("self", nameof(GetById), "Pets", new { id }),
        hateoas.Action("vacinas", nameof(VacinasController.Get), "Vacinas", new { petId = id }),
        hateoas.Action("consultas", nameof(ConsultasController.Get), "Consultas", new { petId = id }),
        hateoas.Action("collection", nameof(Get), "Pets", null)
    ];
}
