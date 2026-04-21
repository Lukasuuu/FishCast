using FishCast.Models;

namespace FishCast.ViewModels
{
    public class PerfilViewModel
    {
        public ApplicationUser Utilizador { get; set; } = null!;
        public List<Captura> Capturas { get; set; } = new List<Captura>();
        public int TotalCapturas { get; set; }
        public int SeguidoresCount { get; set; }
        public int SeguindoCount { get; set; }
        public bool IsFollowing { get; set; }
        public bool IsOwnProfile { get; set; }
    }

    public class PerfilEditViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string? Nome { get; set; }
        public string? Bio { get; set; }
        public string? Localidade { get; set; }
        public string? TipoPescaFavorito { get; set; }
        public string? FotoPerfilAtual { get; set; }
        public IFormFile? NovaFotoPerfil { get; set; }

        public List<string> Localidades { get; set; } = new List<string>
        {
            "Cabedelo", "Afife", "Âncora", "Moledo", "Montedor",
            "Vila Praia de Âncora", "Caminha", "Viana do Castelo"
        };

        public List<string> TiposPesca { get; set; } = new List<string>
        {
            "Mar", "Rio", "Surf Casting", "Pesca à Bóia", "Lure"
        };
    }

    public class SeguidoresListViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<SeguidorViewModel> Seguidores { get; set; } = new List<SeguidorViewModel>();
        public bool IsSeguidoresList { get; set; } // true = seguidores, false = seguindo
    }

    public class SeguidorViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string? Nome { get; set; }
        public string? FotoPerfil { get; set; }
        public string? Bio { get; set; }
        public string? Localidade { get; set; }
        public DateTime DataSeguimento { get; set; }
        public bool IsFollowingBack { get; set; }
    }
}
