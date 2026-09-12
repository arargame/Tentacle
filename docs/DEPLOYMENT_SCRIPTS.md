# Tentacle — Dağıtım ve Çalıştırma Scriptleri Rehberi

Bu doküman, `Tentacle/Deploy/` dizininde yer alan `.bat` scriptlerinin işlevlerini, nasıl çalıştıklarını ve olası sorun giderme adımlarını açıklar.

---

## Script Listesi ve İşlevleri

### 1. `0_Run_All.bat` (Full Stack Başlatıcı)
- **Amaç:** Hem backend (`HydraTentacle.WebApi`) hem de frontend (`HydraTentacle.Blazor`) servislerini tek bir tıklamayla hazır hale getirip ayağa kaldırır.
- **İşlem Adımları:**
  1. `HydraTentacle.sln` için bağımlılıkları denetler ve geri yükler (`dotnet restore`).
  2. Solution'ı Debug modunda derler (`dotnet build`).
  3. WebApi'yi ayrı bir konsol penceresinde başlatır (`http://localhost:5132`).
  4. WebApi'nin initialize olması için 4 saniye bekler.
  5. Blazor uygulamasını ayrı bir konsol penceresinde başlatır (`http://localhost:5121`).

---

### 2. `1_Run_WebApi.bat` (WebApi Başlatıcı)
- **Amaç:** Yalnızca `HydraTentacle.WebApi` projesini bağımsız olarak başlatmak.
- **İşlem Adımları:**
  1. `HydraTentacle.WebApi.csproj` bağımlılıklarını geri yükler (`dotnet restore`).
  2. WebApi projesini derler (`dotnet build`).
  3. `http` launch profiliyle `http://localhost:5132` adresinde API sunucusunu başlatır (`dotnet run`).
- **Desteklenen Parametre:**
  - `--no-build`: Restore ve build adımlarını atlayıp doğrudan uygulamayı başlatır (örneğin `0_Run_All.bat` tarafından derlendikten sonra çağrıldığında hızlı açılış sağlar).

---

### 3. `2_Run_Blazor.bat` (Blazor UI Başlatıcı)
- **Amaç:** Yalnızca `HydraTentacle.Blazor` ön yüz projesini bağımsız olarak başlatmak.
- **İşlem Adımları:**
  1. `HydraTentacle.Blazor.csproj` bağımlılıklarını geri yükler (`dotnet restore`).
  2. Blazor projesini derler (`dotnet build`).
  3. `http` launch profiliyle `http://localhost:5121` adresinde Blazor Server uygulamasını başlatır (`dotnet run`).
- **Desteklenen Parametre:**
  - `--no-build`: Restore ve build adımlarını atlayıp doğrudan uygulamayı başlatır.

---

### 4. `3_Update_Database.bat` (EF Core Veri Tabanı Güncelleyici)
- **Amaç:** Entity Framework Core migration değişikliklerini veri tabanına uygulamak.
- **Çalıştırılan Komut:**
  ```cmd
  dotnet ef database update -p Source\HydraTentacle.Core -s Source\HydraTentacle.WebApi
  ```
- **Gereksinim:** `dotnet-ef` global CLI aracının yüklü olması gerekir (`dotnet tool install --global dotnet-ef`).

---

## Sorun Giderme (Troubleshooting)

1. **Port Çakışması (5132 veya 5121 dolu):**
   - Başka bir süreç bu portları kullanıyorsa komut satırından `netstat -ano | findstr 5132` ile PID tespit edilip sonlandırılabilir.
2. **Bağlantı Hatası (Blazor -> WebApi):**
   - Blazor'ın API'ye ulaşabilmesi için WebApi'nin `http://localhost:5132` adresinde açık ve dinliyor olması gerekir.
3. **Veri Tabanı Bağlantı Hatası:**
   - `Source\HydraTentacle.WebApi\appsettings.json` dosyasındaki `ConnectionStrings:DefaultConnection` parametresinin geçerli bir SQL Server (veya LocalDB) instance'ına işaret ettiğinden emin olun.
