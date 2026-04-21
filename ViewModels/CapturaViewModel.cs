using Microsoft.AspNetCore.Mvc.Rendering;

namespace FishCast.ViewModels
{
    public class CapturaCreateViewModel
    {
        public string? Titulo { get; set; }
        public int PeixeId { get; set; }
        public string? Local { get; set; }
        public string? Praia { get; set; }
        public string? TipoPesca { get; set; }
        public string? Mare { get; set; }
        public DateTime DataHora { get; set; } = DateTime.Now;
        public int Quantidade { get; set; } = 1;
        public decimal? PesoKg { get; set; }
        public string? Observacao { get; set; }
        public IFormFile? Imagem { get; set; }

        public List<SelectListItem> Peixes { get; set; } = new List<SelectListItem>();

        public List<string> Praias { get; set; } = new List<string>
        {
            "Cabedelo", "Praia Norte", "Afife", "Âncora", "Moledo",
            "Montedor", "Vila Praia de Âncora", "Caminha", "Viana do Castelo (Centro)"
        };

        public List<string> TiposPesca { get; set; } = new List<string>
        {
            "Mar", "Rio", "Surf Casting", "Pesca à Bóia", "Lure",
            "Pesca Submarina", "Pesca de Cais", "Pesca de Rocha"
        };

        public List<string> Mares { get; set; } = new List<string>
        {
            "Maré Cheia", "Maré Vazia", "Maré Subindo", "Maré Descendo"
        };
    }

    public class CapturaEditViewModel
    {
        public int Id { get; set; }
        public string? Titulo { get; set; }
        public int PeixeId { get; set; }
        public string? Local { get; set; }
        public string? Praia { get; set; }
        public string? TipoPesca { get; set; }
        public string? Mare { get; set; }
        public DateTime DataHora { get; set; }
        public int Quantidade { get; set; }
        public decimal? PesoKg { get; set; }
        public string? Observacao { get; set; }
        public string? ImagemPathAtual { get; set; }
        public IFormFile? NovaImagem { get; set; }

        public List<SelectListItem> Peixes { get; set; } = new List<SelectListItem>();

        public List<string> Praias { get; set; } = new List<string>
        {
            "Cabedelo", "Praia Norte", "Afife", "Âncora", "Moledo",
            "Montedor", "Vila Praia de Âncora", "Caminha", "Viana do Castelo (Centro)"
        };

        public List<string> TiposPesca { get; set; } = new List<string>
        {
            "Mar", "Rio", "Surf Casting", "Pesca à Bóia", "Lure",
            "Pesca Submarina", "Pesca de Cais", "Pesca de Rocha"
        };

        public List<string> Mares { get; set; } = new List<string>
        {
            "Maré Cheia", "Maré Vazia", "Maré Subindo", "Maré Descendo"
        };
    }

    public class CapturaDetailViewModel
    {
        public FishCast.Models.Captura Captura { get; set; } = null!;
        public int LikesCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public List<ComentarioViewModel> Comentarios { get; set; } = new List<ComentarioViewModel>();
        public string NovoComentario { get; set; } = string.Empty;
        public bool IsOwnCaptura { get; set; }
    }

    public class ComentarioViewModel
    {
        public int Id { get; set; }
        public string? Texto { get; set; }
        public DateTime DataComentario { get; set; }
        public string? NomeUtilizador { get; set; }
        public string? FotoPerfilUtilizador { get; set; }
        public string? UtilizadorId { get; set; }
    }
}
