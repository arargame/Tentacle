@echo off
title HydraTentacle - WebApi
echo.
echo =======================================================
echo   HydraTentacle - WebApi Baslatici
echo =======================================================
echo.

set "ROOT_DIR=%~dp0.."
set "PROJECT=%ROOT_DIR%\Source\HydraTentacle.WebApi\HydraTentacle.WebApi.csproj"

if "%1"=="--no-build" goto RUN_APP

echo [1/3] NuGet paket bagimliliklari denetleniyor ve yukleniyor...
dotnet restore "%PROJECT%"
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [HATA] Bagimliliklar yuklenirken bir sorun olustu!
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo [2/3] HydraTentacle.WebApi projesi derleniyor...
dotnet build "%PROJECT%" -c Debug --no-restore
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [HATA] WebApi derleme islemi basarisiz oldu!
    pause
    exit /b %ERRORLEVEL%
)

:RUN_APP
echo.
echo [3/3] WebApi baslatiliyor (http://localhost:5132)...
dotnet run --project "%PROJECT%" -c Debug --no-build --launch-profile "http"
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [HATA] WebApi calisirken bir hata ile durdu!
    pause
    exit /b %ERRORLEVEL%
)
pause
