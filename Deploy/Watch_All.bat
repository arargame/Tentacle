@echo off
title HydraTentacle - Hot Reload (dotnet watch)
echo.
echo =======================================================
echo   HydraTentacle - Canli Gelistirme (Hot Reload)
echo =======================================================
echo.

set "ROOT_DIR=%~dp0.."
set "WEBAPI=%ROOT_DIR%\Source\HydraTentacle.WebApi\HydraTentacle.WebApi.csproj"
set "BLAZOR=%ROOT_DIR%\Source\HydraTentacle.Blazor\HydraTentacle.Blazor.csproj"

echo Onceki calisan ornekler sonlandiriliyor...
taskkill /F /IM HydraTentacle.Blazor.exe 2>nul
taskkill /F /IM HydraTentacle.WebApi.exe 2>nul
timeout /t 1 /nobreak >nul

echo.
echo WebApi Hot Reload modunda baslatiliyor...
start "HydraTentacle - WebApi (Watch)" cmd /k "dotnet watch --project \"%WEBAPI%\" run --launch-profile \"http\""

timeout /t 3 /nobreak >nul

echo Blazor UI Hot Reload modunda baslatiliyor...
start "HydraTentacle - Blazor UI (Watch)" cmd /k "dotnet watch --project \"%BLAZOR%\" run --launch-profile \"http\""

echo.
echo =======================================================
echo   Hot Reload modu aktif!
echo   C# ve Razor dosyalari kaydoldugunda aninda uygulanir.
echo   Tekrar BAT calistirip durdurmaniza gerek kalmaz.
echo =======================================================
echo.
