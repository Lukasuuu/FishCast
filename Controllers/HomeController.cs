using FishCast.Data;
using FishCast.Models;
using FishCast.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FishCast.Controllers
{
    // Controller do feed principal — acessível sem autenticação
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

        // GET: / ou /Home/Index?pagina=2&filtroPraia=Cabedelo&filtroEspecie=1
        public async Task<IActionResult> Index(int pagina = 1, string? filtroPraia = null, string? filtroEspecie = null)
        {
            var consulta = _context.Capturas
                .Include(c => c.Peixe)
                .Include(c => c.Utilizador)
                .Include(c => c.Likes)
                .Include(c => c.Comentarios)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filtroPraia))
                consulta = consulta.Where(c => c.Praia == filtroPraia);

            if (!string.IsNullOrEmpty(filtroEspecie) && int.TryParse(filtroEspecie, out int especieId))
                consulta = consulta.Where(c => c.PeixeId == especieId);

            consulta = consulta.OrderByDescending(c => c.DataHora);

            var totalItens = await consulta.CountAsync();
            var totalPaginas = (int)Math.Ceiling(totalItens / (double)_itensPorPagina);

            // Garante que a página pedida está dentro do intervalo válido
            var paginaAtual = Math.Max(1, Math.Min(pagina, Math.Max(1, totalPaginas)));

            var capturas = await consulta
                .Skip((paginaAtual - 1) * _itensPorPagina)
                .Take(_itensPorPagina)
                .ToListAsync();

            return View(new FeedViewModel
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
            });
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
