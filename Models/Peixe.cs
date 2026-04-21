namespace FishCast.Models
{
    public class Peixe
    {
        public int Id { get; set; }

        public string? Nome { get; set; }

        public string? Tipo { get; set; }

        public string? Especie { get; set; }

        public int? TamanhoMedioCm { get; set; }

        // Nome científico
        public string? NomeCientifico { get; set; }

        // Descrição do peixe
        public string? Descricao { get; set; }

        // URL de imagem ilustrativa
        public string? ImagemUrl { get; set; }

        // Habitat: "Mar", "Rio", "Estuário"
        public string? HabitatTipo { get; set; }

        // Relacionamento com capturas
        public ICollection<Captura> Capturas { get; set; } = new List<Captura>();
    }
}
