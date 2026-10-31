using LifePet.Application.DTOs;
using LifePet.Domain.Entities;
using LifePet.Domain.Exceptions;
using LifePet.Domain.Repositories;
using LifePet.Domain.Services;

namespace LifePet.Application.Services;

public class VacinaService(
    IVacinaRepository vacinas,
    IPetRepository pets,
    IVacinaDomainService domainService,
    IUnitOfWork unitOfWork) : IVacinaService
{
    public async Task<PagedResult<VacinaDto>> GetAsync(VacinaQueryParameters query, CancellationToken cancellationToken = default)
    {
        var referencia = DateTime.Today;
        var result = await vacinas.GetPagedAsync(query.ToPagedQuery(), query.PetId, query.Atrasadas, referencia, cancellationToken);
        return new PagedResult<VacinaDto>
        {
            Items = result.Items.Select(v => Map(v, referencia)).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems
        };
    }

    public async Task<VacinaDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var vacina = await vacinas.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Vacina {id} não encontrada.");
        return Map(vacina, DateTime.Today);
    }

    public async Task<VacinaDto> CreateAsync(CreateVacinaRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new BusinessException("Nome é obrigatório.");
        if (!await pets.ExistsAsync(request.PetId, cancellationToken))
            throw new BusinessException("PetId inválido.");

        var vacina = new Vacina
        {
            Nome = request.Nome.Trim(),
            DataAplicacao = request.DataAplicacao,
            DataProximaDose = request.DataProximaDose,
            PetId = request.PetId
        };

        await vacinas.AddAsync(vacina, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(vacina, DateTime.Today);
    }

    public async Task UpdateAsync(int id, UpdateVacinaRequest request, CancellationToken cancellationToken = default)
    {
        var vacina = await vacinas.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Vacina {id} não encontrada.");

        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new BusinessException("Nome é obrigatório.");
        if (!await pets.ExistsAsync(request.PetId, cancellationToken))
            throw new BusinessException("PetId inválido.");

        vacina.Nome = request.Nome.Trim();
        vacina.DataAplicacao = request.DataAplicacao;
        vacina.DataProximaDose = request.DataProximaDose;
        vacina.PetId = request.PetId;
        vacinas.Update(vacina);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var vacina = await vacinas.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Vacina {id} não encontrada.");
        vacinas.Remove(vacina);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private VacinaDto Map(Vacina vacina, DateTime referencia) => new()
    {
        Id = vacina.Id,
        Nome = vacina.Nome,
        DataAplicacao = vacina.DataAplicacao,
        DataProximaDose = vacina.DataProximaDose,
        PetId = vacina.PetId,
        Atrasada = domainService.EstaAtrasada(vacina, referencia)
    };
}

public interface IVacinaService
{
    Task<PagedResult<VacinaDto>> GetAsync(VacinaQueryParameters query, CancellationToken cancellationToken = default);
    Task<VacinaDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<VacinaDto> CreateAsync(CreateVacinaRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateVacinaRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
