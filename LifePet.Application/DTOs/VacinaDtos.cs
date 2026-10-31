namespace LifePet.Application.DTOs;

public sealed class VacinaDto
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public DateTime DataAplicacao { get; init; }
    public DateTime? DataProximaDose { get; init; }
    public int PetId { get; init; }
    public bool Atrasada { get; init; }
}

public sealed class CreateVacinaRequest
{
    public string Nome { get; set; } = string.Empty;
    public DateTime DataAplicacao { get; set; }
    public DateTime? DataProximaDose { get; set; }
    public int PetId { get; set; }
}

public sealed class UpdateVacinaRequest
{
    public string Nome { get; set; } = string.Empty;
    public DateTime DataAplicacao { get; set; }
    public DateTime? DataProximaDose { get; set; }
    public int PetId { get; set; }
}

public sealed class VacinaQueryParameters : Common.QueryParameters
{
    public int? PetId { get; set; }
    public bool Atrasadas { get; set; }
}
