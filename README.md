# 🎣 FishCast - Rede Social de Pesca

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white" alt=".NET 10">
  <img src="https://img.shields.io/badge/ASP.NET-MVC-512BD4?logo=dotnet&logoColor=white" alt="ASP.NET MVC">
  <img src="https://img.shields.io/badge/Entity%20Framework-Core-512BD4?logo=dotnet&logoColor=white" alt="EF Core">
  <img src="https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoft-sql-server&logoColor=white" alt="SQL Server">
  <img src="https://img.shields.io/badge/Bootstrap-5-7952B3?logo=bootstrap&logoColor=white" alt="Bootstrap 5">
</p>

<p align="center">
  <b>🌊 O Instagram dos Pescadores das Praias de Viana do Castelo</b>
</p>

---

## 📋 Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [Funcionalidades](#funcionalidades)
- [Tecnologias](#tecnologias)
- [Instalação](#instalação)
- [Utilização](#utilização)
- [Contas de Teste](#contas-de-teste)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Roadmap](#roadmap)
- [Screenshots](#screenshots)
- [Contato](#contato)

---

## 🎯 Sobre o Projeto

**FishCast** é uma rede social completa desenvolvida em **ASP.NET Core 10 MVC**, inspirada no Instagram, mas dedicada exclusivamente à comunidade piscatória da região de **Viana do Castelo**, Portugal.

A aplicação permite aos pescadores:
- 📸 Partilhar fotos das suas capturas
- 🗺️ Registar o local exato da pescaria (Cabedelo, Âncora, Afife, etc.)
- 🐟 Identificar espécies de peixes
- 👥 Seguir outros pescadores
- ❤️ Dar like e comentar em publicações
- 📊 Explorar capturas por filtros (praia, espécie, tipo de pesca)

### 🏖️ Praias Suportadas

- Cabedelo
- Afife
- Âncora
- Moledo
- Montedor
- Vila Praia de Âncora
- Caminha
- Viana do Castelo

---

## ✨ Funcionalidades

### 👤 Utilizadores
- ✅ Registo e autenticação completa com Identity
- ✅ Login com Email ou Nome de utilizador (estilo Instagram)
- ✅ Perfil personalizado com foto, bio e localidade
- ✅ Sistema de seguidores (seguir/deixar de seguir)
- ✅ Lista de seguidores e seguindo
- ✅ Tipos de pesca favoritos

### 📸 Capturas/Publicações
- ✅ Criar, editar e eliminar capturas
- ✅ Upload de fotos (JPG, PNG, WEBP - máx 5MB)
- ✅ Informações detalhadas:
  - Espécie do peixe
  - Peso e quantidade
  - Local/Praia
  - Tipo de pesca
  - Maré (Cheia, Vazia, Subindo, Descendo)
  - Data e hora
  - Observações
- ✅ Sistema de likes
- ✅ Comentários em publicações

### 🐟 Catálogo de Peixes
- ✅ Base de dados com 10 espécies
- ✅ Nomes científicos
- ✅ Habitats (Mar, Rio, Estuário)
- ✅ Imagens e descrições
- ✅ Capturas associadas a cada espécie

### 🏠 Feed e Exploração
- ✅ Feed principal com todas as capturas
- ✅ Paginação (10 itens por página)
- ✅ Filtros por:
  - Praia
  - Espécie de peixe
  - Tipo de pesca
- ✅ Ordenação (mais recentes, mais likes, maior peso)

### 🎨 Interface
- ✅ Design responsivo (mobile-friendly)
- ✅ Tema escuro "Ocean Dark"
- ✅ Paleta de cores inspirada no mar
- ✅ Cards estilo Instagram
- ✅ Animações e transições suaves
- ✅ Preview de imagens antes do upload

---

## 🛠️ Tecnologias

| Camada | Tecnologia |
|--------|------------|
| **Backend** | ASP.NET Core 10 MVC |
| **Autenticação** | ASP.NET Identity |
| **ORM** | Entity Framework Core 10 |
| **Base de Dados** | SQL Server LocalDB |
| **Frontend** | HTML5, CSS3, Bootstrap 5 |
| **Ícones** | Bootstrap Icons |
| **Fonts** | Google Fonts (Inter, Playfair Display) |

---

## 🚀 Instalação

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server LocalDB](https://docs.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb) ou SQL Server
- Git (opcional)

### Passos

1. **Clonar o repositório**
   ```bash
   git clone https://github.com/Lukasuuu/FishCast.git
   cd FishCast
   ```

2. **Restaurar pacotes**
   ```bash
   dotnet restore
   ```

3. **Configurar a base de dados**

   Por padrão, a connection string aponta para LocalDB. Para usar SQL Server remoto, veja [CONEXAO.md](CONEXAO.md).

   Para configurar via variável de ambiente (recomendado para produção):
   ```powershell
   [Environment]::SetEnvironmentVariable("ConnectionStrings__DefaultConnection", "sua-string-de-conexao", "Machine")
   ```

4. **Aplicar migrações e criar base de dados**
   ```bash
   dotnet ef database update
   ```

5. **Executar**
   ```bash
   dotnet run
   ```

6. **Abrir no browser**
   - Aplicação: `https://localhost:5001` ou `http://localhost:5000`

---

## 🎮 Utilização

### Primeiro Acesso

1. Ao iniciar pela primeira vez, a base de dados é automaticamente populada com:
   - 10 espécies de peixes
   - 6 utilizadores de teste
   - 20 capturas de exemplo

2. Faça login com uma das contas de teste abaixo 👇 (pode usar Email ou Username)

### Fluxo Típico

1. **Login** → Escolha uma conta de teste
2. **Feed** → Veja capturas de outros pescadores
3. **Publicar** → Clique em "📸 Publicar +" para partilhar a sua captura
4. **Explorar** → Descubra capturas por praia ou espécie
5. **Perfil** → Veja o seu perfil e estatísticas
6. **Seguir** → Encontre outros pescadores e siga-os

---

## 🔑 Contas de Teste

Utilize estas credenciais para testar a aplicação. Pode fazer login com **Email** ou **Nome de utilizador** (como no Instagram).

| Email | Username | Password | Nome | Localidade | Tipo Preferido |
|-------|----------|----------|------|------------|----------------|
| `joaominhoto@fishcast.pt` | `joaominhoto` | `Pesca@2026!` | João Minhoto | Cabedelo | Surf Casting |
| `mariapesca@fishcast.pt` | `mariapesca` | `Pesca@2026!` | Maria Pesca | Âncora | Pesca à Bóia |
| `pedroviana@fishcast.pt` | `pedroviana` | `Pesca@2026!` | Pedro Viana | Viana do Castelo | Mar |
| `anaribeiro@fishcast.pt` | `anaribeiro` | `Pesca@2026!` | Ana Ribeiro | Afife | Surf Casting |
| `ruisurfcast@fishcast.pt` | `ruisurfcast` | `Pesca@2026!` | Rui Surfcast | Moledo | Lure |
| `lukasu@fishcast.pt` | `lukasu` | `Pesca@2026!` | Lukasu | Viana do Castelo | Surf Casting |

> 💡 **Dica:** Pode fazer login tanto com o email (ex: `joaominhoto@fishcast.pt`) quanto com o username (ex: `joaominhoto`).
>
> ⚠️ Todas as contas têm a mesma password: `Pesca@2026!`

---

## 📁 Estrutura do Projeto

```
FishCast/
├── Areas/
│   └── Identity/
│       └── Pages/
│           └── Account/
│               ├── Login.cshtml      # Página de login (tema escuro)
│               └── Register.cshtml   # Página de registo
├── Controllers/
│   ├── HomeController.cs           # Feed principal
│   ├── CapturaController.cs      # CRUD de capturas, likes, comentários
│   ├── PerfilController.cs         # Perfis e seguidores
│   ├── ExplorarController.cs     # Exploração com filtros
│   └── PeixeController.cs          # Catálogo de espécies
├── Data/
│   ├── AppDbContext.cs             # Contexto EF Core
│   └── DbInitializer.cs            # Seed data (peixes, users, capturas)
├── Models/
│   ├── ApplicationUser.cs          # Utilizador (extends IdentityUser)
│   ├── Captura.cs                  # Captura/Publicação
│   ├── Peixe.cs                    # Espécie de peixe
│   ├── Comentario.cs               # Comentário em captura
│   ├── Seguidor.cs                 # Relação seguir
│   └── CapturaLike.cs              # Like em captura
├── ViewModels/                     # ViewModels para formulários
├── Views/
│   ├── Shared/                     # Layouts e partials
│   ├── Home/                       # Feed
│   ├── Captura/                    # CRUD de capturas
│   ├── Perfil/                     # Perfil e seguidores
│   ├── Explorar/                   # Exploração
│   └── Peixe/                      # Catálogo
├── wwwroot/
│   ├── css/site.css               # Tema Ocean Dark
│   ├── js/site.js                 # JavaScript utilitários
│   ├── images/                     # Imagens padrão
│   └── uploads/                    # Uploads de utilizadores
├── appsettings.json
└── Program.cs
```

---

## 🗺️ Roadmap

### Funcionalidades Implementadas ✅
- [x] Sistema de autenticação completo
- [x] CRUD de capturas com upload de imagens
- [x] Sistema de likes e comentários
- [x] Seguidores e seguindo
- [x] Feed com paginação
- [x] Catálogo de peixes
- [x] Filtros de exploração
- [x] Seed data automático
- [x] Design responsivo

### Futuras Melhorias 🚧
- [ ] Mapa interativo das praias
- [ ] Notificações em tempo real
- [ ] API REST para mobile app
- [ ] Previsão meteorológica integrada
- [ ] Calendário lunar para pesca
- [ ] Sistema de conquistas/badges
- [ ] Chat entre pescadores
- [ ] Exportação de dados de pesca

---

## 📸 Screenshots

<p align="center">
  <em>Capturas do Projeto (adicionar screenshots)</em>
</p>

### 🖥️ Desktop
- Feed com cards de capturas
- Página de perfil tipo Instagram
- Formulário de nova captura

### 📱 Mobile
- Interface responsiva
- Menu adaptativo
- Upload de fotos otimizado

---

## 🧪 Testar Localmente

### Comandos Úteis

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration

# Atualizar base de dados
dotnet ef database update

# Resetar base de dados (apagar e recriar)
dotnet ef database drop --force
dotnet ef database update

# Executar em modo desenvolvimento
dotnet run --environment Development

# Publicar para produção
dotnet publish -c Release
```

---

## 🎨 Personalização

### Cores do Tema

As cores são definidas em `wwwroot/css/site.css`:

```css
:root {
  --ocean-dark: #0a1628;      /* Fundo principal */
  --ocean-navy: #1a3a5c;      /* Azul escuro */
  --ocean-mid: #1e4976;       /* Azul médio */
  --cyan-accent: #00b4d8;       /* Destaque ciano */
  --coral: #e76f51;             /* Coral/laranja */
  --sand: #e9c46a;              /* Areia */
}
```

### Seed Data

Modifique `Data/DbInitializer.cs` para alterar:
- Peixes pré-cadastrados
- Utilizadores de teste
- Capturas de exemplo

---

## 🐛 Resolução de Problemas

### Erro: "Cannot open database"
```bash
# Certifique-se que o LocalDB está instalado
sqllocaldb info

# Criar instância se necessário
sqllocaldb create MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

### Erro: "Failed to apply migrations"
```bash
# Apagar e recriar a base de dados
dotnet ef database drop --force
dotnet ef database update
```

### Erro: "Port already in use"
```bash
# Alterar porta em Properties/launchSettings.json
# Ou usar:
dotnet run --urls "https://localhost:5002"
```

---

## 🤝 Contribuir

1. Faça um Fork do projeto
2. Crie uma branch (`git checkout -b feature/nova-funcionalidade`)
3. Commit (`git commit -m 'Adiciona nova funcionalidade'`)
4. Push (`git push origin feature/nova-funcionalidade`)
5. Abra um Pull Request

---

## 📄 Licença

Este projeto é open-source e está disponível para uso educacional e comercial.

---

## 📧 Contato

**Desenvolvedor:** Lukasuuu

**GitHub:** [@Lukasuuu](https://github.com/Lukasuuu)

**Localização:** Viana do Castelo, Portugal 🏖️

---

<p align="center">
  🎣 <b>FishCast</b> - Feito com ❤️ para a comunidade piscatória do Minho 🌊
</p>

<p align="center">
  <i>"O mar é o mesmo para todos, mas cada um pesca o que merece."</i>
</p>
