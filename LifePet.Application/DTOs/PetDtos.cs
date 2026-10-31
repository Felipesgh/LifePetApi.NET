namespace LifePet.Application.DTOs;

public sealed class PetDto
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Especie { get; init; } = string.Empty;
    public string Raca { get; init; } = string.Empty;
    public DateTime DataNascimento { get; init; }
    public int TutorId { get; init; }
}

public sealed class CreatePetRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string Raca { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public int TutorId { get; set; }
}

public sealed class UpdatePetRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string Raca { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public int TutorId { get; set; }
}

public sealed class PetQueryParameters : Common.QueryParameters
{
    public int? TutorId { get; set; }
    public string? Especie { get; set; }
}
