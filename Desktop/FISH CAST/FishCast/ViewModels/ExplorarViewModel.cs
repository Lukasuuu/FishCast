using FishCast.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FishCast.ViewModels
{
    // Dados para Explorar/Index.cshtml — filtros, ordenação e resultados paginados como cards
    public class ExplorarViewModel
    {
        public List<CapturaCardViewModel> Capturas { get; set; } = new();

        // Estado atual dos filtros (repopulado via query string para manter seleção após submit)
        public string? Ordenacao { get; set; }
        public string? FiltroPraia { get; set; }
        public string? FiltroEspecie { get; set; }
        public string? FiltroTipoPesca { get; set; }
        public string? TermoPesquisa { get; set; }

        // Listas para popular os <select> nos filtros
        public List<SelectListItem> Praias { get; set; } = new();
        public List<SelectListItem> Especies { get; set; } = new();
        public List<SelectListItem> TiposPesca { get; set; } = new();

        public List<SelectListItem> OpcoesOrdenacao { get; set; } = new()
        {
            new SelectListItem { Value = "recentes", Text = "Mais Recentes" },
            new SelectListItem { Value = "likes",    Text = "Mais Likes" },
            new SelectListItem { Value = "peso",     Text = "Maior Peso" }
        };
    }
}
