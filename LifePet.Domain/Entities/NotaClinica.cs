namespace LifePet.Domain.Entities;

public class NotaClinica
{
    public string Id { get; set; } = string.Empty;
    public int PetId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
