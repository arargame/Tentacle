# Tentacle — Ayağa Kaldırma Rehberi

> Bu doküman `Tentacle` reposunu sıfırdan (veya bu makineden başka bir makinede) çalışır hale getirmek için gereken adımları ve proje/DLL bağımlılık haritasını içerir. Mimari detaylar için `AIRepos/Documentation/Hydra/*.md` dosyalarına bakın (özellikle `HydraCoreArchitecture.md`, `TableBFFDataFlow.md`, `TentacleRevivalNotes.md`, `Worklog-2026-07-19.md`).

## 1. Dizin Düzeni (şart)

Tentacle, `Hydra` ve `Hydra.WebApi` ile `Hydra.RazorClassLibrary` projelerine **relative path** üzerinden referans veriyor. Bu yüzden aşağıdaki klasör yapısı korunmalı — repo'lar birbirinin sibling'ı (kardeşi) olmalı:

```
AIRepos/
├── Hydra/                          (repo)
│   └── Hydra.csproj
├── Hydra.WebApi/                   (repo)
│   └── Hydra.WebApi.csproj
├── Hydra.RazorClassLibrary/        (repo)
│   └── Hydra.RazorClassLibrary/Hydra.RazorClassLibrary.csproj
├── Hydra.TestProject/              (repo)
├── Hydra.ConsoleApp/                (repo)
├── global.json                      ← SDK pin, AIRepos KÖKÜNDE
└── Tentacle/                        (repo, bu dosyanın bulunduğu yer)
    └── Source/
        ├── HydraTentacle.Core/
        ├── HydraTentacle.WebApi/
        └── HydraTentacle.Blazor/
```

`Tentacle` tek başına klonlanıp farklı bir yere konursa **derlenmez** — `Hydra`, `Hydra.WebApi`, `Hydra.RazorClassLibrary` repolarının da aynı üst klasörde (`AIRepos/`) sibling olarak durması gerekir.

## 2. Proje / DLL Bağımlılık Haritası

Her proje kendi `.csproj`'undaki `ProjectReference` ile bir öncekinin derlenmiş DLL'ini kullanıyor. Kim kimi referans alıyor:

```
Hydra.csproj  (net9.0)                       →  Hydra.dll (çekirdek kütüphane, bağımsız)
   │
   ├── Hydra.WebApi.csproj (net9.0)          →  Hydra.dll'i referans alır
   │                                             (generic MainController<T>, auth controller'ları)
   │
   ├── Hydra.RazorClassLibrary.csproj (net9.0) →  Hydra.dll'i referans alır
   │                                             (Blazor component seti, ApiClient)
   │
   ├── Hydra.TestProject.csproj (net9.0)     →  Hydra.dll'i referans alır (xUnit testleri)
   │
   └── Hydra.ConsoleApp.csproj (net10.0 ⚠)   →  Hydra.dll'i referans alır (deneme/araç projesi)

Tentacle/Source/HydraTentacle.Core.csproj (net9.0)
   → Hydra.dll                                (BaseObject<T>, Repository<T>, Service<T> vb.)
   → NuGet: Microsoft.EntityFrameworkCore.SqlServer

Tentacle/Source/HydraTentacle.WebApi.csproj (net9.0, Web SDK)
   → HydraTentacle.Core.dll                   (Request/RequestCategory modelleri, DbContext)
   → Hydra.dll
   → Hydra.WebApi.dll                          (MainController<T> mirası, auth controller'ları bedavaya gelir)

Tentacle/Source/HydraTentacle.Blazor.csproj (net9.0, Web SDK)
   → HydraTentacle.Core.dll                   (DTO'lar, entity tipleri)
   → Hydra.dll
   → Hydra.RazorClassLibrary.dll               (GenericListView, HydraGrid, ApiClient, CRUD component'leri)
```

**Not (⚠):** `Hydra.ConsoleApp` tek başına `net10.0`'ı hedefliyor, geri kalan her şey `net9.0`. Bu bir hataya yol açmaz (ConsoleApp Hydra'yı sadece tüketiyor) ama tutarsızlık — ileride net9'a çekilmesi önerilir.

`Tentacle/HydraTentacle.sln` içinde bu beş proje (`HydraTentacle.Core`, `HydraTentacle.WebApi`, `HydraTentacle.Blazor`, `Hydra`, `Hydra.WebApi`) tek solution altında toplanmış durumda; `Hydra.RazorClassLibrary` solution'a eklenmemiş ama `HydraTentacle.Blazor.csproj` ona doğrudan referans veriyor (derlemede otomatik dahil olur, Visual Studio'da ayrıca solution'a eklemek istersen elle eklemen gerekir).

## 3. Önkoşullar

- **.NET SDK 9.0.308** (`AIRepos/global.json` bunu pinliyor, `rollForward: latestPatch` — yani sadece 9.0.3xx yamaları kabul eder). Kontrol: `dotnet --list-sdks`
- **SQL Server** (LocalDB yeterli) — `HydraTentacle.WebApi/appsettings.json` içindeki `ConnectionStrings:DefaultConnection` ve `LogDbConnection` şu an **boş**, doldurulması lazım.
- Visual Studio 2022 (veya `dotnet` CLI) + EF Core tools (`dotnet tool install --global dotnet-ef` gerekebilir).

## 4. İlk Kurulum Adımları

1. **Bağlantı stringlerini doldur** — `Tentacle/Source/HydraTentacle.WebApi/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HydraTentacle;Trusted_Connection=True;",
     "LogDbConnection": "Server=(localdb)\\mssqllocaldb;Database=HydraTentacleLogs;Trusted_Connection=True;"
   }
   ```
   (Prod'da User Secrets / gerçek SQL Server kullanılmalı — appsettings.json'a şifre yazma.)

2. **Migration'ları uygula** (ilk kurulumda `DbInitializer` bunu otomatik yapar ama elle de tetiklenebilir):
   ```bash
   cd Tentacle/Source
   dotnet ef database update -p HydraTentacle.Core -s HydraTentacle.WebApi
   ```
   `DbInitializer` (bkz. `Hydra/Services/DbInitializer.cs`) migration geçmişi varsa `Database.Migrate()` çalıştırır. **Eski `EnsureCreated()` ile kurulmuş** bir DB'n varsa (migration geçmişi yok), `appsettings.json`'a bir kereliğine şunu ekle, DB'yi sıfırdan kurdur, sonra kaldır:
   ```json
   "Database": { "RecreateOnStartup": "true" }
   ```

3. **Derle** (solution kökü `AIRepos/Tentacle`):
   ```bash
   dotnet build Tentacle/HydraTentacle.sln
   ```
   Aynı anda `Hydra.sln` (AIRepos kökünde) ile core kütüphaneleri de derleyebilirsin ama zorunlu değil — Tentacle solution'ı zaten `ProjectReference` ile onları da derler.

4. **API'yi başlat**:
   ```bash
   dotnet run --project Tentacle/Source/HydraTentacle.WebApi
   ```
   - HTTP: `http://localhost:5132`
   - HTTPS: `https://localhost:7215`
   - Doğrulama: `GET http://localhost:5132/api/Request/Ping` → `"Pong"` dönmeli.

5. **Örnek veri üret** (DEBUG modunda açık):
   ```
   POST http://localhost:5132/api/Request/Seed/25
   ```
   Employee → Position → OrganizationUnit → RequestCategory → Request zincirini FK-güvenli şekilde otomatik kurar.

6. **Blazor'u başlat** (API çalışırken, ayrı terminalde):
   ```bash
   dotnet run --project Tentacle/Source/HydraTentacle.Blazor
   ```
   - HTTP: `http://localhost:5121` (varsayılan, tarayıcıyı otomatik açar)
   - HTTPS: `https://localhost:7238` / `http://localhost:5120`
   - Blazor'un API'ye baktığı adres `Program.cs` içinde sabit: `http://localhost:5132/api/`.

   **Port uyumu şart:** API tarafında CORS + origin-koruma middleware'i sadece şu üç origin'e izin veriyor: `http://localhost:5120`, `https://localhost:7238`, `http://localhost:5121`. Blazor'u farklı bir portta çalıştırırsan `Tentacle/Source/HydraTentacle.WebApi/Program.cs` içindeki **iki ayrı listeyi** (CORS policy + custom origin middleware) güncellemen gerekir.

7. **Gez**: `/` (landing) → `/Dashboard` → `/Request` (liste, filtre, sıralama, sayfalama) → Yeni Kayıt → Detay (master-detail collection'lar) → Düzenle/Sil. Diğer entity'ler için: `/Employee`, `/Position`, `/OrganizationUnit`, `/SystemUser`, `/Role`, `/Permission`, `/RequestCategory`, `/RequestCategoryResponsiblePosition` (hepsi aynı CRUD kalıbı: `Index` / `Create` / `Update/{id}` / `Details/{id}`).

## 5. Bilinen Kısıtlar / Sırada Ne Var

RAD (XAF-light) hedefi açısından bir entity'nin tam otomatik CRUD alması için gereken zincir tamam: `Entity : BaseObject<T>` → `XDTO : ViewDTO` (`LoadConfigurations()`) → generic `Repository`/`Service` → `XController : MainController<T>` → `XClient : ApiClient<X>` → `GenericListView`/`GenericCreateView`/`GenericEditView`/`GenericDetailsView`. Tentacle'da 9 entity bu zinciri kullanıyor (bkz. yukarıdaki Pages/Crud listesi).

Eksik kalanlar:
- **Auth akışı bağlı değil**: `SystemUser`/`Role`/`Permission` nesneleri ve `AuthenticationService`/`HydraAuthenticationStateProvider` hazır ama login sayfası + JWT alma + permission bazlı menü/route koruması henüz kurulmadı.
- **Dosya yükleme UI'ı yok**: `RequestAttachment` modeli ve `InputToUploadFile` alt yapısı var, `FileUploadComponent` yazılmadı.
- **Kategori→pozisyon otomatik atama** iş kuralı zayıf ("talep açılınca sorumlu pozisyona ata" mantığı yazılmalı).
- Test projesi (`Hydra.TestProject`) yeni Select sözleşmesi / RowDTO Id düzeltmesi için güncellenmedi.

## 6. Önce Bunu Kontrol Et — Commit Durumu

**Önemli:** Bu repodaki (ve `Hydra`, `Hydra.WebApi`, `Hydra.RazorClassLibrary` repolarındaki) 19-20 Temmuz oturumunda yapılan iş — Select sözleşmesi düzeltmesi, tüm generic CRUD component'leri, Tentacle'daki 9 entity'lik CRUD sayfaları, Dashboard — **henüz commit edilmemiş**, sadece diskte duruyor (`git status` ile kontrol et: `Pages/Crud/`, `Dashboard.razor` ve çok sayıda değişik dosya untracked/modified görünüyor). Ayrıca `Tentacle/.git/index.lock` adında eski bir kilit dosyası var; git komutları garip davranırsa (VS ve tüm git process'leri kapalıyken) bu dosyayı elle sil.

Önerilen sıra: (1) `dotnet build` ile derlemenin geçtiğini doğrula, (2) `index.lock`'u temizle, (3) `Hydra`, `Hydra.WebApi`, `Hydra.RazorClassLibrary`, `Tentacle` repolarındaki bekleyen değişiklikleri anlamlı commit'lere böl ve kaydet.
