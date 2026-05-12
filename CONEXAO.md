# Configuração da Connection String

Este documento explica como configurar a connection string do SQL Server de forma segura.

## Opção 1: Variável de Ambiente (Recomendado)

### Windows (PowerShell como Administrador)
```powershell
[Environment]::SetEnvironmentVariable("ConnectionStrings__DefaultConnection", "Server=62.28.39.135,62444;Database=FishCastDB;user=efa0125;password=EFA0125@;TrustServerCertificate=True;", "Machine")
```

### Windows (CMD como Administrador)
```cmd
setx ConnectionStrings__DefaultConnection "Server=62.28.39.135,62444;Database=FishCastDB;user=efa0125;password=EFA0125@;TrustServerCertificate=True;" /M
```

Ou execute o ficheiro `configurar-ambiente.bat` como Administrador.

**Nota:** Após definir a variável de ambiente, reinicie o Visual Studio ou terminal.

### Linux/macOS
```bash
export ConnectionStrings__DefaultConnection="Server=62.28.39.135,62444;Database=FishCastDB;user=efa0125;password=EFA0125@;TrustServerCertificate=True;"
```

Para tornar permanente, adicione ao `~/.bashrc` ou `~/.zshrc`.

### Docker
```dockerfile
ENV ConnectionStrings__DefaultConnection="Server=62.28.39.135,62444;Database=FishCastDB;user=efa0125;password=EFA0125@;TrustServerCertificate=True;"
```

### Azure / Servidores Cloud
Use "Application Settings" ou "Configuration" no portal do serviço.

## Opção 2: User Secrets (Desenvolvimento)

```bash
cd FishCast

dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=62.28.39.135,62444;Database=FishCastDB;user=efa0125;password=EFA0125@;TrustServerCertificate=True;"
```

Para verificar:
```bash
dotnet user-secrets list
```

## Opção 3: appsettings.Development.json

Crie o ficheiro `appsettings.Development.json` (não é rastreado pelo Git se estiver no .gitignore):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=62.28.39.135,62444;Database=FishCastDB;user=efa0125;password=EFA0125@;TrustServerCertificate=True;"
  }
}
```

## Prioridade de Configuração

O ASP.NET Core lê as configurações na seguinte ordem (última sobrescreve anteriores):

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. User Secrets (apenas em Development)
4. Variáveis de ambiente
5. Argumentos de linha de comando

## Verificação

Para verificar se a configuração está funcionando, execute:

```bash
dotnet run
```

O aplicativo deve conectar-se ao SQL Server remoto especificado.

## Segurança

⚠️ **Nunca comite credenciais no Git!**

O `.gitignore` já está configurado para ignorar:
- `appsettings.Development.json`
- Ficheiros de segredos do utilizador

Use sempre variáveis de ambiente em produção.

## Troubleshooting

### Erro: "Login failed for user"
Verifique se o user e password estão corretos.

### Erro: "A network-related or instance-specific error"
Verifique se o IP e porta estão acessíveis e se não há firewall a bloquear.

### Erro: "The certificate chain was issued by an authority that is not trusted"
A opção `TrustServerCertificate=True` já está incluída na string.
