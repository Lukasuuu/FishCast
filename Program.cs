using FishCast.Data;
using FishCast.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- SERVIÇOS (injeção de dependência) ---

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity com email único; confirmação de conta desativada para simplificar o registo
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole>()           // necessário para [Authorize(Roles="Admin")]
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// --- PIPELINE DE MIDDLEWARE (a ordem importa) ---

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Home/Error");

// UseStaticFiles antes do routing garante que /images/, /uploads/, /css/, /js/
// são servidos diretamente do wwwroot sem passar pelo pipeline de autenticação
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();    // quem és tu?
app.UseAuthorization();     // o que podes fazer?
app.UseSession();
app.MapStaticAssets();
app.MapRazorPages();        // necessário para as páginas de Identity (Login, Register, etc.)

// Rota MVC padrão: /Controller/Action/id
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Seed inicial da base de dados (peixes, utilizadores e capturas de exemplo)
using (var escopo = app.Services.CreateScope())
{
    var servicos = escopo.ServiceProvider;
    try
    {
        await DbInitializer.SeedAsync(servicos);
    }
    catch (Exception ex)
    {
        var logger = servicos.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao inicializar a base de dados.");
    }
}

app.Run();
