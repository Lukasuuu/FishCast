using FishCast.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FishCast.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Aplicar migrações pendentes
            await context.Database.MigrateAsync();

            // Seed Roles
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Seed Peixes
            if (!await context.Peixes.AnyAsync())
            {
                var peixes = new List<Peixe>
                {
                    new Peixe
                    {
                        Nome = "Robalo",
                        NomeCientifico = "Dicentrarchus labrax",
                        Especie = "Robalo",
                        Tipo = "Marinho",
                        HabitatTipo = "Mar",
                        TamanhoMedioCm = 60,
                        Descricao = "Peixe predador muito apreciado na costa portuguesa. Vive em zonas rochosas e praias de arriba."
                    },
                    new Peixe
                    {
                        Nome = "Dourada",
                        NomeCientifico = "Sparus aurata",
                        Especie = "Dourada",
                        Tipo = "Marinho",
                        HabitatTipo = "Mar",
                        TamanhoMedioCm = 50,
                        Descricao = "Peixe nobre da costa portuguesa, muito valorizado pela sua carne. Frequenta estuários e zonas costeiras."
                    },
                    new Peixe
                    {
                        Nome = "Linguado",
                        NomeCientifico = "Solea solea",
                        Especie = "Linguado",
                        Tipo = "Marinho",
                        HabitatTipo = "Mar",
                        TamanhoMedioCm = 40,
                        Descricao = "Peixe plano que vive enterrado na areia. Muito comum nas praias do Minho."
                    },
                    new Peixe
                    {
                        Nome = "Polvo",
                        NomeCientifico = "Octopus vulgaris",
                        Especie = "Polvo",
                        Tipo = "Marinho",
                        HabitatTipo = "Mar",
                        TamanhoMedioCm = 100,
                        Descricao = "Cefalópode inteligente que habita rochas e fundos arenosos. Excelente para cataplana."
                    },
                    new Peixe
                    {
                        Nome = "Corvina",
                        NomeCientifico = "Argyrosomus regius",
                        Especie = "Corvina",
                        Tipo = "Marinho",
                        HabitatTipo = "Estuário",
                        TamanhoMedioCm = 120,
                        Descricao = "Grande peixe dos estuários, famoso pelo seu som característico. Pode atingir grandes dimensões."
                    },
                    new Peixe
                    {
                        Nome = "Sargo",
                        NomeCientifico = "Diplodus sargus",
                        Especie = "Sargo",
                        Tipo = "Marinho",
                        HabitatTipo = "Mar",
                        TamanhoMedioCm = 35,
                        Descricao = "Peixe comum nas rochas do litoral minhoto. Muito combative para a pesca desportiva."
                    },
                    new Peixe
                    {
                        Nome = "Ruivo",
                        NomeCientifico = "Mullus surmuletus",
                        Especie = "Ruivo",
                        Tipo = "Marinho",
                        HabitatTipo = "Mar",
                        TamanhoMedioCm = 25,
                        Descricao = "Peixe de carne delicada que habita fundos arenosos. As suas barbilhas são características."
                    },
                    new Peixe
                    {
                        Nome = "Enguia",
                        NomeCientifico = "Anguilla anguilla",
                        Especie = "Enguia",
                        Tipo = "Dulcícola",
                        HabitatTipo = "Rio",
                        TamanhoMedioCm = 80,
                        Descricao = "Peixe serpentiforme que vive nos rios do Minho e desova no Mar dos Sargaços."
                    },
                    new Peixe
                    {
                        Nome = "Pargo",
                        NomeCientifico = "Pagrus pagrus",
                        Especie = "Pargo",
                        Tipo = "Marinho",
                        HabitatTipo = "Mar",
                        TamanhoMedioCm = 70,
                        Descricao = "Peixe de grandes dimensões com corpo avermelhado. Excelente para grelhados."
                    },
                    new Peixe
                    {
                        Nome = "Salmonete",
                        NomeCientifico = "Mullus barbatus",
                        Especie = "Salmonete",
                        Tipo = "Marinho",
                        HabitatTipo = "Mar",
                        TamanhoMedioCm = 20,
                        Descricao = "Primo menor do ruivo, muito apreciado em receitas tradicionais do Minho."
                    }
                };

                await context.Peixes.AddRangeAsync(peixes);
                await context.SaveChangesAsync();
            }

            // Seed Utilizadores (Email, Password, Nome, Username, Localidade, TipoPesca, Bio)
            var utilizadoresSeed = new List<(string Email, string Password, string Nome, string Username, string Localidade, string TipoPesca, string Bio)>
            {
                ("joaominhoto@fishcast.pt", "Pesca@2026!", "João Minhoto", "joaominhoto", "Cabedelo", "Surf Casting", "Pescador apaixonado pelo Cabedelo. Sempre em busca do Robalo perfeito!"),
                ("mariapesca@fishcast.pt", "Pesca@2026!", "Maria Pesca", "mariapesca", "Âncora", "Pesca à Bóia", "Amante da pesca em família nas praias do Alto Minho."),
                ("pedroviana@fishcast.pt", "Pesca@2026!", "Pedro Viana", "pedroviana", "Viana do Castelo", "Mar", "Pescador desde criança. Conheço cada pedra da costa vianense."),
                ("anaribeiro@fishcast.pt", "Pesca@2026!", "Ana Ribeiro", "anaribeiro", "Afife", "Surf Casting", "A única mulher do grupo de pesca local. Representando as mulheres no mar!"),
                ("ruisurfcast@fishcast.pt", "Pesca@2026!", "Rui Surfcast", "ruisurfcast", "Moledo", "Lure", "Especialista em pesca com iscas artificiais. Moledo é o meu quintal."),
                ("lukasu@fishcast.pt", "Pesca@2026!", "Lukasu", "lukasu", "Viana do Castelo", "Surf Casting", "Entusiasta da pesca e desenvolvedor do FishCast. 🎣")
            };

            var users = new List<ApplicationUser>();
            foreach (var (email, password, nome, username, localidade, tipoPesca, bio) in utilizadoresSeed)
            {
                if (await userManager.FindByEmailAsync(email) == null && await userManager.FindByNameAsync(username) == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = username,
                        Email = email,
                        Nome = nome,
                        Localidade = localidade,
                        TipoPescaFavorito = tipoPesca,
                        Bio = bio,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "User");
                        users.Add(user);
                    }
                }
                else
                {
                    users.Add(await userManager.FindByEmailAsync(email) ?? await userManager.FindByNameAsync(username));
                }
            }

            // Garantir que temos os usuários carregados
            users = users.Where(u => u != null).ToList();

            // Seed Capturas
            if (!await context.Capturas.AnyAsync() && users.Any())
            {
                var peixes = await context.Peixes.ToListAsync();
                var praias = new[] { "Cabedelo", "Praia Norte", "Âncora", "Afife", "Moledo" };
                var tiposPesca = new[] { "Surf Casting", "Pesca à Bóia", "Mar", "Lure", "Rio" };
                var mares = new[] { "Maré Cheia", "Maré Vazia", "Maré Subindo", "Maré Descendo" };

                var random = new Random(42); // Seed para reprodutibilidade
                var capturas = new List<Captura>();

                var titulos = new[] {
                    "Grande captura de hoje!", "Manhã memorável", "O mar estava generoso",
                    "Captura épica", "Dia de sorte", "Pescaria incrível", "O Robalo apareceu",
                    "Fim de semana perfeito", "Nascer do sol e pesca", "Recompensa do mar"
                };

                var observacoes = new[] {
                    "Excelente manhã na praia. A maré estava perfeita.",
                    "Peixe capturado com isca artificial às 6h da manhã.",
                    "Dia espetacular com amigos e família.",
                    "Condições ideais de mar. Água limpa e calma.",
                    "Mais um dia incrível nas praias do Minho!",
                    "Pescaria de surf casting com resultados fantásticos.",
                    "Primeira captura do dia logo ao nascer do sol.",
                    "Demorou mas apareceu o grande!",
                    "Sessão noturna muito produtiva.",
                    "Maré cheia rendeu bons frutos hoje."
                };

                for (int i = 0; i < 20; i++)
                {
                    var user = users[random.Next(users.Count)];
                    var peixe = peixes[random.Next(peixes.Count)];
                    var praia = praias[random.Next(praias.Length)];
                    var tipoPesca = tiposPesca[random.Next(tiposPesca.Length)];
                    var mare = mares[random.Next(mares.Length)];

                    // Peso aleatório entre 0.3 e 8.5 kg
                    var peso = Math.Round((decimal)(random.NextDouble() * 8.2 + 0.3), 2);

                    // Data nos últimos 60 dias
                    var data = DateTime.Now.AddDays(-random.Next(60));

                    capturas.Add(new Captura
                    {
                        UtilizadorId = user.Id,
                        PeixeId = peixe.Id,
                        Titulo = titulos[random.Next(titulos.Length)],
                        Local = praia,
                        Praia = praia,
                        TipoPesca = tipoPesca,
                        Mare = mare,
                        DataHora = data,
                        Quantidade = random.Next(1, 5),
                        PesoKg = peso,
                        Observacao = observacoes[random.Next(observacoes.Length)],
                        ImagemPath = $"/uploads/capturas/sample_{(i % 5) + 1}.jpg"
                    });
                }

                await context.Capturas.AddRangeAsync(capturas);
                await context.SaveChangesAsync();

                // Criar alguns seguidores entre os utilizadores
                var capturasDb = await context.Capturas.ToListAsync();
                foreach (var user in users)
                {
                    foreach (var otherUser in users.Where(u => u.Id != user.Id))
                    {
                        if (random.Next(2) == 0) // 50% chance de seguir
                        {
                            if (!await context.Seguidores.AnyAsync(s => s.SeguidorId == user.Id && s.SeguidoId == otherUser.Id))
                            {
                                context.Seguidores.Add(new Seguidor
                                {
                                    SeguidorId = user.Id,
                                    SeguidoId = otherUser.Id
                                });
                            }
                        }
                    }
                }

                // Criar alguns likes aleatórios
                foreach (var captura in capturasDb)
                {
                    foreach (var user in users)
                    {
                        if (random.Next(3) == 0) // ~33% chance de dar like
                        {
                            if (!await context.CapturaLikes.AnyAsync(l => l.CapturaId == captura.Id && l.UtilizadorId == user.Id))
                            {
                                context.CapturaLikes.Add(new CapturaLike
                                {
                                    CapturaId = captura.Id,
                                    UtilizadorId = user.Id
                                });
                            }
                        }
                    }
                }

                // Adicionar alguns comentários
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

                foreach (var captura in capturasDb.Take(10))
                {
                    var numComentarios = random.Next(1, 4);
                    for (int c = 0; c < numComentarios; c++)
                    {
                        var commenter = users[random.Next(users.Count)];
                        context.Comentarios.Add(new Comentario
                        {
                            CapturaId = captura.Id,
                            UtilizadorId = commenter.Id,
                            Texto = comentariosTexto[random.Next(comentariosTexto.Length)],
                            DataComentario = captura.DataHora.AddHours(random.Next(1, 48))
                        });
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
