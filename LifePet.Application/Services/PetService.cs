using LifePet.Application.DTOs;
using LifePet.Domain.Entities;
using LifePet.Domain.Exceptions;
using LifePet.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LifePet.Application.Services;

public class PetService(
    IPetRepository pets,
    ITutorRepository tutors,
    IUnitOfWork unitOfWork,
    ILogger<PetService> logger) : IPetService
{
    public async Task<PagedResult<PetDto>> GetAsync(PetQueryParameters query, CancellationToken cancellationToken = default)
    {
        var result = await pets.GetPagedAsync(query.ToPagedQuery(), query.TutorId, query.Especie, cancellationToken);
        return new PagedResult<PetDto>
        {
            Items = result.Items.Select(Map).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems
        };
    }

    public async Task<PetDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var pet = await pets.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Pet {id} não encontrado.");
        return Map(pet);
    }

    public async Task<PetDto> CreateAsync(CreatePetRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new BusinessException("Nome é obrigatório.");

        if (!await tutors.ExistsAsync(request.TutorId, cancellationToken))
            throw new BusinessException("TutorId inválido.");

        var pet = new Pet
        {
            Nome = request.Nome.Trim(),
            Especie = request.Especie?.Trim() ?? string.Empty,
            Raca = request.Raca?.Trim() ?? string.Empty,
            DataNascimento = request.DataNascimento,
            TutorId = request.TutorId
        };

        await pets.AddAsync(pet, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Pet {PetId} criado para tutor {TutorId}", pet.Id, pet.TutorId);
        return Map(pet);
    }

    public async Task UpdateAsync(int id, UpdatePetRequest request, CancellationToken cancellationToken = default)
    {
        var pet = await pets.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Pet {id} não encontrado.");

        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new BusinessException("Nome é obrigatório.");

        if (!await tutors.ExistsAsync(request.TutorId, cancellationToken))
            throw new BusinessException("TutorId inválido.");

        pet.Nome = request.Nome.Trim();
        pet.Especie = request.Especie?.Trim() ?? string.Empty;
        pet.Raca = request.Raca?.Trim() ?? string.Empty;
        pet.DataNascimento = request.DataNascimento;
        pet.TutorId = request.TutorId;
        pets.Update(pet);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var pet = await pets.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Pet {id} não encontrado.");
        pets.Remove(pet);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static PetDto Map(Pet pet) => new()
    {
        Id = pet.Id,
        Nome = pet.Nome,
        Especie = pet.Especie,
        Raca = pet.Raca,
        DataNascimento = pet.DataNascimento,
        TutorId = pet.TutorId
    };
}

public interface IPetService
{
    Task<PagedResult<PetDto>> GetAsync(PetQueryParameters query, CancellationToken cancellationToken = default);
    Task<PetDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PetDto> CreateAsync(CreatePetRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdatePetRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
