namespace LifePet.Application.DTOs;

public sealed class HistoricoDto
{
    public int Id { get; init; }
    public DateTime Data { get; init; }
    public string Descricao { get; init; } = string.Empty;
    public string Tipo { get; init; } = string.Empty;
    public int PetId { get; init; }
}

public sealed class CreateHistoricoRequest
{
    public DateTime Data { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int PetId { get; set; }
}

public sealed class UpdateHistoricoRequest
{
    public DateTime Data { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int PetId { get; set; }
}

public sealed class HistoricoQueryParameters : Common.QueryParameters
{
    public int? PetId { get; set; }
    public string? Tipo { get; set; }
}
