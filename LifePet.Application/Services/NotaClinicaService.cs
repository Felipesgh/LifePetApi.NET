using LifePet.Application.DTOs;
using LifePet.Domain.Entities;
using LifePet.Domain.Exceptions;
using LifePet.Domain.Repositories;

namespace LifePet.Application.Services;

public class NotaClinicaService(
    INotaClinicaRepository notas,
    IPetRepository pets) : INotaClinicaService
{
    public async Task<PagedResult<NotaClinicaDto>> GetAsync(NotaClinicaQueryParameters query, CancellationToken cancellationToken = default)
    {
        var result = await notas.GetPagedAsync(query.ToPagedQuery(), query.PetId, cancellationToken);
        return new PagedResult<NotaClinicaDto>
        {
            Items = result.Items.Select(Map).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems
        };
    }

    public async Task<NotaClinicaDto> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var nota = await notas.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Nota clínica {id} não encontrada.");
        return Map(nota);
    }

    public async Task<NotaClinicaDto> CreateAsync(CreateNotaClinicaRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Titulo) || string.IsNullOrWhiteSpace(request.Conteudo))
            throw new BusinessException("Título e conteúdo são obrigatórios.");
        if (!await pets.ExistsAsync(request.PetId, cancellationToken))
            throw new BusinessException("PetId inválido.");

        var nota = new NotaClinica
        {
            Id = Guid.NewGuid().ToString("N"),
            PetId = request.PetId,
            Titulo = request.Titulo.Trim(),
            Conteudo = request.Conteudo.Trim(),
            Autor = request.Autor?.Trim() ?? string.Empty,
            CriadoEm = DateTime.UtcNow
        };

        await notas.AddAsync(nota, cancellationToken);
        return Map(nota);
    }

    public async Task UpdateAsync(string id, UpdateNotaClinicaRequest request, CancellationToken cancellationToken = default)
    {
        var nota = await notas.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Nota clínica {id} não encontrada.");

        if (string.IsNullOrWhiteSpace(request.Titulo) || string.IsNullOrWhiteSpace(request.Conteudo))
            throw new BusinessException("Título e conteúdo são obrigatórios.");

        nota.Titulo = request.Titulo.Trim();
        nota.Conteudo = request.Conteudo.Trim();
        nota.Autor = request.Autor?.Trim() ?? string.Empty;
        await notas.UpdateAsync(nota, cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var nota = await notas.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Nota clínica {id} não encontrada.");
        await notas.DeleteAsync(nota.Id, cancellationToken);
    }

    private static NotaClinicaDto Map(NotaClinica nota) => new()
    {
        Id = nota.Id,
        PetId = nota.PetId,
        Titulo = nota.Titulo,
        Conteudo = nota.Conteudo,
        Autor = nota.Autor,
        CriadoEm = nota.CriadoEm
    };
}

public interface INotaClinicaService
{
    Task<PagedResult<NotaClinicaDto>> GetAsync(NotaClinicaQueryParameters query, CancellationToken cancellationToken = default);
    Task<NotaClinicaDto> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<NotaClinicaDto> CreateAsync(CreateNotaClinicaRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(string id, UpdateNotaClinicaRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
