using FishCast.Models;

namespace FishCast.ViewModels
{
    // Dados paginados para Home/Index.cshtml (feed principal)
    public class FeedViewModel
    {
        public List<Captura> Capturas { get; set; } = new();
        public int PaginaAtual { get; set; }
        public int TotalPaginas { get; set; }
        public string? FiltroPraia { get; set; }
        public string? FiltroEspecie { get; set; }
        public List<string> Praias { get; set; } = new();   // opções do filtro, lidas da BD
        public List<Peixe> Especies { get; set; } = new();  // opções do filtro, lidas da BD
    }

    // Cartão de captura com dados calculados (likes, comentários, estado do utilizador atual).
    // Usado em Explorar — o Home usa Captura diretamente por razões de paginação simples.
    public class CapturaCardViewModel
    {
        public Captura Captura { get; set; } = null!;
        public bool IsLikedByCurrentUser { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
    }
}
