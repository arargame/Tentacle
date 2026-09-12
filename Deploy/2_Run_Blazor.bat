@echo off
title HydraTentacle - Blazor UI
echo.
echo =======================================================
echo   HydraTentacle - Blazor UI Baslatici
echo =======================================================
echo.

set "ROOT_DIR=%~dp0.."
set "PROJECT=%ROOT_DIR%\Source\HydraTentacle.Blazor\HydraTentacle.Blazor.csproj"

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
echo [2/3] HydraTentacle.Blazor projesi derleniyor...
dotnet build "%PROJECT%" -c Debug --no-restore
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [HATA] Blazor UI derleme islemi basarisiz oldu!
    pause
    exit /b %ERRORLEVEL%
)

:RUN_APP
echo.
echo [3/3] Blazor UI baslatiliyor (http://localhost:5121)...
echo Tarayici otomatik olarak aciliyor...
start "" /min cmd /c "timeout /t 2 /nobreak >nul & start http://localhost:5121"
dotnet run --project "%PROJECT%" -c Debug --no-build --launch-profile "http"
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [HATA] Blazor UI calisirken bir hata ile durdu!
    pause
    exit /b %ERRORLEVEL%
)
pause
