using FishCast.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FishCast.ViewModels
{
    public class ExplorarViewModel
    {
        public List<CapturaCardViewModel> Capturas { get; set; } = new List<CapturaCardViewModel>();
        public string? Ordenacao { get; set; }
        public string? FiltroPraia { get; set; }
        public string? FiltroEspecie { get; set; }
        public string? FiltroTipoPesca { get; set; }

        public List<SelectListItem> Praias { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Especies { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> TiposPesca { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> OpcoesOrdenacao { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "recentes", Text = "Mais Recentes" },
            new SelectListItem { Value = "likes", Text = "Mais Likes" },
            new SelectListItem { Value = "peso", Text = "Maior Peso" }
        };
    }
}
