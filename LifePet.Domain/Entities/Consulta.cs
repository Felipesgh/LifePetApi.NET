namespace LifePet.Domain.Entities;

public class Consulta
{
    public int Id { get; set; }
    public DateTime DataHora { get; set; }
    public string Veterinario { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public int PetId { get; set; }
    public Pet? Pet { get; set; }
}
