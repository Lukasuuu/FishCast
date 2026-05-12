using FishCast.Models;

namespace FishCast.ViewModels
{
    // Catálogo de espécies com filtros para Peixe/Index.cshtml
    public class PeixeCatalogoViewModel
    {
        public List<Peixe> Peixes { get; set; } = new();
        public string? FiltroHabitat { get; set; }
        public string? TermoPesquisa { get; set; }
    }

    // Detalhe de uma espécie com estatísticas agregadas das capturas registadas
    public class PeixeDetailViewModel
    {
        public Peixe Peixe { get; set; } = null!;
        public List<CapturaResumoViewModel> Capturas { get; set; } = new();  // últimas 12 capturas
        public int TotalCapturas { get; set; }
        public decimal? MaiorPeso { get; set; }
    }

    // Projeção leve de Captura para a grelha de capturas na página de detalhe do peixe
    public class CapturaResumoViewModel
    {
        public int Id { get; set; }
        public string? Titulo { get; set; }
        public decimal? PesoKg { get; set; }
        public DateTime DataHora { get; set; }
        public string? ImagemPath { get; set; }
        public string? NomeUtilizador { get; set; }
    }
}
