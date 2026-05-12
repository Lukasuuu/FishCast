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
    // Todas as actions requerem autenticação por defeito; Details e Comentar têm exceções abaixo
    [Authorize]
    public class CapturaController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment; // para salvar imagens no disco e guardar o caminho na BD em vez de usar um serviço externo e evitar custos adicionais

        public CapturaController(AppDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /Captura — lista as capturas do utilizador autenticado
        public async Task<IActionResult> Index()
        {
            var idUsuario = _userManager.GetUserId(User);
            var capturas = await _context.Capturas
                .Include(c => c.Peixe)
                .Include(c => c.Likes)
                .Include(c => c.Comentarios)
                .Where(c => c.UtilizadorId == idUsuario)
                .OrderByDescending(c => c.DataHora)
                .ToListAsync();

            return View(capturas);
        }

        // GET: /Captura/Details/5 — acessível sem login (AllowAnonymous sobrepõe [Authorize] da classe)
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var captura = await _context.Capturas
                .Include(c => c.Peixe)
                .Include(c => c.Utilizador)
                .Include(c => c.Likes).ThenInclude(l => l.Utilizador)
                .Include(c => c.Comentarios).ThenInclude(c => c.Utilizador)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (captura == null) return NotFound();

            var idUsuarioAtual = _userManager.GetUserId(User);

            return View(new CapturaDetailViewModel
            {
                Captura = captura,
                LikesCount = captura.Likes.Count,
                IsLikedByCurrentUser = idUsuarioAtual != null && captura.Likes.Any(l => l.UtilizadorId == idUsuarioAtual),
                IsOwnCaptura = captura.UtilizadorId == idUsuarioAtual,
                Comentarios = captura.Comentarios
                    .OrderByDescending(c => c.DataComentario)
                    .Select(c => new ComentarioViewModel
                    {
                        Id = c.Id,
                        Texto = c.Texto,
                        DataComentario = c.DataComentario,
                        NomeUtilizador = c.Utilizador?.Nome ?? c.Utilizador?.UserName ?? "Utilizador",
                        FotoPerfilUtilizador = c.Utilizador?.FotoPerfil ?? "/images/default-avatar.svg",
                        UtilizadorId = c.UtilizadorId ?? ""
                    }).ToList()
            });
        }

        // POST: /Captura/Comentar — recebe o texto diretamente como parâmetro (sem ViewModel intermédio)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Comentar(int capturaId, string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return RedirectToAction(nameof(Details), new { id = capturaId });

            var idUsuario = _userManager.GetUserId(User);
            if (idUsuario == null) return Unauthorized();

            _context.Comentarios.Add(new Comentario
            {
                CapturaId = capturaId,
                UtilizadorId = idUsuario,
                Texto = texto.Trim(),
                DataComentario = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = capturaId });
        }

        // GET: /Captura/Create — preenche as listas de seleção para o formulário
        public async Task<IActionResult> Create()
        {
            return View(new CapturaCreateViewModel
            {
                Peixes = await _context.Peixes
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nome })
                    .ToListAsync()
            });
        }

        // POST: /Captura/Create — faz upload da imagem antes de guardar o registo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CapturaCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Peixes = await _context.Peixes
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nome })
                    .ToListAsync();
                return View(viewModel);
            }

            var idUsuario = _userManager.GetUserId(User);
            var caminhoImagem = viewModel.Imagem != null && viewModel.Imagem.Length > 0
                ? await SalvarImagem(viewModel.Imagem, "capturas")
                : null;

            _context.Capturas.Add(new Captura
            {
                UtilizadorId = idUsuario,
                Titulo = viewModel.Titulo,
                PeixeId = viewModel.PeixeId,
                Praia = viewModel.Praia,
                TipoPesca = viewModel.TipoPesca,
                Mare = viewModel.Mare,
                DataHora = viewModel.DataHora,
                Quantidade = viewModel.Quantidade,
                PesoKg = viewModel.PesoKg,
                Observacao = viewModel.Observacao,
                ImagemPath = caminhoImagem ?? "/images/default-captura.svg"
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Captura/Edit/5 — só o dono pode editar (verificação por idUsuario)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var captura = await _context.Capturas.FindAsync(id);
            if (captura == null) return NotFound();

            var idUsuario = _userManager.GetUserId(User);
            if (captura.UtilizadorId != idUsuario) return Unauthorized();

            var peixes = await _context.Peixes
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nome })
                .ToListAsync();

            var viewModel = new CapturaEditViewModel
            {
                Id = captura.Id,
                Titulo = captura.Titulo,
                PeixeId = captura.PeixeId,
                Praia = captura.Praia,
                TipoPesca = captura.TipoPesca,
                Mare = captura.Mare,
                DataHora = captura.DataHora,
                Quantidade = captura.Quantidade,
                PesoKg = captura.PesoKg,
                Observacao = captura.Observacao,
                ImagemPathAtual = captura.ImagemPath,
                Peixes = peixes
            };

            return View(viewModel);
        }

        // POST: /Captura/Edit/5 — exclui a imagem antiga do disco antes de salvar a nova
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CapturaEditViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                viewModel.Peixes = await _context.Peixes
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nome })
                    .ToListAsync();
                return View(viewModel);
            }

            var captura = await _context.Capturas.FindAsync(id);
            if (captura == null) return NotFound();

            var idUsuario = _userManager.GetUserId(User);
            if (captura.UtilizadorId != idUsuario) return Unauthorized();

            if (viewModel.NovaImagem != null && viewModel.NovaImagem.Length > 0)
            {
                if (!string.IsNullOrEmpty(captura.ImagemPath) && !captura.ImagemPath.Contains("default"))
                    ExcluirImagem(captura.ImagemPath);

                captura.ImagemPath = await SalvarImagem(viewModel.NovaImagem, "capturas");
            }

            captura.Titulo = viewModel.Titulo;
            captura.PeixeId = viewModel.PeixeId;
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

        // GET: /Captura/Delete/5 — admins também podem excluir (moderação de conteúdo)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var captura = await _context.Capturas
                .Include(c => c.Peixe)
                .Include(c => c.Utilizador)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (captura == null) return NotFound();

            var idUsuario = _userManager.GetUserId(User);
            if (captura.UtilizadorId != idUsuario && !User.IsInRole("Admin")) return Unauthorized();

            return View(captura);
        }

        // POST: /Captura/Delete/5 — exclui o arquivo de imagem do disco após remover o registo da BD
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var captura = await _context.Capturas.FindAsync(id);
            if (captura == null) return NotFound();

            var idUsuario = _userManager.GetUserId(User);
            if (captura.UtilizadorId != idUsuario && !User.IsInRole("Admin")) return Unauthorized();

            if (!string.IsNullOrEmpty(captura.ImagemPath) && !captura.ImagemPath.Contains("default"))
                ExcluirImagem(captura.ImagemPath);

            _context.Capturas.Remove(captura);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /Captura/LikeToggle/5 — toggle: adiciona ou remove a curtida do usuário atual.
        // Redireciona para o Referer para funcionar tanto no feed como na página de detalhe.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LikeToggle(int id)
        {
            var idUsuario = _userManager.GetUserId(User);
            if (idUsuario == null) return Unauthorized();

            var curtidaExistente = await _context.CapturaLikes
                .FirstOrDefaultAsync(l => l.CapturaId == id && l.UtilizadorId == idUsuario);

            if (curtidaExistente != null)
                _context.CapturaLikes.Remove(curtidaExistente);
            else
                _context.CapturaLikes.Add(new CapturaLike { CapturaId = id, UtilizadorId = idUsuario });

            await _context.SaveChangesAsync();

            var urlAnterior = Request.Headers.Referer.ToString();
            return !string.IsNullOrEmpty(urlAnterior)
                ? Redirect(urlAnterior)
                : RedirectToAction(nameof(Details), new { id });
        }

        // Salva o arquivo em wwwroot/uploads/{pasta}/ e retorna o caminho relativo para a BD.
        // Valida extensão e tamanho (máx. 5 MB) antes de escrever no disco.
        private async Task<string?> SalvarImagem(IFormFile imagem, string pasta)
        {
            if (imagem == null || imagem.Length == 0) return null;

            var extensao = Path.GetExtension(imagem.FileName).ToLowerInvariant();
            var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            if (!extensoesPermitidas.Contains(extensao) || imagem.Length > 5 * 1024 * 1024) return null;

            var pastaUploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", pasta);
            if (!Directory.Exists(pastaUploads))
                Directory.CreateDirectory(pastaUploads);

            var nomeUnico = $"{Guid.NewGuid()}{extensao}";
            var caminhoCompleto = Path.Combine(pastaUploads, nomeUnico);

            using var fluxo = new FileStream(caminhoCompleto, FileMode.Create);
            await imagem.CopyToAsync(fluxo);

            return $"/uploads/{pasta}/{nomeUnico}";
        }

        // Remove o arquivo físico; ignora se o caminho for nulo ou já não existir
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
