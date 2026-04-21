using FishCast.Data;
using FishCast.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FishCast.Controllers
{
    public class ExplorarController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Models.ApplicationUser> _userManager;

        public ExplorarController(AppDbContext context, UserManager<Models.ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Explorar
        public async Task<IActionResult> Index(string? ordenacao, string? filtroPraia, string? filtroEspecie, string? filtroTipoPesca)
        {
            var query = _context.Capturas
                .Include(c => c.Peixe)
                .Include(c => c.Utilizador)
                .Include(c => c.Likes)
                .Include(c => c.Comentarios)
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(filtroPraia))
            {
                query = query.Where(c => c.Praia == filtroPraia);
            }

            if (!string.IsNullOrEmpty(filtroEspecie))
            {
                if (int.TryParse(filtroEspecie, out int especieId))
                {
                    query = query.Where(c => c.PeixeId == especieId);
                }
            }

            if (!string.IsNullOrEmpty(filtroTipoPesca))
            {
                query = query.Where(c => c.TipoPesca == filtroTipoPesca);
            }

            // Ordenação
            ordenacao ??= "recentes";
            query = ordenacao switch
            {
                "likes" => query.OrderByDescending(c => c.Likes.Count).ThenByDescending(c => c.DataHora),
                "peso" => query.OrderByDescending(c => c.PesoKg ?? 0).ThenByDescending(c => c.DataHora),
                _ => query.OrderByDescending(c => c.DataHora)
            };

            var capturas = await query.ToListAsync();
            var currentUserId = _userManager.GetUserId(User);

            var viewModel = new ExplorarViewModel
            {
                Capturas = capturas.Select(c => new CapturaCardViewModel
                {
                    Captura = c,
                    LikesCount = c.Likes.Count,
                    CommentsCount = c.Comentarios.Count,
                    IsLikedByCurrentUser = currentUserId != null && c.Likes.Any(l => l.UtilizadorId == currentUserId)
                }).ToList(),
                Ordenacao = ordenacao,
                FiltroPraia = filtroPraia,
                FiltroEspecie = filtroEspecie,
                FiltroTipoPesca = filtroTipoPesca,
                Praias = await GetPraiasSelectList(),
                Especies = await _context.Peixes.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nome
                }).ToListAsync(),
                TiposPesca = GetTiposPescaSelectList()
            };

            return View(viewModel);
        }

        private async Task<List<SelectListItem>> GetPraiasSelectList()
        {
            var praias = await _context.Capturas
                .Where(c => !string.IsNullOrEmpty(c.Praia))
                .Select(c => c.Praia)
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();

            var result = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Todas as praias" }
            };

            result.AddRange(praias.Select(p => new SelectListItem
            {
                Value = p!,
                Text = p
            }));

            return result;
        }

        private List<SelectListItem> GetTiposPescaSelectList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Todos os tipos" },
                new SelectListItem { Value = "Mar", Text = "Mar" },
                new SelectListItem { Value = "Rio", Text = "Rio" },
                new SelectListItem { Value = "Surf Casting", Text = "Surf Casting" },
                new SelectListItem { Value = "Pesca à Bóia", Text = "Pesca à Bóia" },
                new SelectListItem { Value = "Lure", Text = "Lure" },
                new SelectListItem { Value = "Pesca Submarina", Text = "Pesca Submarina" },
                new SelectListItem { Value = "Pesca de Cais", Text = "Pesca de Cais" },
                new SelectListItem { Value = "Pesca de Rocha", Text = "Pesca de Rocha" }
            };
        }
    }
}
