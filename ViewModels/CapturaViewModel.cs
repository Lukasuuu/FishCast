using FishCast.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FishCast.ViewModels
{
    // ViewModels são DTOs entre Controller e View.
    // Nunca expõem o Model diretamente — isolam a View das mudanças no domínio.

    // Usado em Captura/Create.cshtml (GET preenche listas; POST recebe dados do formulário)
    public class CapturaCreateViewModel
    {
        public string? Titulo { get; set; }
        public int PeixeId { get; set; }
        public string? Praia { get; set; }
        public string? TipoPesca { get; set; }
        public string? Mare { get; set; }
        public DateTime DataHora { get; set; } = DateTime.Now;
        public int Quantidade { get; set; } = 1;
        public decimal? PesoKg { get; set; }
        public string? Observacao { get; set; }

        public IFormFile? Imagem { get; set; }  // ficheiro enviado via multipart/form-data que o controller processa e guarda;


        // Peixes é carregada da BD no controller; as restantes vêm de PescaConstants
        public List<SelectListItem> Peixes { get; set; } = new();
        public IReadOnlyList<string> Praias { get; init; } = PescaConstants.Praias;
        public IReadOnlyList<string> TiposPesca { get; init; } = PescaConstants.TiposPesca;
        public IReadOnlyList<string> Mares { get; init; } = PescaConstants.Mares;
    }

    // Usado em Captura/Edit.cshtml; inclui Id para identificar o registo e ImagemPathAtual para preview
    public class CapturaEditViewModel
    {
        public int Id { get; set; }
        public string? Titulo { get; set; }
        public int PeixeId { get; set; }
        public string? Praia { get; set; }
        public string? TipoPesca { get; set; }
        public string? Mare { get; set; }
        public DateTime DataHora { get; set; }
        public int Quantidade { get; set; }
        public decimal? PesoKg { get; set; }
        public string? Observacao { get; set; }
        public string? ImagemPathAtual { get; set; }    // mostrado na View enquanto não há nova imagem
        public IFormFile? NovaImagem { get; set; }      // opcional; substitui ImagemPathAtual se enviado

        public List<SelectListItem> Peixes { get; set; } = new();
        public IReadOnlyList<string> Praias { get; init; } = PescaConstants.Praias;
        public IReadOnlyList<string> TiposPesca { get; init; } = PescaConstants.TiposPesca;
        public IReadOnlyList<string> Mares { get; init; } = PescaConstants.Mares;
    }

    // Usado em Captura/Details.cshtml; agrega dados calculados (likes, permissões) que a View precisa
    public class CapturaDetailViewModel
    {
        public FishCast.Models.Captura Captura { get; set; } = null!;
        public int LikesCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }  // controla o estado do botão de like
        public bool IsOwnCaptura { get; set; }           // controla visibilidade de Editar/Apagar
        public List<ComentarioViewModel> Comentarios { get; set; } = new();
    }

    // Projeção plana de Comentario + dados do utilizador, evitando lazy loading na View
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
