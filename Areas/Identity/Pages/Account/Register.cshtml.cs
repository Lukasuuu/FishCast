// Importações necessárias para o funcionamento da página de registro
    using FishCast.Models;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.WebUtilities;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Text.Encodings.Web;

    namespace FishCast.Areas.Identity.Pages.Account
    {
        // Classe que representa o modelo da página de registro
        public class RegisterModel : PageModel
        {
            // Gerenciador de autenticação de usuários
            private readonly SignInManager<ApplicationUser> _signInManager;
            // Gerenciador de usuários do Identity
            private readonly UserManager<ApplicationUser> _userManager;
            // Armazenamento de usuários
            private readonly IUserStore<ApplicationUser> _userStore;
            // Armazenamento de e-mails de usuários
            private readonly IUserEmailStore<ApplicationUser> _emailStore;
            // Logger para registrar eventos
            private readonly ILogger<RegisterModel> _logger;
            // Serviço de envio de e-mails
            private readonly IEmailSender _emailSender;

        // Construtor que recebe todas as dependências necessárias
        public RegisterModel(
                UserManager<ApplicationUser> userManager,
                IUserStore<ApplicationUser> userStore,
                SignInManager<ApplicationUser> signInManager,
                ILogger<RegisterModel> logger,
                IEmailSender emailSender)
            {
                // Inicializa o gerenciador de usuários
                _userManager = userManager;
                // Inicializa o armazenamento de usuários
                _userStore = userStore;
                // Obtém o armazenamento de e-mails
                _emailStore = GetEmailStore();
                // Inicializa o gerenciador de autenticação
                _signInManager = signInManager;
                // Inicializa o logger
                _logger = logger;
                // Inicializa o serviço de e-mail
                _emailSender = emailSender;
            }

            // Propriedade que vincula os dados do formulário ao modelo
            [BindProperty]
            public InputModel Input { get; set; }

            // URL para redirecionamento após o registro
            public string ReturnUrl { get; set; }

            // Lista de provedores de autenticação externa (Google, Facebook, etc.)
            public IList<AuthenticationScheme> ExternalLogins { get; set; }

            // Classe que define os campos do formulário de registro
            public class InputModel
            {
                // Campo de nome - obrigatório
                [Required(ErrorMessage = "O nome é obrigatório.")]
                [Display(Name = "Nome")]
                public string Nome { get; set; }

                // Campo de e-mail - obrigatório
                [Required(ErrorMessage = "O e-mail é obrigatório.")]
                [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
                [Display(Name = "E-mail")]
                public string Email { get; set; }

                // Campo de senha - obrigatório, com validação de tamanho
                [Required(ErrorMessage = "A senha é obrigatória.")]
                [StringLength(100, ErrorMessage = "A {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
                [DataType(DataType.Password)]
                [Display(Name = "Senha")]
                public string Password { get; set; }

                // Campo de confirmação de senha
                [DataType(DataType.Password)]
                [Display(Name = "Confirmar senha")]
                [Compare("Password", ErrorMessage = "A senha e a confirmação de senha não coincidem.")]
                public string ConfirmPassword { get; set; }
            }

            // Método executado quando a página é carregada (requisição GET)
            public async Task OnGetAsync(string returnUrl = null)
            {
                // Define a URL de retorno
                ReturnUrl = returnUrl;
                // Carrega os provedores de autenticação externa disponíveis
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            }

            // Método executado quando o formulário é enviado (requisição POST)
            public async Task<IActionResult> OnPostAsync(string returnUrl = null)
            {
                // Define a URL de retorno padrão se não for fornecida
                returnUrl ??= Url.Content("~/");
                // Recarrega os provedores de autenticação externa
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            
                // Verifica se o modelo é válido (todos os campos obrigatórios foram preenchidos corretamente)
                if (ModelState.IsValid)
                {
                    // Cria uma nova instância do usuário
                    var user = CreateUser();

                    // Define o nome de usuário como o e-mail fornecido
                    await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                    // Define o e-mail do usuário
                    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                    // Define o nome do usuário
                    user.Nome = Input.Nome;
                    user.DataCriacao = DateTime.Now;
                    // Cria o usuário no banco de dados com a senha fornecida
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    // Verifica se o usuário foi criado com sucesso
                    if (result.Succeeded)
                    {
                        // Registra no log que um novo usuário foi criado
                        _logger.LogInformation("Usuário criou uma nova conta com senha.");

                        // Obtém o ID do usuário recém-criado
                        var userId = await _userManager.GetUserIdAsync(user);
                        // Gera um token de confirmação de e-mail
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        // Codifica o token em Base64
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        // Cria a URL de callback para confirmação de e-mail
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        // Envia o e-mail de confirmação para o usuário
                        await _emailSender.SendEmailAsync(Input.Email, "Confirme seu e-mail",
                            $"Por favor, confirme sua conta <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicando aqui</a>.");

                        // Verifica se é necessário confirmar a conta antes de fazer login
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            // Redireciona para a página de confirmação de registro
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            // Faz login automático do usuário
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            // Redireciona para a URL de retorno
                            return LocalRedirect(returnUrl);
                        }
                    }
                
                    // Se houver erros na criação do usuário, adiciona-os ao ModelState
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                // Retorna a página com os erros de validação
                return Page();
            }

            // Método auxiliar para criar uma nova instância de ApplicationUser
            private ApplicationUser CreateUser()
            {
                try
                {
                    // Cria uma nova instância usando Activator
                    return Activator.CreateInstance<ApplicationUser>();
                }
                catch
                {
                    // Lança exceção se não for possível criar a instância
                    throw new InvalidOperationException($"Não é possível criar uma instância de '{nameof(ApplicationUser)}'. " +
                        $"Certifique-se de que '{nameof(ApplicationUser)}' não é uma classe abstrata e possui um construtor sem parâmetros, ou " +
                        $"substitua a página de registro em /Areas/Identity/Pages/Account/Register.cshtml");
                }
            }

            // Método auxiliar para obter o armazenamento de e-mails
            private IUserEmailStore<ApplicationUser> GetEmailStore()
            {
                // Verifica se o gerenciador de usuários suporta e-mail
                if (!_userManager.SupportsUserEmail)
                {
                    throw new NotSupportedException("A interface padrão requer um armazenamento de usuário com suporte a e-mail.");
                }
                // Retorna o armazenamento convertido para IUserEmailStore
                return (IUserEmailStore<ApplicationUser>)_userStore;
            }
        }
}
