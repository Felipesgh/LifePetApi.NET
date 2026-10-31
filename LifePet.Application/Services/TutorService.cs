using LifePet.Application.DTOs;
using LifePet.Domain.Entities;
using LifePet.Domain.Exceptions;
using LifePet.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LifePet.Application.Services;

public class TutorService(
    ITutorRepository tutors,
    IUnitOfWork unitOfWork,
    ILogger<TutorService> logger) : ITutorService
{
    public async Task<PagedResult<TutorDto>> GetAsync(DTOs.Common.QueryParameters query, CancellationToken cancellationToken = default)
    {
        var result = await tutors.GetPagedAsync(query.ToPagedQuery(), cancellationToken);
        return new PagedResult<TutorDto>
        {
            Items = result.Items.Select(Map).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems
        };
    }

    public async Task<TutorDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var tutor = await tutors.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Tutor {id} não encontrado.");
        return Map(tutor);
    }

    public async Task<TutorDto> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var tutor = await tutors.GetByEmailAsync(email, cancellationToken)
            ?? throw new NotFoundException($"Tutor com e-mail {email} não encontrado.");
        return Map(tutor);
    }

    public async Task<TutorDto> CreateAsync(CreateTutorRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) || string.IsNullOrWhiteSpace(request.Email))
            throw new BusinessException("Nome e e-mail são obrigatórios.");

        var tutor = new Tutor
        {
            Nome = request.Nome.Trim(),
            Email = request.Email.Trim(),
            Telefone = request.Telefone?.Trim() ?? string.Empty
        };

        await tutors.AddAsync(tutor, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Tutor {TutorId} criado", tutor.Id);
        return Map(tutor);
    }

    public async Task UpdateAsync(int id, UpdateTutorRequest request, CancellationToken cancellationToken = default)
    {
        var tutor = await tutors.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Tutor {id} não encontrado.");

        if (string.IsNullOrWhiteSpace(request.Nome) || string.IsNullOrWhiteSpace(request.Email))
            throw new BusinessException("Nome e e-mail são obrigatórios.");

        tutor.Nome = request.Nome.Trim();
        tutor.Email = request.Email.Trim();
        tutor.Telefone = request.Telefone?.Trim() ?? string.Empty;
        tutors.Update(tutor);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var tutor = await tutors.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Tutor {id} não encontrado.");
        tutors.Remove(tutor);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static TutorDto Map(Tutor tutor) => new()
    {
        Id = tutor.Id,
        Nome = tutor.Nome,
        Email = tutor.Email,
        Telefone = tutor.Telefone
    };
}

public interface ITutorService
{
    Task<PagedResult<TutorDto>> GetAsync(DTOs.Common.QueryParameters query, CancellationToken cancellationToken = default);
    Task<TutorDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TutorDto> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<TutorDto> CreateAsync(CreateTutorRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateTutorRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
