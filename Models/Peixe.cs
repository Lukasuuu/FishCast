namespace FishCast.Models;

public class Peixe
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? Tipo { get; set; }
    public string? Especie { get; set; }
    public int? TamanhoMedioCm { get; set; }
    public string? NomeCientifico { get; set; }
    public string? Descricao { get; set; }
    public string? ImagemUrl { get; set; }
    public string? HabitatTipo { get; set; }
    public ICollection<Captura>? Capturas { get; set; }
}
