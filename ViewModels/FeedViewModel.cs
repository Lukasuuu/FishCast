using FishCast.Models;

namespace FishCast.ViewModels
{
    public class FeedViewModel
    {
        public List<Captura> Capturas { get; set; } = new List<Captura>();
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public string? FiltroPraia { get; set; }
        public string? FiltroEspecie { get; set; }
        public List<string> Praias { get; set; } = new List<string>();
        public List<Peixe> Especies { get; set; } = new List<Peixe>();
    }

    public class CapturaCardViewModel
    {
        public Captura Captura { get; set; } = null!;
        public bool IsLikedByCurrentUser { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
    }
}
