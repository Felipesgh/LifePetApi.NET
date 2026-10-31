using LifePet.Application.DTOs;
using LifePet.Domain.Entities;
using LifePet.Domain.Exceptions;
using LifePet.Domain.Repositories;

namespace LifePet.Application.Services;

public class ConsultaService(
    IConsultaRepository consultas,
    IPetRepository pets,
    IUnitOfWork unitOfWork) : IConsultaService
{
    public async Task<PagedResult<ConsultaDto>> GetAsync(ConsultaQueryParameters query, CancellationToken cancellationToken = default)
    {
        var result = await consultas.GetPagedAsync(query.ToPagedQuery(), query.PetId, query.Futuras, DateTime.Now, cancellationToken);
        return new PagedResult<ConsultaDto>
        {
            Items = result.Items.Select(Map).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems
        };
    }

    public async Task<ConsultaDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var consulta = await consultas.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Consulta {id} não encontrada.");
        return Map(consulta);
    }

    public async Task<ConsultaDto> CreateAsync(CreateConsultaRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Motivo))
            throw new BusinessException("Motivo é obrigatório.");
        if (!await pets.ExistsAsync(request.PetId, cancellationToken))
            throw new BusinessException("PetId inválido.");

        var consulta = new Consulta
        {
            DataHora = request.DataHora,
            Veterinario = request.Veterinario?.Trim() ?? string.Empty,
            Motivo = request.Motivo.Trim(),
            PetId = request.PetId
        };

        await consultas.AddAsync(consulta, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(consulta);
    }

    public async Task UpdateAsync(int id, UpdateConsultaRequest request, CancellationToken cancellationToken = default)
    {
        var consulta = await consultas.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Consulta {id} não encontrada.");

        if (string.IsNullOrWhiteSpace(request.Motivo))
            throw new BusinessException("Motivo é obrigatório.");
        if (!await pets.ExistsAsync(request.PetId, cancellationToken))
            throw new BusinessException("PetId inválido.");

        consulta.DataHora = request.DataHora;
        consulta.Veterinario = request.Veterinario?.Trim() ?? string.Empty;
        consulta.Motivo = request.Motivo.Trim();
        consulta.PetId = request.PetId;
        consultas.Update(consulta);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var consulta = await consultas.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Consulta {id} não encontrada.");
        consultas.Remove(consulta);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static ConsultaDto Map(Consulta consulta) => new()
    {
        Id = consulta.Id,
        DataHora = consulta.DataHora,
        Veterinario = consulta.Veterinario,
        Motivo = consulta.Motivo,
        PetId = consulta.PetId
    };
}

public interface IConsultaService
{
    Task<PagedResult<ConsultaDto>> GetAsync(ConsultaQueryParameters query, CancellationToken cancellationToken = default);
    Task<ConsultaDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ConsultaDto> CreateAsync(CreateConsultaRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateConsultaRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
