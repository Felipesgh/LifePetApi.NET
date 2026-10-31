namespace LifePet.Application.DTOs;

public sealed class ConsultaDto
{
    public int Id { get; init; }
    public DateTime DataHora { get; init; }
    public string Veterinario { get; init; } = string.Empty;
    public string Motivo { get; init; } = string.Empty;
    public int PetId { get; init; }
}

public sealed class CreateConsultaRequest
{
    public DateTime DataHora { get; set; }
    public string Veterinario { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public int PetId { get; set; }
}

public sealed class UpdateConsultaRequest
{
    public DateTime DataHora { get; set; }
    public string Veterinario { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public int PetId { get; set; }
}

public sealed class ConsultaQueryParameters : Common.QueryParameters
{
    public int? PetId { get; set; }
    public bool Futuras { get; set; }
}
