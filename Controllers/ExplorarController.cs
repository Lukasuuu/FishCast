using FishCast.Data;
using FishCast.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FishCast.Controllers
{
    // Acessível sem autenticação — funciona como motor de pesquisa/descoberta da plataforma
    public class ExplorarController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Models.ApplicationUser> _userManager;

        public ExplorarController(AppDbContext context, UserManager<Models.ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Explorar?ordenacao=likes&filtroPraia=Cabedelo&filtroEspecie=1&filtroTipoPesca=Mar&termoPesquisa=robalo
        // Todos os filtros são opcionais e cumulativos
        public async Task<IActionResult> Index(
            string? ordenacao,
            string? filtroPraia,
            string? filtroEspecie,
            string? filtroTipoPesca,
            string? termoPesquisa)
        {
            var consulta = _context.Capturas
                .Include(c => c.Peixe)
                .Include(c => c.Utilizador)
                .Include(c => c.Likes)
                .Include(c => c.Comentarios)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filtroPraia))
            {
                consulta = consulta.Where(c => c.Praia == filtroPraia);
            }
            
            if (!string.IsNullOrEmpty(filtroEspecie) && int.TryParse(filtroEspecie, out int especieId))
                {
                    consulta = consulta.Where(c => c.PeixeId == especieId);
                }

            if (!string.IsNullOrEmpty(filtroTipoPesca))
                {
                    consulta = consulta.Where(c => c.TipoPesca == filtroTipoPesca);
                }

            // Pesquisa textual em vários campos; ToLower() para pesquisa case-insensitive no SQL Server
            if (!string.IsNullOrEmpty(termoPesquisa))
            {
                var termo = termoPesquisa.ToLower();
                consulta = consulta.Where(c =>
                    (c.Titulo != null && c.Titulo.ToLower().Contains(termo)) ||
                    (c.Observacao != null && c.Observacao.ToLower().Contains(termo)) ||
                    (c.Praia != null && c.Praia.ToLower().Contains(termo)) ||
                    (c.Peixe != null && c.Peixe.Nome != null && c.Peixe.Nome.ToLower().Contains(termo)));
            }

            ordenacao ??= "recentes";
            consulta = ordenacao switch
            {
                "likes" => consulta.OrderByDescending(c => c.Likes.Count).ThenByDescending(c => c.DataHora),
                "peso"  => consulta.OrderByDescending(c => c.PesoKg ?? 0).ThenByDescending(c => c.DataHora),
                _       => consulta.OrderByDescending(c => c.DataHora)
            };

            var capturas = await consulta.ToListAsync();
            var idUsuarioAtual = _userManager.GetUserId(User);

            return View(new ExplorarViewModel
            {
                Capturas = capturas.Select(c => new CapturaCardViewModel
                {
                    Captura = c,
                    LikesCount = c.Likes.Count,
                    CommentsCount = c.Comentarios.Count,
                    IsLikedByCurrentUser = idUsuarioAtual != null && c.Likes.Any(l => l.UtilizadorId == idUsuarioAtual)
                }
                ).ToList(),
                Ordenacao = ordenacao,
                FiltroPraia = filtroPraia,
                FiltroEspecie = filtroEspecie,
                FiltroTipoPesca = filtroTipoPesca,
                TermoPesquisa = termoPesquisa,
                Praias = await GetPraiasSelectList(),
                Especies = await _context.Peixes
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nome })
                    .ToListAsync(),
                TiposPesca = GetTiposPescaSelectList()
            });
        }

        // Praias dinâmicas: lidas das capturas existentes (não há tabela própria de praias)
        private async Task<List<SelectListItem>> GetPraiasSelectList()
        {
            var praias = await _context.Capturas
                .Where(c => !string.IsNullOrEmpty(c.Praia))
                .Select(c => c.Praia)
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();

            var lista = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Todas as praias" }
            };
            lista.AddRange(praias.Select(p => new SelectListItem { Value = p!, Text = p }));
            return lista;
        }

        private List<SelectListItem> GetTiposPescaSelectList() => new()
        {
            new SelectListItem { Value = "",                Text = "Todos os tipos" },
            new SelectListItem { Value = "Mar",             Text = "Mar" },
            new SelectListItem { Value = "Rio",             Text = "Rio" },
            new SelectListItem { Value = "Surf Casting",    Text = "Surf Casting" },
            new SelectListItem { Value = "Pesca à Bóia",   Text = "Pesca à Bóia" },
            new SelectListItem { Value = "Lure",            Text = "Lure" },
            new SelectListItem { Value = "Pesca Submarina", Text = "Pesca Submarina" },
            new SelectListItem { Value = "Pesca de Cais",  Text = "Pesca de Cais" },
            new SelectListItem { Value = "Pesca de Rocha", Text = "Pesca de Rocha" }
        };
    }
}
