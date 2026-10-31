namespace LifePet.Domain.Entities;

public class Vacina
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataAplicacao { get; set; }
    public DateTime? DataProximaDose { get; set; }
    public int PetId { get; set; }
    public Pet? Pet { get; set; }
}
