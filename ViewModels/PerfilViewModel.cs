using FishCast.Models;

namespace FishCast.ViewModels
{
    // Agrega dados do utilizador + métricas sociais para Perfil/Index.cshtml
    public class PerfilViewModel
    {
        public ApplicationUser Utilizador { get; set; } = null!;
        public List<Captura> Capturas { get; set; } = new();
        public int TotalCapturas { get; set; }
        public int SeguidoresCount { get; set; }
        public int SeguindoCount { get; set; }
        public bool IsFollowing { get; set; }       // o utilizador atual já segue este perfil?
        public bool IsOwnProfile { get; set; }      // controla visibilidade do botão "Editar Perfil"
    }

    // Formulário de edição de perfil; FotoPerfilAtual mantém a imagem anterior enquanto não há upload novo
    public class PerfilEditViewModel
    {
        public string? Nome { get; set; }
        public string? Bio { get; set; }
        public string? Localidade { get; set; }
        public string? TipoPescaFavorito { get; set; }
        public string? FotoPerfilAtual { get; set; }
        public IFormFile? NovaFotoPerfil { get; set; }

        public List<string> Localidades { get; set; } = new()
        {
            "Cabedelo", "Afife", "Âncora", "Moledo", "Montedor",
            "Vila Praia de Âncora", "Caminha", "Viana do Castelo"
        };

        public List<string> TiposPesca { get; set; } = new()
        {
            "Mar", "Rio", "Surf Casting", "Pesca à Bóia", "Lure"
        };
    }

    // Partilhado entre Perfil/Seguidores e Perfil/Seguindo;
    // IsSeguidoresList distingue o título e o contexto exibido na View
    public class SeguidoresListViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<SeguidorViewModel> Seguidores { get; set; } = new();
        public bool IsSeguidoresList { get; set; }
    }

    // Projeção plana de ApplicationUser para os cartões de seguidores/seguindo
    public class SeguidorViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string? Nome { get; set; }
        public string? FotoPerfil { get; set; }
        public string? Bio { get; set; }
        public string? Localidade { get; set; }
        public DateTime DataSeguimento { get; set; }
        public bool IsFollowingBack { get; set; }   // o utilizador atual segue este utilizador de volta?
    }
}
