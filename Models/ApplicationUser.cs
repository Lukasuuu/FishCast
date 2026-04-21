using Microsoft.AspNetCore.Identity;

namespace FishCast.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Nome do utilizador (vai aparecer no feed)
        public string? Nome { get; set; }

        // Caminho da imagem de perfil
        public string? FotoPerfil { get; set; }

        // Data de criação da conta
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        // Bio - descrição do pescador
        public string? Bio { get; set; }

        // Localidade - dropdown: "Cabedelo", "Afife", "Âncora", "Moledo", "Montedor", "Vila Praia de Âncora", "Caminha", "Viana do Castelo"
        public string? Localidade { get; set; }

        // Tipo de pesca favorito
        public string? TipoPescaFavorito { get; set; }

        // Relacionamentos de seguidores
        public ICollection<Seguidor> Seguidores { get; set; } = new List<Seguidor>();
        public ICollection<Seguidor> Seguidos { get; set; } = new List<Seguidor>();

        // Likes dados pelo utilizador
        public ICollection<CapturaLike> Likes { get; set; } = new List<CapturaLike>();

        // Capturas do utilizador
        public ICollection<Captura> Capturas { get; set; } = new List<Captura>();
    }
}
