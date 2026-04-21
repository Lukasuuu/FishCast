using FishCast.Data;
using FishCast.Models;
using FishCast.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FishCast.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly int _itensPorPagina = 10;

        public HomeController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int pagina = 1, string? filtroPraia = null, string? filtroEspecie = null)
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

            if (!string.IsNullOrEmpty(filtroEspecie) && int.TryParse(filtroEspecie, out int especieId))
            {
                query = query.Where(c => c.PeixeId == especieId);
            }

            // Ordenar por data descendente
            query = query.OrderByDescending(c => c.DataHora);

            // Paginação
            var totalItens = await query.CountAsync();
            var totalPaginas = (int)Math.Ceiling(totalItens / (double)_itensPorPagina);
            var paginaAtual = Math.Max(1, Math.Min(pagina, Math.Max(1, totalPaginas)));

            var capturas = await query
                .Skip((paginaAtual - 1) * _itensPorPagina)
                .Take(_itensPorPagina)
                .ToListAsync();

            var currentUserId = _userManager.GetUserId(User);

            var viewModel = new FeedViewModel
            {
                Capturas = capturas,
                PaginaAtual = paginaAtual,
                TotalPaginas = totalPaginas,
                FiltroPraia = filtroPraia,
                FiltroEspecie = filtroEspecie,
                Praias = await _context.Capturas
                    .Where(c => !string.IsNullOrEmpty(c.Praia))
                    .Select(c => c.Praia!)
                    .Distinct()
                    .OrderBy(p => p)
                    .ToListAsync(),
                Especies = await _context.Peixes.ToListAsync()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
