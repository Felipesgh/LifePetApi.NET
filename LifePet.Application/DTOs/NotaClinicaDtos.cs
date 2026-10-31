namespace LifePet.Application.DTOs;

public sealed class NotaClinicaDto
{
    public string Id { get; init; } = string.Empty;
    public int PetId { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Conteudo { get; init; } = string.Empty;
    public string Autor { get; init; } = string.Empty;
    public DateTime CriadoEm { get; init; }
}

public sealed class CreateNotaClinicaRequest
{
    public int PetId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
}

public sealed class UpdateNotaClinicaRequest
{
    public string Titulo { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
}

public sealed class NotaClinicaQueryParameters : Common.QueryParameters
{
    public int? PetId { get; set; }
}
