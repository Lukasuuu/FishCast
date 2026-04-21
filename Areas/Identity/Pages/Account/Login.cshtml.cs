using FishCast.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace FishCast.Areas.Identity.Pages.Account
{
    public class LoginModel(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<LoginModel> logger) : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<LoginModel> _logger = logger;

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O nome de usuário ou email é obrigatório.")]
            [Display(Name = "Nome de usuário ou Email")]
            public string? UsernameOrEmail { get; set; }

            [Required(ErrorMessage = "A senha é obrigatória.")]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string? Password { get; set; }

            [Display(Name = "Lembrar-me?")]
            public bool RememberMe { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                ApplicationUser user = null;
                var input = Input.UsernameOrEmail.Trim();

                // Tentar encontrar o usuário por email ou username
                if (input.Contains("@"))
                {
                    // Tentar primeiro por email
                    user = await _userManager.FindByEmailAsync(input);

                    // Se não encontrou por email, tentar por username (caso alguém tenha @ no username)
                    if (user == null)
                    {
                        user = await _userManager.FindByNameAsync(input);
                    }
                }
                else
                {
                    // Sem @, tentar por username diretamente
                    user = await _userManager.FindByNameAsync(input);
                }

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Nome de utilizador/email ou password incorretos.");
                    return Page();
                }

                // Usar o UserName normalizado do Identity para login
                var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuário conectado com sucesso.");
                    return LocalRedirect(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Conta de usuário bloqueada.");
                    return RedirectToPage("./Lockout");
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Nome de utilizador/email ou password incorretos.");
                    return Page();
                }
            }

            return Page();
        }
    }
}
