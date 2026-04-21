using FishCast.Data;
using FishCast.Models;
using FishCast.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FishCast.Controllers
{
    public class PerfilController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PerfilController(AppDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Perfil/Index?userId=...
        public async Task<IActionResult> Index(string? userId)
        {
            // Se não foi especificado userId, mostrar perfil do utilizador atual
            var currentUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                userId = currentUserId;
            }

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var utilizador = await _userManager.Users
                .Include(u => u.Capturas)
                .ThenInclude(c => c.Peixe)
                .Include(u => u.Seguidores)
                .Include(u => u.Seguidos)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (utilizador == null) return NotFound();

            // Buscar capturas ordenadas
            var capturas = await _context.Capturas
                .Where(c => c.UtilizadorId == userId)
                .Include(c => c.Peixe)
                .OrderByDescending(c => c.DataHora)
                .ToListAsync();

            // Verificar se o utilizador atual segue este perfil
            bool isFollowing = false;
            if (currentUserId != null && currentUserId != userId)
            {
                isFollowing = await _context.Seguidores
                    .AnyAsync(s => s.SeguidorId == currentUserId && s.SeguidoId == userId);
            }

            var viewModel = new PerfilViewModel
            {
                Utilizador = utilizador,
                Capturas = capturas,
                TotalCapturas = capturas.Count,
                SeguidoresCount = utilizador.Seguidores.Count,
                SeguindoCount = utilizador.Seguidos.Count,
                IsFollowing = isFollowing,
                IsOwnProfile = currentUserId == userId
            };

            return View(viewModel);
        }

        // GET: Perfil/Editar
        [Authorize]
        public async Task<IActionResult> Editar()
        {
            var userId = _userManager.GetUserId(User);
            var utilizador = await _userManager.FindByIdAsync(userId);

            if (utilizador == null) return NotFound();

            var viewModel = new PerfilEditViewModel
            {
                Id = utilizador.Id,
                Nome = utilizador.Nome,
                Bio = utilizador.Bio,
                Localidade = utilizador.Localidade,
                TipoPescaFavorito = utilizador.TipoPescaFavorito,
                FotoPerfilAtual = utilizador.FotoPerfil
            };

            return View(viewModel);
        }

        // POST: Perfil/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Editar(PerfilEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var utilizador = await _userManager.FindByIdAsync(userId);

                if (utilizador == null) return NotFound();

                // Upload da nova foto de perfil se fornecida
                if (viewModel.NovaFotoPerfil != null && viewModel.NovaFotoPerfil.Length > 0)
                {
                    // Validar tamanho (máx 5MB)
                    if (viewModel.NovaFotoPerfil.Length <= 5 * 1024 * 1024)
                    {
                        var extensao = Path.GetExtension(viewModel.NovaFotoPerfil.FileName).ToLowerInvariant();
                        var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

                        if (extensoesPermitidas.Contains(extensao))
                        {
                            // Apagar foto antiga se existir
                            if (!string.IsNullOrEmpty(utilizador.FotoPerfil) && !utilizador.FotoPerfil.Contains("default"))
                            {
                                ApagarImagem(utilizador.FotoPerfil);
                            }

                            utilizador.FotoPerfil = await UploadImagem(viewModel.NovaFotoPerfil, "perfis");
                        }
                    }
                }

                utilizador.Nome = viewModel.Nome;
                utilizador.Bio = viewModel.Bio;
                utilizador.Localidade = viewModel.Localidade;
                utilizador.TipoPescaFavorito = viewModel.TipoPescaFavorito;

                await _userManager.UpdateAsync(utilizador);

                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        // POST: Perfil/Seguir
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Seguir(string userId)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null || currentUserId == userId)
            {
                return RedirectToAction(nameof(Index), new { userId });
            }

            var existingSeguidor = await _context.Seguidores
                .FirstOrDefaultAsync(s => s.SeguidorId == currentUserId && s.SeguidoId == userId);

            if (existingSeguidor != null)
            {
                // Desseguir
                _context.Seguidores.Remove(existingSeguidor);
            }
            else
            {
                // Seguir
                _context.Seguidores.Add(new Seguidor
                {
                    SeguidorId = currentUserId,
                    SeguidoId = userId,
                    DataSeguimento = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            // Redirecionar de volta para a página anterior
            var referer = Request.Headers.Referer.ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction(nameof(Index), new { userId });
        }

        // GET: Perfil/Seguidores
        public async Task<IActionResult> Seguidores(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = _userManager.GetUserId(User);
            }

            if (string.IsNullOrEmpty(userId)) return NotFound();

            var utilizador = await _userManager.FindByIdAsync(userId);
            if (utilizador == null) return NotFound();

            var seguidores = await _context.Seguidores
                .Where(s => s.SeguidoId == userId)
                .Include(s => s.Seguidor_Nav)
                .OrderByDescending(s => s.DataSeguimento)
                .ToListAsync();

            var currentUserId = _userManager.GetUserId(User);

            var viewModel = new SeguidoresListViewModel
            {
                UserId = userId,
                UserName = utilizador.Nome ?? utilizador.UserName ?? "Utilizador",
                IsSeguidoresList = true,
                Seguidores = seguidores.Select(s => new SeguidorViewModel
                {
                    Id = s.Seguidor_Nav!.Id,
                    Nome = s.Seguidor_Nav.Nome ?? s.Seguidor_Nav.UserName ?? "Utilizador",
                    FotoPerfil = s.Seguidor_Nav.FotoPerfil ?? "/images/default-avatar.png",
                    Bio = s.Seguidor_Nav.Bio,
                    Localidade = s.Seguidor_Nav.Localidade,
                    DataSeguimento = s.DataSeguimento,
                    IsFollowingBack = currentUserId != null &&
                        _context.Seguidores.Any(sf => sf.SeguidorId == currentUserId && sf.SeguidoId == s.SeguidorId)
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: Perfil/Seguindo
        public async Task<IActionResult> Seguindo(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = _userManager.GetUserId(User);
            }

            if (string.IsNullOrEmpty(userId)) return NotFound();

            var utilizador = await _userManager.FindByIdAsync(userId);
            if (utilizador == null) return NotFound();

            var seguindo = await _context.Seguidores
                .Where(s => s.SeguidorId == userId)
                .Include(s => s.Seguido_Nav)
                .OrderByDescending(s => s.DataSeguimento)
                .ToListAsync();

            var currentUserId = _userManager.GetUserId(User);

            var viewModel = new SeguidoresListViewModel
            {
                UserId = userId,
                UserName = utilizador.Nome ?? utilizador.UserName ?? "Utilizador",
                IsSeguidoresList = false,
                Seguidores = seguindo.Select(s => new SeguidorViewModel
                {
                    Id = s.Seguido_Nav!.Id,
                    Nome = s.Seguido_Nav.Nome ?? s.Seguido_Nav.UserName ?? "Utilizador",
                    FotoPerfil = s.Seguido_Nav.FotoPerfil ?? "/images/default-avatar.png",
                    Bio = s.Seguido_Nav.Bio,
                    Localidade = s.Seguido_Nav.Localidade,
                    DataSeguimento = s.DataSeguimento,
                    IsFollowingBack = true // Já segue por definição
                }).ToList()
            };

            return View(viewModel);
        }

        private async Task<string?> UploadImagem(IFormFile imagem, string pasta)
        {
            if (imagem == null || imagem.Length == 0) return null;

            var extensao = Path.GetExtension(imagem.FileName).ToLowerInvariant();
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", pasta);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var nomeUnico = $"{Guid.NewGuid()}{extensao}";
            var caminhoCompleto = Path.Combine(uploadsFolder, nomeUnico);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await imagem.CopyToAsync(stream);
            }

            return $"/uploads/{pasta}/{nomeUnico}";
        }

        private void ApagarImagem(string? imagemPath)
        {
            if (string.IsNullOrEmpty(imagemPath)) return;

            var caminhoCompleto = Path.Combine(_webHostEnvironment.WebRootPath, imagemPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(caminhoCompleto))
            {
                System.IO.File.Delete(caminhoCompleto);
            }
        }
    }
}
