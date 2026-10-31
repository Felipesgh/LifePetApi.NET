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
public class TutoresController(ITutorService tutores, IPetService pets, HateoasFactory hateoas) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponseDto<TutorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<TutorDto>>> Get([FromQuery] QueryParameters query, CancellationToken cancellationToken)
    {
        var result = await tutores.GetAsync(query, cancellationToken);
        return Ok(hateoas.Page(result, nameof(Get), "Tutores", query, tutor =>
        [
            hateoas.Action("self", nameof(GetById), "Tutores", new { id = tutor.Id }),
            hateoas.Action("pets", nameof(GetPets), "Tutores", new { tutorId = tutor.Id }),
            hateoas.Action("update", nameof(Update), "Tutores", new { id = tutor.Id }, "PUT"),
            hateoas.Action("delete", nameof(Delete), "Tutores", new { id = tutor.Id }, "DELETE")
        ]));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ResourceDto<TutorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResourceDto<TutorDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var tutor = await tutores.GetByIdAsync(id, cancellationToken);
        return Ok(hateoas.Wrap(tutor,
            hateoas.Action("self", nameof(GetById), "Tutores", new { id }),
            hateoas.Action("pets", nameof(GetPets), "Tutores", new { tutorId = id }),
            hateoas.Action("collection", nameof(Get), "Tutores", null)));
    }

    [HttpGet("email/{email}")]
    [ProducesResponseType(typeof(ResourceDto<TutorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResourceDto<TutorDto>>> GetByEmail(string email, CancellationToken cancellationToken)
    {
        var tutor = await tutores.GetByEmailAsync(email, cancellationToken);
        return Ok(hateoas.Wrap(tutor, hateoas.Action("self", nameof(GetById), "Tutores", new { id = tutor.Id })));
    }

    [HttpGet("{tutorId:int}/pets")]
    [ProducesResponseType(typeof(PagedResponseDto<PetDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<PetDto>>> GetPets(int tutorId, [FromQuery] QueryParameters query, CancellationToken cancellationToken)
    {
        await tutores.GetByIdAsync(tutorId, cancellationToken);
        var result = await pets.GetAsync(new PetQueryParameters
        {
            Page = query.Page,
            PageSize = query.PageSize,
            SortBy = query.SortBy,
            SortDirection = query.SortDirection,
            Search = query.Search,
            TutorId = tutorId
        }, cancellationToken);

        return Ok(hateoas.Page(result, nameof(GetPets), "Tutores", new { tutorId }, pet =>
        [
            hateoas.Action("self", nameof(PetsController.GetById), "Pets", new { id = pet.Id })
        ]));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResourceDto<TutorDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResourceDto<TutorDto>>> Create([FromBody] CreateTutorRequest request, CancellationToken cancellationToken)
    {
        var tutor = await tutores.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = tutor.Id }, hateoas.Wrap(tutor, hateoas.Action("self", nameof(GetById), "Tutores", new { id = tutor.Id })));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTutorRequest request, CancellationToken cancellationToken)
    {
        await tutores.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await tutores.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
