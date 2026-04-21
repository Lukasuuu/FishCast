using FishCast.Data;
using FishCast.Models;
using FishCast.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FishCast.Controllers
{
    [Authorize]
    public class CapturaController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CapturaController(AppDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Captura
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var capturas = await _context.Capturas
                .Include(c => c.Peixe)
                .Include(c => c.Likes)
                .Include(c => c.Comentarios)
                .Where(c => c.UtilizadorId == userId)
                .OrderByDescending(c => c.DataHora)
                .ToListAsync();

            return View(capturas);
        }

        // GET: Captura/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var captura = await _context.Capturas
                .Include(c => c.Peixe)
                .Include(c => c.Utilizador)
                .Include(c => c.Likes)
                .ThenInclude(l => l.Utilizador)
                .Include(c => c.Comentarios)
                .ThenInclude(c => c.Utilizador)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (captura == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            var viewModel = new CapturaDetailViewModel
            {
                Captura = captura,
                LikesCount = captura.Likes.Count,
                IsLikedByCurrentUser = currentUserId != null && captura.Likes.Any(l => l.UtilizadorId == currentUserId),
                IsOwnCaptura = captura.UtilizadorId == currentUserId,
                Comentarios = captura.Comentarios.OrderByDescending(c => c.DataComentario).Select(c => new ComentarioViewModel
                {
                    Id = c.Id,
                    Texto = c.Texto,
                    DataComentario = c.DataComentario,
                    NomeUtilizador = c.Utilizador?.Nome ?? c.Utilizador?.UserName ?? "Utilizador",
                    FotoPerfilUtilizador = c.Utilizador?.FotoPerfil ?? "/images/default-avatar.png",
                    UtilizadorId = c.UtilizadorId ?? ""
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Captura/Comentar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Comentar(int capturaId, string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return RedirectToAction(nameof(Details), new { id = capturaId });
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var comentario = new Comentario
            {
                CapturaId = capturaId,
                UtilizadorId = userId,
                Texto = texto.Trim(),
                DataComentario = DateTime.Now
            };

            _context.Comentarios.Add(comentario);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = capturaId });
        }

        // GET: Captura/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new CapturaCreateViewModel
            {
                Peixes = await _context.Peixes.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nome
                }).ToListAsync()
            };

            return View(viewModel);
        }

        // POST: Captura/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CapturaCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);

                // Upload da imagem
                string? imagemPath = null;
                if (viewModel.Imagem != null && viewModel.Imagem.Length > 0)
                {
                    imagemPath = await UploadImagem(viewModel.Imagem, "capturas");
                }

                var captura = new Captura
                {
                    UtilizadorId = userId,
                    Titulo = viewModel.Titulo,
                    PeixeId = viewModel.PeixeId,
                    Local = viewModel.Local,
                    Praia = viewModel.Praia,
                    TipoPesca = viewModel.TipoPesca,
                    Mare = viewModel.Mare,
                    DataHora = viewModel.DataHora,
                    Quantidade = viewModel.Quantidade,
                    PesoKg = viewModel.PesoKg,
                    Observacao = viewModel.Observacao,
                    ImagemPath = imagemPath ?? "/images/default-captura.jpg"
                };

                _context.Capturas.Add(captura);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Repopular dropdowns em caso de erro
            viewModel.Peixes = await _context.Peixes.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Nome
            }).ToListAsync();

            return View(viewModel);
        }

        // GET: Captura/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var captura = await _context.Capturas.FindAsync(id);
            if (captura == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (captura.UtilizadorId != userId) return Unauthorized();

            var viewModel = new CapturaEditViewModel
            {
                Id = captura.Id,
                Titulo = captura.Titulo,
                PeixeId = captura.PeixeId,
                Local = captura.Local,
                Praia = captura.Praia,
                TipoPesca = captura.TipoPesca,
                Mare = captura.Mare,
                DataHora = captura.DataHora,
                Quantidade = captura.Quantidade,
                PesoKg = captura.PesoKg,
                Observacao = captura.Observacao,
                ImagemPathAtual = captura.ImagemPath,
                Peixes = await _context.Peixes.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nome
                }).ToListAsync()
            };

            return View(viewModel);
        }

        // POST: Captura/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CapturaEditViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var captura = await _context.Capturas.FindAsync(id);
                if (captura == null) return NotFound();

                var userId = _userManager.GetUserId(User);
                if (captura.UtilizadorId != userId) return Unauthorized();

                // Upload da nova imagem se fornecida
                if (viewModel.NovaImagem != null && viewModel.NovaImagem.Length > 0)
                {
                    // Apagar imagem antiga se existir
                    if (!string.IsNullOrEmpty(captura.ImagemPath) && !captura.ImagemPath.Contains("default"))
                    {
                        ApagarImagem(captura.ImagemPath);
                    }
                    captura.ImagemPath = await UploadImagem(viewModel.NovaImagem, "capturas");
                }

                captura.Titulo = viewModel.Titulo;
                captura.PeixeId = viewModel.PeixeId;
                captura.Local = viewModel.Local;
                captura.Praia = viewModel.Praia;
                captura.TipoPesca = viewModel.TipoPesca;
                captura.Mare = viewModel.Mare;
                captura.DataHora = viewModel.DataHora;
                captura.Quantidade = viewModel.Quantidade;
                captura.PesoKg = viewModel.PesoKg;
                captura.Observacao = viewModel.Observacao;

                _context.Update(captura);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            viewModel.Peixes = await _context.Peixes.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Nome
            }).ToListAsync();

            return View(viewModel);
        }

        // GET: Captura/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var captura = await _context.Capturas
                .Include(c => c.Peixe)
                .Include(c => c.Utilizador)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (captura == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (captura.UtilizadorId != userId && !User.IsInRole("Admin")) return Unauthorized();

            return View(captura);
        }

        // POST: Captura/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var captura = await _context.Capturas.FindAsync(id);
            if (captura == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (captura.UtilizadorId != userId && !User.IsInRole("Admin")) return Unauthorized();

            // Apagar imagem associada
            if (!string.IsNullOrEmpty(captura.ImagemPath) && !captura.ImagemPath.Contains("default"))
            {
                ApagarImagem(captura.ImagemPath);
            }

            _context.Capturas.Remove(captura);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Captura/LikeToggle/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LikeToggle(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var existingLike = await _context.CapturaLikes
                .FirstOrDefaultAsync(l => l.CapturaId == id && l.UtilizadorId == userId);

            if (existingLike != null)
            {
                _context.CapturaLikes.Remove(existingLike);
            }
            else
            {
                _context.CapturaLikes.Add(new CapturaLike
                {
                    CapturaId = id,
                    UtilizadorId = userId
                });
            }

            await _context.SaveChangesAsync();

            // Redirecionar de volta para a página anterior
            var referer = Request.Headers.Referer.ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task<string?> UploadImagem(IFormFile imagem, string pasta)
        {
            if (imagem == null || imagem.Length == 0) return null;

            // Validar extensão
            var extensao = Path.GetExtension(imagem.FileName).ToLowerInvariant();
            var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            if (!extensoesPermitidas.Contains(extensao)) return null;

            // Validar tamanho (máx 5MB)
            if (imagem.Length > 5 * 1024 * 1024) return null;

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
