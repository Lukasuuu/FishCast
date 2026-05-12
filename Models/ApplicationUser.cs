using Microsoft.AspNetCore.Identity;

namespace FishCast.Models
{
    // Estende IdentityUser para adicionar campos de perfil específicos da aplicação.
    // O login pode ser feito por UserName ou Email (configurado no Identity).
    public class ApplicationUser : IdentityUser
    {
        public string? Nome { get; set; }
        public string? FotoPerfil { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public string? Bio { get; set; }
        public string? Localidade { get; set; }
        public string? TipoPescaFavorito { get; set; }

        // Navegação para as relações sociais.
        // "Seguidos" = utilizadores que este utilizador segue (este é o SeguidorId).
        // "Seguidores" = utilizadores que seguem este utilizador (este é o SeguidoId).
        public ICollection<Seguidor> Seguidores { get; set; } = new List<Seguidor>();
        public ICollection<Seguidor> Seguidos { get; set; } = new List<Seguidor>();

        public ICollection<CapturaLike> Likes { get; set; } = new List<CapturaLike>();
        public ICollection<Captura> Capturas { get; set; } = new List<Captura>();
    }
}
