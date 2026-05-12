using Microsoft.EntityFrameworkCore;

namespace FishCast.Models
{
    public class Captura
    {
        public int Id { get; set; }

        // FK para o utilizador dono da captura (string porque Identity usa GUIDs)
        public string? UtilizadorId { get; set; }
        public ApplicationUser? Utilizador { get; set; }

        public int PeixeId { get; set; }
        public Peixe? Peixe { get; set; }

        public string? Local { get; set; }      // campo legado; usar Praia para novo código
        public string? Praia { get; set; }
        public string? TipoPesca { get; set; }
        public string? Mare { get; set; }
        public DateTime DataHora { get; set; } = DateTime.Now;
        public int Quantidade { get; set; }

        [Precision(5, 2)]   // máx. 999.99 kg — suficiente para qualquer espécie
        public decimal? PesoKg { get; set; }

        public string? Observacao { get; set; }
        public string? ImagemPath { get; set; } // caminho relativo a wwwroot (ex: /uploads/capturas/guid.jpg)
        public string? Titulo { get; set; }

        // Coleções de navegação carregadas com Include() nos controllers
        public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public ICollection<CapturaLike> Likes { get; set; } = new List<CapturaLike>();
    }
}
