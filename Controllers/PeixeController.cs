using FishCast.Data;
using FishCast.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FishCast.Controllers
{
    public class PeixeController : Controller
    {
        private readonly AppDbContext _context;

        public PeixeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Peixe
        public async Task<IActionResult> Index(string? filtroHabitat, string? termoPesquisa)
        {
            var query = _context.Peixes.AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(filtroHabitat))
            {
                query = query.Where(p => p.HabitatTipo == filtroHabitat);
            }

            if (!string.IsNullOrEmpty(termoPesquisa))
            {
                var termo = termoPesquisa.ToLower();
                query = query.Where(p =>
                    (p.Nome != null && p.Nome.ToLower().Contains(termo)) ||
                    (p.NomeCientifico != null && p.NomeCientifico.ToLower().Contains(termo)) ||
                    (p.Descricao != null && p.Descricao.ToLower().Contains(termo)));
            }

            var peixes = await query.OrderBy(p => p.Nome).ToListAsync();

            var viewModel = new PeixeCatalogoViewModel
            {
                Peixes = peixes,
                FiltroHabitat = filtroHabitat,
                TermoPesquisa = termoPesquisa
            };

            return View(viewModel);
        }

        // GET: Peixe/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var peixe = await _context.Peixes
                .Include(p => p.Capturas)
                .ThenInclude(c => c.Utilizador)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (peixe == null) return NotFound();

            // Buscar capturas com detalhes
            var capturas = await _context.Capturas
                .Where(c => c.PeixeId == id)
                .Include(c => c.Utilizador)
                .OrderByDescending(c => c.DataHora)
                .Take(12)
                .Select(c => new CapturaResumoViewModel
                {
                    Id = c.Id,
                    Titulo = c.Titulo,
                    PesoKg = c.PesoKg,
                    DataHora = c.DataHora,
                    ImagemPath = c.ImagemPath,
                    NomeUtilizador = c.Utilizador != null ? (c.Utilizador.Nome ?? c.Utilizador.UserName) : "Utilizador",
                    Local = c.Local
                })
                .ToListAsync();

            // Calcular maior peso
            var maiorPeso = await _context.Capturas
                .Where(c => c.PeixeId == id && c.PesoKg != null)
                .MaxAsync(c => (decimal?)c.PesoKg);

            var viewModel = new PeixeDetailViewModel
            {
                Peixe = peixe,
                Capturas = capturas,
                TotalCapturas = await _context.Capturas.CountAsync(c => c.PeixeId == id),
                MaiorPeso = maiorPeso
            };

            return View(viewModel);
        }
    }
}
