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
        private readonly IWebHostEnvironment _webHostEnvironment; // Para salvar/excluir imagens no disco e evitar expor lógica de arquivos na View e manter o controller responsável por isso.

        public PerfilController(AppDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /Perfil ou /Perfil/Index?userId=xxx
        // Sem userId → mostra o perfil do usuário autenticado; sem sessão → redireciona para login
        public async Task<IActionResult> Index(string? userId)
        {
            var idUsuarioAtual = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
                userId = idUsuarioAtual;

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account", new { area = "Identity" });

            var usuario = await _userManager.Users
                .Include(u => u.Capturas).ThenInclude(c => c.Peixe)
                .Include(u => u.Seguidores)
                .Include(u => u.Seguidos)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (usuario == null) return NotFound();

            // Segunda query para obter dados sociais (likes, comentários) que UserManager não carrega
            var capturas = await _context.Capturas
                .Where(c => c.UtilizadorId == userId)
                .Include(c => c.Peixe)
                .Include(c => c.Likes)
                .Include(c => c.Comentarios)
                .OrderByDescending(c => c.DataHora)
                .ToListAsync();

            var estaSeguindo = idUsuarioAtual != null && idUsuarioAtual != userId &&
                await _context.Seguidores.AnyAsync(s => s.SeguidorId == idUsuarioAtual && s.SeguidoId == userId);

            return View(new PerfilViewModel
            {
                Utilizador = usuario,
                Capturas = capturas,
                TotalCapturas = capturas.Count,
                SeguidoresCount = usuario.Seguidores.Count,
                SeguindoCount = usuario.Seguidos.Count,
                IsFollowing = estaSeguindo,
                IsOwnProfile = idUsuarioAtual == userId
            });
        }

        // GET: /Perfil/Editar — preenche o formulário com os dados atuais do usuário autenticado
        [Authorize]
        public async Task<IActionResult> Editar()
        {
            var idUsuario = _userManager.GetUserId(User);
            var usuario = await _userManager.FindByIdAsync(idUsuario);
            if (usuario == null) return NotFound();

            return View(new PerfilEditViewModel
            {
                Nome = usuario.Nome,
                Bio = usuario.Bio,
                Localidade = usuario.Localidade,
                TipoPescaFavorito = usuario.TipoPescaFavorito,
                FotoPerfilAtual = usuario.FotoPerfil
            });
        }

        // POST: /Perfil/Editar — idUsuario vem da sessão, nunca do formulário (evita alteração de outro perfil)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Editar(PerfilEditViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var idUsuario = _userManager.GetUserId(User);
            var usuario = await _userManager.FindByIdAsync(idUsuario);
            if (usuario == null) return NotFound();

            // Upload de nova foto: valida extensão e tamanho antes de substituir a existente
            if (viewModel.NovaFotoPerfil != null && viewModel.NovaFotoPerfil.Length > 0 &&
                viewModel.NovaFotoPerfil.Length <= 5 * 1024 * 1024)
            {
                var extensao = Path.GetExtension(viewModel.NovaFotoPerfil.FileName).ToLowerInvariant();
                var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

                if (extensoesPermitidas.Contains(extensao))
                {
                    if (!string.IsNullOrEmpty(usuario.FotoPerfil) && !usuario.FotoPerfil.Contains("default"))
                        ExcluirImagem(usuario.FotoPerfil);

                    usuario.FotoPerfil = await SalvarImagem(viewModel.NovaFotoPerfil, "perfis");
                }
            }

            usuario.Nome = viewModel.Nome;
            usuario.Bio = viewModel.Bio;
            usuario.Localidade = viewModel.Localidade;
            usuario.TipoPescaFavorito = viewModel.TipoPescaFavorito;

            await _userManager.UpdateAsync(usuario);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Perfil/Seguir — toggle de seguimento: segue se não seguia, deixa de seguir se já seguia.
        // Redireciona para o Referer para funcionar em qualquer página (perfil, feed, explorar).
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Seguir(string userId)
        {
            var idUsuarioAtual = _userManager.GetUserId(User);

            if (idUsuarioAtual == null || idUsuarioAtual == userId)
                return RedirectToAction(nameof(Index), new { userId });

            var seguidorExistente = await _context.Seguidores
                .FirstOrDefaultAsync(s => s.SeguidorId == idUsuarioAtual && s.SeguidoId == userId);

            if (seguidorExistente != null)
                _context.Seguidores.Remove(seguidorExistente);
            else
                _context.Seguidores.Add(new Seguidor
                {
                    SeguidorId = idUsuarioAtual,
                    SeguidoId = userId,
                    DataSeguimento = DateTime.Now
                });

            await _context.SaveChangesAsync();

            var urlAnterior = Request.Headers.Referer.ToString();
            return !string.IsNullOrEmpty(urlAnterior)
                ? Redirect(urlAnterior)
                : RedirectToAction(nameof(Index), new { userId });
        }

        // GET: /Perfil/Seguidores?userId=xxx — lista quem segue este usuário
        public async Task<IActionResult> Seguidores(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId)) return NotFound();

            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null) return NotFound();

            var idUsuarioAtual = _userManager.GetUserId(User);

            var seguidores = await _context.Seguidores
                .Where(s => s.SeguidoId == userId)
                .Include(s => s.Seguidor_Nav)
                .OrderByDescending(s => s.DataSeguimento)
                .ToListAsync();

            return View(new SeguidoresListViewModel
            {
                UserId = userId,
                UserName = usuario.Nome ?? usuario.UserName ?? "Utilizador",
                IsSeguidoresList = true,
                Seguidores = seguidores.Select(s => new SeguidorViewModel
                {
                    Id = s.Seguidor_Nav!.Id,
                    Nome = s.Seguidor_Nav.Nome ?? s.Seguidor_Nav.UserName ?? "Utilizador",
                    FotoPerfil = s.Seguidor_Nav.FotoPerfil ?? "/images/default-avatar.svg",
                    Bio = s.Seguidor_Nav.Bio,
                    Localidade = s.Seguidor_Nav.Localidade,
                    DataSeguimento = s.DataSeguimento,
                    // Verifica se o usuário atual segue de volta cada seguidor
                    IsFollowingBack = idUsuarioAtual != null &&
                        _context.Seguidores.Any(sf => sf.SeguidorId == idUsuarioAtual && sf.SeguidoId == s.SeguidorId)
                }).ToList()
            });
        }

        // GET: /Perfil/Seguindo?userId=xxx — lista quem este usuário segue
        public async Task<IActionResult> Seguindo(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId)) return NotFound();

            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null) return NotFound();

            var seguindo = await _context.Seguidores
                .Where(s => s.SeguidorId == userId)
                .Include(s => s.Seguido_Nav)
                .OrderByDescending(s => s.DataSeguimento)
                .ToListAsync();

            return View(new SeguidoresListViewModel
            {
                UserId = userId,
                UserName = usuario.Nome ?? usuario.UserName ?? "Utilizador",
                IsSeguidoresList = false,
                Seguidores = seguindo.Select(s => new SeguidorViewModel
                {
                    Id = s.Seguido_Nav!.Id,
                    Nome = s.Seguido_Nav.Nome ?? s.Seguido_Nav.UserName ?? "Utilizador",
                    FotoPerfil = s.Seguido_Nav.FotoPerfil ?? "/images/default-avatar.svg",
                    Bio = s.Seguido_Nav.Bio,
                    Localidade = s.Seguido_Nav.Localidade,
                    DataSeguimento = s.DataSeguimento,
                    IsFollowingBack = true  // se aparece nesta lista, o usuário já segue
                }).ToList()
            });
        }

        private async Task<string?> SalvarImagem(IFormFile imagem, string pasta)
        {
            if (imagem == null || imagem.Length == 0) return null;

            var extensao = Path.GetExtension(imagem.FileName).ToLowerInvariant();
            var pastaUploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", pasta);

            if (!Directory.Exists(pastaUploads))
                Directory.CreateDirectory(pastaUploads);

            var nomeUnico = $"{Guid.NewGuid()}{extensao}";
            var caminhoCompleto = Path.Combine(pastaUploads, nomeUnico);

            using var fluxo = new FileStream(caminhoCompleto, FileMode.Create);
            await imagem.CopyToAsync(fluxo);

            return $"/uploads/{pasta}/{nomeUnico}";
        }

        private void ExcluirImagem(string? caminho)
        {
            if (string.IsNullOrEmpty(caminho)) return;

            var caminhoCompleto = Path.Combine(
                _webHostEnvironment.WebRootPath,
                caminho.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (System.IO.File.Exists(caminhoCompleto))
                System.IO.File.Delete(caminhoCompleto);
        }
    }
}
