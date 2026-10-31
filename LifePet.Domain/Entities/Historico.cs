namespace LifePet.Domain.Entities;

public class Historico
{
    public int Id { get; set; }
    public DateTime Data { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int PetId { get; set; }
    public Pet? Pet { get; set; }
}
