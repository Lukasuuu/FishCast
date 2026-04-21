using FishCast.Models;

namespace FishCast.ViewModels
{
    public class PeixeCatalogoViewModel
    {
        public List<Peixe> Peixes { get; set; } = new List<Peixe>();
        public string? FiltroHabitat { get; set; }
        public string? TermoPesquisa { get; set; }
    }

    public class PeixeDetailViewModel
    {
        public Peixe Peixe { get; set; } = null!;
        public List<CapturaResumoViewModel> Capturas { get; set; } = new List<CapturaResumoViewModel>();
        public int TotalCapturas { get; set; }
        public decimal? MaiorPeso { get; set; }
    }

    public class CapturaResumoViewModel
    {
        public int Id { get; set; }
        public string? Titulo { get; set; }
        public decimal? PesoKg { get; set; }
        public DateTime DataHora { get; set; }
        public string? ImagemPath { get; set; }
        public string? NomeUtilizador { get; set; }
        public string? Local { get; set; }
    }
}
