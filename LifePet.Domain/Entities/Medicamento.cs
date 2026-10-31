namespace LifePet.Domain.Entities;

public class Medicamento
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Dosagem { get; set; } = string.Empty;
    public string Frequencia { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int PetId { get; set; }
    public Pet? Pet { get; set; }
}
