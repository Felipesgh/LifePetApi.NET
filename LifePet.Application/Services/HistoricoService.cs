using LifePet.Application.DTOs;
using LifePet.Domain.Entities;
using LifePet.Domain.Exceptions;
using LifePet.Domain.Repositories;

namespace LifePet.Application.Services;

public class HistoricoService(
    IHistoricoRepository historicos,
    IPetRepository pets,
    IUnitOfWork unitOfWork) : IHistoricoService
{
    public async Task<PagedResult<HistoricoDto>> GetAsync(HistoricoQueryParameters query, CancellationToken cancellationToken = default)
    {
        var result = await historicos.GetPagedAsync(query.ToPagedQuery(), query.PetId, query.Tipo, cancellationToken);
        return new PagedResult<HistoricoDto>
        {
            Items = result.Items.Select(Map).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems
        };
    }

    public async Task<HistoricoDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var historico = await historicos.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Histórico {id} não encontrado.");
        return Map(historico);
    }

    public async Task<HistoricoDto> CreateAsync(CreateHistoricoRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Descricao))
            throw new BusinessException("Descrição é obrigatória.");
        if (!await pets.ExistsAsync(request.PetId, cancellationToken))
            throw new BusinessException("PetId inválido.");

        var historico = new Historico
        {
            Data = request.Data,
            Descricao = request.Descricao.Trim(),
            Tipo = request.Tipo?.Trim() ?? string.Empty,
            PetId = request.PetId
        };

        await historicos.AddAsync(historico, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(historico);
    }

    public async Task UpdateAsync(int id, UpdateHistoricoRequest request, CancellationToken cancellationToken = default)
    {
        var historico = await historicos.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Histórico {id} não encontrado.");

        if (string.IsNullOrWhiteSpace(request.Descricao))
            throw new BusinessException("Descrição é obrigatória.");
        if (!await pets.ExistsAsync(request.PetId, cancellationToken))
            throw new BusinessException("PetId inválido.");

        historico.Data = request.Data;
        historico.Descricao = request.Descricao.Trim();
        historico.Tipo = request.Tipo?.Trim() ?? string.Empty;
        historico.PetId = request.PetId;
        historicos.Update(historico);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var historico = await historicos.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Histórico {id} não encontrado.");
        historicos.Remove(historico);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static HistoricoDto Map(Historico historico) => new()
    {
        Id = historico.Id,
        Data = historico.Data,
        Descricao = historico.Descricao,
        Tipo = historico.Tipo,
        PetId = historico.PetId
    };
}

public interface IHistoricoService
{
    Task<PagedResult<HistoricoDto>> GetAsync(HistoricoQueryParameters query, CancellationToken cancellationToken = default);
    Task<HistoricoDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<HistoricoDto> CreateAsync(CreateHistoricoRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateHistoricoRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
