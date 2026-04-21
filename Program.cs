using FishCast.Data;
using FishCast.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// Ele configura os serviços necessários, como controladores, cache em memória e sessões, e define o pipeline de solicitação HTTP.
var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao contêiner.

// O código a seguir adiciona o serviço de contexto de banco de dados usando Entity Framework Core com SQL Server.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Configura o Identity para usar a classe ApplicationUser personalizada e o contexto de banco de dados.
//false para RequireConfirmedAccount significa que os usuários não precisam confirmar suas contas por e-mail para fazer login.

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true; // Torna o email obrigatório e garante que cada email seja único
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddControllersWithViews();   // ← ADICIONAR suporte para controladores e visualizações (MVC)
builder.Services.AddDistributedMemoryCache(); // ← ADICIONAR a cache em memória para a sessão
builder.Services.AddSession(options =>        // ← ADICIONAR a sessão
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configurar o pipeline de solicitação HTTP.
// Que significa isso? O pipeline de solicitação HTTP é a sequência de middleware que processa as solicitações HTTP recebidas e gera respostas.
// Cada middleware pode realizar ações antes e depois de chamar o próximo middleware na cadeia.
// O código a seguir configura o pipeline para lidar com erros, roteamento, autorização e sessões.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");  // Em ambientes de produção, use um manipulador de exceção personalizado para lidar com erros de forma mais amigável.
}

app.UseRouting();

app.UseAuthentication(); // ← HABILITAR o middleware de autenticação para permitir que os usuários façam login e acessem recursos protegidos
app.UseAuthorization(); // ← HABILITAR o middleware de autorização para proteger as rotas que exigem autenticação


app.UseSession();       // ← HABILITAR o middleware de sessão

app.UseStaticFiles(); // ← HABILITAR o middleware de arquivos estáticos para servir arquivos como CSS, JavaScript e imagens da pasta wwwroot

app.MapStaticAssets();

app.MapRazorPages(); // ← HABILITAR o roteamento para páginas Razor (usado pelo Identity para as páginas de login, registro, etc.)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Executar seed do banco de dados
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbInitializer.SeedAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao inicializar o banco de dados.");
    }
}

app.Run();
