using Microsoft.EntityFrameworkCore;

namespace FishCast.Models
{
    public class Captura
    {
        public int Id { get; set; }

        // O Id do utilizador que fez a captura
        public string? UtilizadorId { get; set; }
        public ApplicationUser? Utilizador { get; set; }

        // O Id do peixe capturado
        public int PeixeId { get; set; }
        public Peixe? Peixe { get; set; }

        public string? Local { get; set; }

        public string? TipoPesca { get; set; }

        public string? Mare { get; set; }

        public DateTime DataHora { get; set; } = DateTime.Now;

        public int Quantidade { get; set; }

        [Precision(5, 2)] // define a precisão para 5 dígitos no total, com 2 casas decimais
        public decimal? PesoKg { get; set; }

        public string? Observacao { get; set; }

        public string? ImagemPath { get; set; }

        // Título da publicação
        public string? Titulo { get; set; }

        // Praia onde foi capturado
        public string? Praia { get; set; }

        // Relacionamento com comentários (uma captura pode ter muitos comentários)
        public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

        // Likes na captura
        public ICollection<CapturaLike> Likes { get; set; } = new List<CapturaLike>();
    }
}
