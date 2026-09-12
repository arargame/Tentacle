@echo off
title HydraTentacle - Full Stack Launcher
echo.
echo =======================================================
echo   HydraTentacle - Full Stack Baslatici (WebApi + Blazor)
echo =======================================================
echo.

set "ROOT_DIR=%~dp0.."
set "SLN=%ROOT_DIR%\HydraTentacle.sln"

echo [1/4] Tum solution bagimliliklari denetleniyor ve yukleniyor...
dotnet restore "%SLN%"
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [HATA] Solution bagimliliklari yuklenirken bir sorun olustu!
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo [2/4] HydraTentacle solution derleniyor...
dotnet build "%SLN%" -c Debug --no-restore
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [HATA] Solution derlenirken bir sorun olustu!
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo [3/4] WebApi ayri konsol penceresinde baslatiliyor...
start "HydraTentacle - WebApi" "%~dp01_Run_WebApi.bat" --no-build

echo.
echo [4/4] WebApi'nin hazir olmasi icin 4 saniye bekleniyor...
timeout /t 4 /nobreak >nul

echo Blazor UI ayri konsol penceresinde baslatiliyor...
start "HydraTentacle - Blazor UI" "%~dp02_Run_Blazor.bat" --no-build

echo.
echo =======================================================
echo   Servisler basariyla baslatildi!
echo   - WebApi : http://localhost:5132
echo   - Blazor : http://localhost:5121
echo =======================================================
echo.
