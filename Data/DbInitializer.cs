using FishCast.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FishCast.Data
{
    // Executa uma única vez no arranque da aplicação (chamado em Program.cs).
    // Aplica migrações pendentes, cria a role "User" e popula a BD com dados de demonstração.
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider provedorServicos)
        {
            using var escopo = provedorServicos.CreateScope();
            var contexto = escopo.ServiceProvider.GetRequiredService<AppDbContext>();
            var gerUsuarios = escopo.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var gerPerfis = escopo.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await contexto.Database.MigrateAsync();

            if (!await gerPerfis.RoleExistsAsync("User"))
                await gerPerfis.CreateAsync(new IdentityRole("User"));

            // Só popula peixes se a tabela estiver vazia (evita duplicados em restarts)
            if (!await contexto.Peixes.AnyAsync())
            {
                var peixes = new List<Peixe>
                {
                    new Peixe { Nome = "Robalo",     NomeCientifico = "Dicentrarchus labrax",  Especie = "Robalo",     Tipo = "Marinho",    HabitatTipo = "Mar",      TamanhoMedioCm = 60,  Descricao = "Peixe predador muito apreciado na costa portuguesa. Vive em zonas rochosas e praias de arriba.",      ImagemUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/0e/Dicentrarchus_labrax.jpg/640px-Dicentrarchus_labrax.jpg" },
                    new Peixe { Nome = "Dourada",    NomeCientifico = "Sparus aurata",          Especie = "Dourada",    Tipo = "Marinho",    HabitatTipo = "Mar",      TamanhoMedioCm = 50,  Descricao = "Peixe nobre da costa portuguesa, muito valorizado pela sua carne. Frequenta estuários e zonas costeiras.", ImagemUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/3/33/Sparus_aurata.jpg/640px-Sparus_aurata.jpg" },
                    new Peixe { Nome = "Linguado",   NomeCientifico = "Solea solea",            Especie = "Linguado",   Tipo = "Marinho",    HabitatTipo = "Mar",      TamanhoMedioCm = 40,  Descricao = "Peixe plano que vive enterrado na areia. Muito comum nas praias do Minho.",                              ImagemUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/96/Solea_solea_1.jpg/640px-Solea_solea_1.jpg" },
                    new Peixe { Nome = "Polvo",      NomeCientifico = "Octopus vulgaris",       Especie = "Polvo",      Tipo = "Marinho",    HabitatTipo = "Mar",      TamanhoMedioCm = 100, Descricao = "Cefalópode inteligente que habita rochas e fundos arenosos. Excelente para cataplana.",                  ImagemUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/26/Octopus_vulgaris_1.jpg/640px-Octopus_vulgaris_1.jpg" },
                    new Peixe { Nome = "Corvina",    NomeCientifico = "Argyrosomus regius",     Especie = "Corvina",    Tipo = "Marinho",    HabitatTipo = "Estuário", TamanhoMedioCm = 120, Descricao = "Grande peixe dos estuários, famoso pelo seu som característico. Pode atingir grandes dimensões.",         ImagemUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/8/8d/Argyrosomus_regius.jpg/640px-Argyrosomus_regius.jpg" },
                    new Peixe { Nome = "Sargo",      NomeCientifico = "Diplodus sargus",        Especie = "Sargo",      Tipo = "Marinho",    HabitatTipo = "Mar",      TamanhoMedioCm = 35,  Descricao = "Peixe comum nas rochas do litoral minhoto. Muito combative para a pesca desportiva.",                    ImagemUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/5/5e/Diplodus_sargus_1.jpg/640px-Diplodus_sargus_1.jpg" },
                    new Peixe { Nome = "Ruivo",      NomeCientifico = "Mullus surmuletus",      Especie = "Ruivo",      Tipo = "Marinho",    HabitatTipo = "Mar",      TamanhoMedioCm = 25,  Descricao = "Peixe de carne delicada que habita fundos arenosos. As suas barbilhas são características.",             ImagemUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/48/Mullus_surmuletus.jpg/640px-Mullus_surmuletus.jpg" },
                    new Peixe { Nome = "Enguia",     NomeCientifico = "Anguilla anguilla",      Especie = "Enguia",     Tipo = "Dulcícola",  HabitatTipo = "Rio",      TamanhoMedioCm = 80,  Descricao = "Peixe serpentiforme que vive nos rios do Minho e desova no Mar dos Sargaços.",                           ImagemUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/8/88/Anguilla_anguilla.jpg/640px-Anguilla_anguilla.jpg" },
                    new Peixe { Nome = "Pargo",      NomeCientifico = "Pagrus pagrus",          Especie = "Pargo",      Tipo = "Marinho",    HabitatTipo = "Mar",      TamanhoMedioCm = 70,  Descricao = "Peixe de grandes dimensões com corpo avermelhado. Excelente para grelhados.",                             ImagemUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3e/Pagrus_pagrus.jpg/640px-Pagrus_pagrus.jpg" },
                    new Peixe { Nome = "Salmonete",  NomeCientifico = "Mullus barbatus",        Especie = "Salmonete",  Tipo = "Marinho",    HabitatTipo = "Mar",      TamanhoMedioCm = 20,  Descricao = "Primo menor do ruivo, muito apreciado em receitas tradicionais do Minho.",                               ImagemUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/27/Mullus_barbatus.jpg/640px-Mullus_barbatus.jpg" }
                };

                await contexto.Peixes.AddRangeAsync(peixes);
                await contexto.SaveChangesAsync();
            }

            // Usuários de demonstração — senha idêntica para facilitar testes
            var usuariosSeed = new List<(string Email, string Senha, string Nome, string NomeLogin, string Localidade, string TipoPesca, string Bio, string FotoPerfil)>
            {
                ("joaominhoto@fishcast.pt",  "Pesca@2026!", "João Minhoto", "joaominhoto", "Cabedelo",          "Surf Casting",   "Pescador apaixonado pelo Cabedelo. Sempre em busca do Robalo perfeito!",             "https://i.pravatar.cc/150?img=11"),
                ("mariapesca@fishcast.pt",   "Pesca@2026!", "Maria Pesca",  "mariapesca",  "Âncora",            "Pesca à Bóia",   "Amante da pesca em família nas praias do Alto Minho.",                               "https://i.pravatar.cc/150?img=5"),
                ("pedroviana@fishcast.pt",   "Pesca@2026!", "Pedro Viana",  "pedroviana",  "Viana do Castelo",  "Mar",            "Pescador desde criança. Conheço cada pedra da costa vianense.",                     "https://i.pravatar.cc/150?img=12"),
                ("anaribeiro@fishcast.pt",   "Pesca@2026!", "Ana Ribeiro",  "anaribeiro",  "Afife",             "Surf Casting",   "A única mulher do grupo de pesca local. Representando as mulheres no mar!",          "https://i.pravatar.cc/150?img=9"),
                ("ruisurfcast@fishcast.pt",  "Pesca@2026!", "Rui Surfcast", "ruisurfcast", "Moledo",            "Lure",           "Especialista em pesca com iscas artificiais. Moledo é o meu quintal.",              "https://i.pravatar.cc/150?img=3"),
                ("lukasu@fishcast.pt",       "Pesca@2026!", "Lukasu",       "lukasu",      "Viana do Castelo",  "Surf Casting",   "Entusiasta da pesca e desenvolvedor do FishCast. 🎣",                               "https://i.pravatar.cc/150?img=8")
            };

            var usuarios = new List<ApplicationUser>();
            foreach (var (email, senha, nome, nomeLogin, localidade, tipoPesca, bio, fotoPerfil) in usuariosSeed)
            {
                // Verifica por email E por nomeLogin para evitar conflitos com registros manuais anteriores
                if (await gerUsuarios.FindByEmailAsync(email) == null && await gerUsuarios.FindByNameAsync(nomeLogin) == null)
                {
                    var usuario = new ApplicationUser
                    {
                        UserName = nomeLogin,
                        Email = email,
                        Nome = nome,
                        Localidade = localidade,
                        TipoPescaFavorito = tipoPesca,
                        Bio = bio,
                        FotoPerfil = fotoPerfil,
                        EmailConfirmed = true
                    };

                    var resultado = await gerUsuarios.CreateAsync(usuario, senha);
                    if (resultado.Succeeded)
                    {
                        await gerUsuarios.AddToRoleAsync(usuario, "User");
                        usuarios.Add(usuario);
                    }
                }
                else
                {
                    usuarios.Add(await gerUsuarios.FindByEmailAsync(email) ?? await gerUsuarios.FindByNameAsync(nomeLogin));
                }
            }

            usuarios = usuarios.Where(u => u != null).ToList();

            // Apaga TODAS as capturas de seed e recria sempre com as fotos atuais do wwwroot/images
            var todasCapturas = await contexto.Capturas
                .Include(c => c.Likes)
                .Include(c => c.Comentarios)
                .Where(c => c.ImagemPath == null || !c.ImagemPath.StartsWith("/uploads/"))
                .ToListAsync();

            if (todasCapturas.Any())
            {
                contexto.CapturaLikes.RemoveRange(todasCapturas.SelectMany(c => c.Likes));
                contexto.Comentarios.RemoveRange(todasCapturas.SelectMany(c => c.Comentarios));
                contexto.Capturas.RemoveRange(todasCapturas);
                await contexto.SaveChangesAsync();
            }

            // Cria posts de demonstração com uma foto real por post, sem repetições
            if (usuarios.Any())
            {
                var peixes = await contexto.Peixes.ToListAsync();
                var praias = new[] { "Cabedelo", "Praia Norte", "Âncora", "Afife", "Moledo" };
                var tiposPesca = new[] { "Surf Casting", "Pesca à Bóia", "Mar", "Lure", "Rio" };
                var mares = new[] { "Maré Cheia", "Maré Vazia", "Maré Subindo", "Maré Descendo" };
                var aleatorio = new Random(42);

                Peixe P(string nome) => peixes.FirstOrDefault(p => p.Nome == nome) ?? peixes[aleatorio.Next(peixes.Count)];

                // Cada entrada corresponde exatamente ao conteúdo visual da foto
                var capturasData = new (string Imagem, Peixe Peixe, string Titulo, string Praia, string Tipo, string Mare, string Obs)[]
                {
                    ("/images/hq720.jpg",
                     P("Sargo"), "Pôr do sol e sargos nas pedras",
                     "Cabedelo", "Surf Casting", "Maré Descendo",
                     "Sessão ao entardecer nas pedras do Cabedelo. Seis sargos em menos de duas horas — a maré a descer foi a chave."),

                    ("/images/Pargos-1.jpg",
                     P("Pargo"), "Dois pargos enormes ao largo",
                     "Praia Norte", "Mar", "Maré Cheia",
                     "Saída ao mar rendeu estes dois pargos de peso. Pesca de fundo com amêijoa a uns 40 metros de profundidade."),

                    ("/images/19052011069.jpg",
                     P("Sargo"), "Primeiro peixe no lure hoje",
                     "Afife", "Lure", "Maré Subindo",
                     "Ao entardecer na praia apanhei este com isca artificial. Pequeno mas animador — a sessão ainda ia a começar!"),

                    ("/images/02092010467.jpg",
                     P("Corvina"), "Par de chicharros fresquinhos",
                     "Âncora", "Pesca à Bóia", "Maré Vazia",
                     "Dois chicharros apanhados à bóia na doca. Já estão no lava-loiças prontos para grelhar — nada se desperdiça!"),

                    ("/images/09-pesca-desportiva-1080x1920px_w1920.jpg",
                     P("Pargo"), "Bica avermelhada de quase 2 kg",
                     "Moledo", "Mar", "Maré Cheia",
                     "Que exemplar! Apanhado ao largo numa saída de barco. Pesca de fundo com pilado, bicho lutou muito até vir à superfície."),

                    ("/images/Profundidade-4-1-scaled.jpeg",
                     P("Salmonete"), "Cinco bicas numa tarde no barco",
                     "Cabedelo", "Mar", "Maré Descendo",
                     "Dia de sorte ao largo — cinco bicas em série com aparelho de corrico. O boné Rapala está a trazer sorte!"),

                    ("/images/hq720 (1).jpg",
                     P("Robalo"), "Sete robalos na pedra",
                     "Praia Norte", "Surf Casting", "Maré Subindo",
                     "Safada de robalos numa manhã de surf casting. Fundo misto de rocha e areia foi o segredo desta sessão."),

                    ("/images/sddefault.jpg",
                     P("Dourada"), "Dez peixes na tábua ao fim do dia",
                     "Âncora", "Surf Casting", "Maré Cheia",
                     "Mistura de douradas e sargos apanhados na costa. Maré a subir de manhã cedo e condições de mar perfeitas."),

                    ("/images/DSC07585.JPG",
                     P("Sargo"), "Quatro bicas prontas para a frigideira",
                     "Moledo", "Pesca à Bóia", "Maré Vazia",
                     "Quatro bicas limpas no prato de inox. Apanhadas de manhã na bóia, ao almoço já estavam na mesa da família!"),

                    ("/images/maxresdefault.jpg",
                     P("Corvina"), "Sessão épica de surf casting nas rochas",
                     "Cabedelo", "Surf Casting", "Maré Descendo",
                     "Mais de dez peixes numa só tarde nas rochas com a cana. Palombetas e outros à fila — nunca vi o mar tão generoso."),

                    ("/images/hq720 (2).jpg",
                     P("Dourada"), "Palombetas na areia com mar azul",
                     "Afife", "Surf Casting", "Maré Subindo",
                     "Quatro palombetas brilhantes deitadas na areia com as varas ainda espetadas atrás. Surf casting com mar calmo e céu limpo."),
                };

                var capturas = new List<Captura>();
                foreach (var d in capturasData)
                {
                    capturas.Add(new Captura
                    {
                        UtilizadorId = usuarios[aleatorio.Next(usuarios.Count)].Id,
                        PeixeId = d.Peixe.Id,
                        Titulo = d.Titulo,
                        Local = d.Praia,
                        Praia = d.Praia,
                        TipoPesca = d.Tipo,
                        Mare = d.Mare,
                        DataHora = DateTime.Now.AddDays(-aleatorio.Next(60)),
                        Quantidade = aleatorio.Next(1, 5),
                        PesoKg = Math.Round((decimal)(aleatorio.NextDouble() * 8.2 + 0.3), 2),
                        Observacao = d.Obs,
                        ImagemPath = d.Imagem
                    });
                }

                await contexto.Capturas.AddRangeAsync(capturas);
                await contexto.SaveChangesAsync();

                var capturasSalvas = await contexto.Capturas.ToListAsync();

                // Seguidores aleatórios com 50% de probabilidade entre cada par de usuários
                foreach (var usuario in usuarios)
                {
                    foreach (var outroUsuario in usuarios.Where(u => u.Id != usuario.Id))
                    {
                        if (aleatorio.Next(2) == 0 && !await contexto.Seguidores.AnyAsync(s => s.SeguidorId == usuario.Id && s.SeguidoId == outroUsuario.Id))
                            contexto.Seguidores.Add(new Seguidor { SeguidorId = usuario.Id, SeguidoId = outroUsuario.Id });
                    }
                }

                // Curtidas aleatórias com ~33% de probabilidade por usuário/captura
                foreach (var captura in capturasSalvas)
                {
                    foreach (var usuario in usuarios)
                    {
                        if (aleatorio.Next(3) == 0 && !await contexto.CapturaLikes.AnyAsync(l => l.CapturaId == captura.Id && l.UtilizadorId == usuario.Id))
                            contexto.CapturaLikes.Add(new CapturaLike { CapturaId = captura.Id, UtilizadorId = usuario.Id });
                    }
                }

                var comentariosTexto = new[] {
                    "Grande captura! Parabéns!",
                    "Que lindo exemplar!",
                    "Boa! Onde foi isso?",
                    "Está na hora de um jantarzinho!",
                    "Muitos parabéns, excelente trabalho!",
                    "Essas são as manhãs que vale a pena acordar cedo!",
                    "Que sorte! O mar estava generoso.",
                    "Grandes memórias de pesca!",
                    "Impressionante! Qual a isca?",
                    "Adoro ver estas capturas!"
                };

                // Comentários apenas nas primeiras 10 capturas para simular atividade inicial realista
                foreach (var captura in capturasSalvas.Take(10))
                {
                    for (int c = 0; c < aleatorio.Next(1, 4); c++)
                    {
                        var comentarista = usuarios[aleatorio.Next(usuarios.Count)];
                        contexto.Comentarios.Add(new Comentario
                        {
                            CapturaId = captura.Id,
                            UtilizadorId = comentarista.Id,
                            Texto = comentariosTexto[aleatorio.Next(comentariosTexto.Length)],
                            DataComentario = captura.DataHora.AddHours(aleatorio.Next(1, 48))
                        });
                    }
                }

                await contexto.SaveChangesAsync();
            }
        }
    }
}
