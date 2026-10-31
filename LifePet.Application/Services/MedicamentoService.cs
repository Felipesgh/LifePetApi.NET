using LifePet.Application.DTOs;
using LifePet.Domain.Entities;
using LifePet.Domain.Exceptions;
using LifePet.Domain.Repositories;
using LifePet.Domain.Services;

namespace LifePet.Application.Services;

public class MedicamentoService(
    IMedicamentoRepository medicamentos,
    IPetRepository pets,
    IMedicamentoDomainService domainService,
    IUnitOfWork unitOfWork) : IMedicamentoService
{
    public async Task<PagedResult<MedicamentoDto>> GetAsync(MedicamentoQueryParameters query, CancellationToken cancellationToken = default)
    {
        var referencia = DateTime.Today;
        var result = await medicamentos.GetPagedAsync(query.ToPagedQuery(), query.PetId, query.Ativos, referencia, cancellationToken);
        return new PagedResult<MedicamentoDto>
        {
            Items = result.Items.Select(m => Map(m, referencia)).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems
        };
    }

    public async Task<MedicamentoDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var medicamento = await medicamentos.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Medicamento {id} não encontrado.");
        return Map(medicamento, DateTime.Today);
    }

    public async Task<MedicamentoDto> CreateAsync(CreateMedicamentoRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new BusinessException("Nome é obrigatório.");
        if (!await pets.ExistsAsync(request.PetId, cancellationToken))
            throw new BusinessException("PetId inválido.");

        var medicamento = new Medicamento
        {
            Nome = request.Nome.Trim(),
            Dosagem = request.Dosagem?.Trim() ?? string.Empty,
            Frequencia = request.Frequencia?.Trim() ?? string.Empty,
            DataInicio = request.DataInicio,
            DataFim = request.DataFim,
            PetId = request.PetId
        };

        await medicamentos.AddAsync(medicamento, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(medicamento, DateTime.Today);
    }

    public async Task UpdateAsync(int id, UpdateMedicamentoRequest request, CancellationToken cancellationToken = default)
    {
        var medicamento = await medicamentos.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Medicamento {id} não encontrado.");

        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new BusinessException("Nome é obrigatório.");
        if (!await pets.ExistsAsync(request.PetId, cancellationToken))
            throw new BusinessException("PetId inválido.");

        medicamento.Nome = request.Nome.Trim();
        medicamento.Dosagem = request.Dosagem?.Trim() ?? string.Empty;
        medicamento.Frequencia = request.Frequencia?.Trim() ?? string.Empty;
        medicamento.DataInicio = request.DataInicio;
        medicamento.DataFim = request.DataFim;
        medicamento.PetId = request.PetId;
        medicamentos.Update(medicamento);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var medicamento = await medicamentos.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Medicamento {id} não encontrado.");
        medicamentos.Remove(medicamento);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private MedicamentoDto Map(Medicamento medicamento, DateTime referencia) => new()
    {
        Id = medicamento.Id,
        Nome = medicamento.Nome,
        Dosagem = medicamento.Dosagem,
        Frequencia = medicamento.Frequencia,
        DataInicio = medicamento.DataInicio,
        DataFim = medicamento.DataFim,
        PetId = medicamento.PetId,
        Ativo = domainService.EstaAtivo(medicamento, referencia)
    };
}

public interface IMedicamentoService
{
    Task<PagedResult<MedicamentoDto>> GetAsync(MedicamentoQueryParameters query, CancellationToken cancellationToken = default);
    Task<MedicamentoDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<MedicamentoDto> CreateAsync(CreateMedicamentoRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateMedicamentoRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
