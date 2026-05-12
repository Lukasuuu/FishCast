using FishCast.Data;
using FishCast.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FishCast.Controllers
{
    // Controller de consulta — apenas leitura, sem autenticação necessária
    public class PeixeController : Controller
    {
        private readonly AppDbContext _context;

        public PeixeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Peixe?filtroHabitat=Mar&termoPesquisa=robalo
        public async Task<IActionResult> Index(string? filtroHabitat, string? termoPesquisa)
        {
            var consulta = _context.Peixes.AsQueryable();

            if (!string.IsNullOrEmpty(filtroHabitat))
                consulta = consulta.Where(p => p.HabitatTipo == filtroHabitat);

            if (!string.IsNullOrEmpty(termoPesquisa))
            {
                var termo = termoPesquisa.ToLower();
                consulta = consulta.Where(p =>
                    (p.Nome != null && p.Nome.ToLower().Contains(termo)) ||
                    (p.NomeCientifico != null && p.NomeCientifico.ToLower().Contains(termo)) ||
                    (p.Descricao != null && p.Descricao.ToLower().Contains(termo)));
            }

            return View(new PeixeCatalogoViewModel
            {
                Peixes = await consulta.OrderBy(p => p.Nome).ToListAsync(),
                FiltroHabitat = filtroHabitat,
                TermoPesquisa = termoPesquisa
            });
        }

        // GET: /Peixe/Details/5 — inclui estatísticas agregadas e as últimas 12 capturas da espécie
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var peixe = await _context.Peixes
                .Include(p => p.Capturas).ThenInclude(c => c.Utilizador)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (peixe == null) return NotFound();

            // Projeção direta para CapturaResumoViewModel evita carregar campos desnecessários
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
                    NomeUtilizador = c.Utilizador != null
                        ? (c.Utilizador.Nome ?? c.Utilizador.UserName)
                        : "Utilizador"
                })
                .ToListAsync();

            return View(new PeixeDetailViewModel
            {
                Peixe = peixe,
                Capturas = capturas,
                TotalCapturas = await _context.Capturas.CountAsync(c => c.PeixeId == id),
                MaiorPeso = await _context.Capturas
                    .Where(c => c.PeixeId == id && c.PesoKg != null)
                    .MaxAsync(c => (decimal?)c.PesoKg)
            });
        }
    }
}
