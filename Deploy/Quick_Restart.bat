@echo off
title HydraTentacle - Quick Restart
echo.
echo =======================================================
echo   HydraTentacle - Hizli Yeniden Baslatici (Incremental)
echo =======================================================
echo.

set "ROOT_DIR=%~dp0.."
set "SLN=%ROOT_DIR%\HydraTentacle.sln"

echo [1/3] Onceki calisan ornekler sonlandiriliyor...
taskkill /F /IM HydraTentacle.Blazor.exe 2>nul
taskkill /F /IM HydraTentacle.WebApi.exe 2>nul
timeout /t 1 /nobreak >nul

echo.
echo [2/3] Sadece degisen dosyalar derleniyor (Restore atlandi)...
dotnet build "%SLN%" -c Debug --no-restore
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [HATA] Derleme sirasinda hata olustu!
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo [3/3] Servisler derlemesiz hizlica baslatiliyor...
start "HydraTentacle - WebApi" "%~dp01_Run_WebApi.bat" --no-build
timeout /t 2 /nobreak >nul
start "HydraTentacle - Blazor UI" "%~dp02_Run_Blazor.bat" --no-build

echo.
echo =======================================================
echo   Hizli baslatma tamamlandi! (Paket taramasi atlandi)
echo   - WebApi : http://localhost:5132
echo   - Blazor : http://localhost:5121
echo =======================================================
echo.
