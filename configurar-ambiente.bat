@echo off
chcp 65001 >nul
echo ===========================================
echo    FishCast - Configuração de Ambiente
echo ===========================================
echo.

:: Define a variável de ambiente para a connection string
setx ConnectionStrings__DefaultConnection "Server=62.28.39.135,62444;Database=FishCastDB;user=efa0125;password=efa0125@;TrustServerCertificate=True;" /M

echo ✅ Variável de ambiente configurada com sucesso!
echo.
echo ConnectionStrings__DefaultConnection
echo.
echo ===========================================
echo    IMPORTANTE: Reinicie o computador
echo    ou faça logout/login para aplicar
echo ===========================================
echo.
pause
