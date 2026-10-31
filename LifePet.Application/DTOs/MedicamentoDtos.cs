namespace LifePet.Application.DTOs;

public sealed class MedicamentoDto
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Dosagem { get; init; } = string.Empty;
    public string Frequencia { get; init; } = string.Empty;
    public DateTime DataInicio { get; init; }
    public DateTime? DataFim { get; init; }
    public int PetId { get; init; }
    public bool Ativo { get; init; }
}

public sealed class CreateMedicamentoRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Dosagem { get; set; } = string.Empty;
    public string Frequencia { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int PetId { get; set; }
}

public sealed class UpdateMedicamentoRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Dosagem { get; set; } = string.Empty;
    public string Frequencia { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int PetId { get; set; }
}

public sealed class MedicamentoQueryParameters : Common.QueryParameters
{
    public int? PetId { get; set; }
    public bool Ativos { get; set; }
}
