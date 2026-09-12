@echo off
title HydraTentacle - Database Migrations
echo.
echo =======================================================
echo   HydraTentacle - Veri Tabani Guncelleyici (EF Core)
echo =======================================================
echo.

set "ROOT_DIR=%~dp0.."
set "CORE_PROJ=%ROOT_DIR%\Source\HydraTentacle.Core"
set "API_PROJ=%ROOT_DIR%\Source\HydraTentacle.WebApi"

echo [1/2] Projeler kontrol ediliyor...
echo   - Core Projesi: %CORE_PROJ%
echo   - Startup Projesi: %API_PROJ%
echo.

echo [2/2] Entity Framework Core Migration'lari uygulaniyor...
dotnet ef database update -p "%CORE_PROJ%" -s "%API_PROJ%"
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [HATA] Veri tabani guncellemesi basarisiz oldu!
    echo Not: 'dotnet ef' CLI araci yuklu degilse su komutu calistirabilirsiniz:
    echo dotnet tool install --global dotnet-ef
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo =======================================================
echo   Veri tabani basariyla guncellendi!
echo =======================================================
echo.
pause
