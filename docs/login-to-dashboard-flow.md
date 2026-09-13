# Login → Dashboard Akışı: Mevcut Durum ve Bağlama Planı

Bu doküman tek bir soruyu cevaplıyor: *Tentacle'da bir kullanıcı `/Login`'e girip
"Log In"'e bastığında bugün gerçekte ne oluyor, gerçek kimlik doğrulamaya
bağlamak için hangi somut adımlar gerekiyor?* Mimarinin genel anlatımı (Hydra
core'dan sayfalara kadar `SystemUser`/`Role`/`Permission` hattı) için
`AIRepos/Hydra/Academy/05-Access-Management-Story/` bölümüne bakın — orası
"nasıl çalışıyor", bu doküman "bizim projede şu an ne durumda, sırada ne var".

## 1. Bugün gerçekte olan akış

1. **`/` (Home)** → herkese açık, `EmptyLayout` kullanır.
2. **`/Login`** (`Pages/Login.razor`) → yine `EmptyLayout`, Stitch tasarımının
   Blazor'a uyarlanmış hali. Form alanları (`_email`, `_password`) gerçek, şifre
   göster/gizle butonu gerçekten çalışıyor — ama **"Log In" butonunun arkasında
   hiçbir kimlik doğrulama çağrısı yok**:

   ```razor
   private void HandleLogin()
   {
       //TODO: Gerçek kimlik doğrulama henüz bağlı değil...
       Navigation.NavigateTo("Dashboard");
   }
   ```

   Yani bugün e-posta/şifre ne yazılırsa yazılsın (hatta boş bırakılıp `required`
   validasyonu atlatılsa bile) buton `/Dashboard`'a yönlendiriyor.
3. **`/Dashboard`** (`Pages/Dashboard.razor`) → yine `EmptyLayout`, ama
   **gerçek veriye bağlı**: `ApiClient<Request>`, `ApiClient<RequestCategory>`,
   `ApiClient<Employee>` üzerinden toplam talep sayısı, durum/öncelik
   kırılımları, kategori dağılımı (donut grafik) ve son talepler tablosu
   API'den çekiliyor. Tasarımdaki uydurma sayılar kullanılmamış — bkz. kod
   içindeki `@code` bloğu, `OnInitializedAsync`.
4. Soldaki menüden (`Layouts/NavMenu.razor`) `SystemUser`, `Role`, `Permission`
   dahil **her ekrana** kimlik doğrulaması olmadan gidilebiliyor —
   `MainLayout.razor`/`Routes.razor` route bazlı bir koruma uygulamıyor.

Kısacası: Dashboard'un kendisi sağlam (gerçek veri), ama önündeki kapı (Login)
süs — kimin girdiğine bakmıyor, ve arkasındaki hiçbir oda kilitli değil.

## 2. Hazır ama bağlanmamış altyapı

Bağlamak için sıfırdan yazılacak neredeyse hiçbir şey yok — parçaların hepsi
zaten var, sadece birbirine kablolanmamış:

| Parça | Konum | Ne yapıyor |
|---|---|---|
| Login endpoint | `Hydra.WebApi/Controllers/SystemUserController.cs` → `POST api/SystemUser/Login` | `[AllowAnonymous]`. Email/username + `IsActive` eşleşmesine bakar, kullanıcının rollerini (`GetRolesAsync`) ve izinlerini (`GetPermissionsAsync`) çeker, JWT üretir. |
| Token üretimi | `Hydra/AccessManagement/Jwt/JwtTokenManager.cs` | HS256, issuer `hydra-api`, audience `hydra-clients`. Secret `ICustomConfigurationService`'ten `"JwtSecretKey"` anahtarıyla okunuyor — bugün karşılığı yok, kod içi `"fallback-secret"`e düşüyor. |
| İstemci login çağrısı | `Hydra.RazorClassLibrary/.../Services/Authentication/AuthenticationService.cs` | `Login(LoginViewDTO)` → `POST api/SystemUser/Login`, dönen token'ı `ILocalStorageService`'e (`authToken` anahtarı) yazar, `HydraAuthenticationStateProvider.NotifyUserLogin` çağırır. |
| Auth state | `Hydra.RazorClassLibrary/.../Services/Authentication/HydraAuthenticationStateProvider.cs` | LocalStorage'daki JWT'yi decode edip `ClaimsPrincipal` üretir; Blazor'un `AuthenticationStateProvider` sözleşmesini karşılar. |
| DI kaydı | `Hydra.RazorClassLibrary/.../Utils/ServiceCollectionExtensions.cs` → `AddHydraRazorLibrary()` | `AddAuthorizationCore()`, `IAuthenticationService`, `HydraAuthenticationStateProvider` (hem kendisi hem `AuthenticationStateProvider` olarak) zaten kayıtlı. |

Yani `IAuthenticationService`'i `Login.razor`'a inject edip çağırmak
**teknik olarak tek satırlık bir iş** — ama üç yerde daha eksik var (aşağıda).

## 3. Neden "çalışmıyor" değil "bağlı değil"

Üç ayrı boşluk var, üçü de birbirinden bağımsız çözülmeli:

**a) WebApi: authentication middleware'i hiç yok.**
`HydraTentacle.WebApi/Program.cs` şu an `app.UseAuthorization()` çağırıyor ama
öncesinde `app.UseAuthentication()` yok, ve hiçbir yerde
`AddAuthentication().AddJwtBearer(...)` kaydı yok. Yani `MainController<T>`'a
veya herhangi bir controller'a bugün `[Authorize]` eklesen, ASP.NET Core
"hangi şemaya göre?" diye None şema hatası verir — JWT doğrulaması **çalışacak
bir altyapı üzerine oturmuyor**, sadece token *üretiliyor*, hiçbir yerde
*doğrulanmıyor*.

**b) Blazor: route bazlı koruma yok.**
`Routes.razor` düz:
```razor
<Router AppAssembly="typeof(Program).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="routeData" DefaultLayout="typeof(MainLayout)" />
        <FocusOnNavigate RouteData="routeData" Selector="h1" />
    </Found>
</Router>
```
`<CascadingAuthenticationState>` ve `<AuthorizeRouteView>`/`<NotAuthorized>`
yok. `HydraAuthenticationStateProvider` DI'da hazır olsa da, hiçbir component
`<AuthorizeView>` veya `[Authorize]` kullanmadığı için devre dışı kalıyor —
motor takılı ama kontağa basan yok.

**c) `SystemUserController.LoginAsync` şifreyi hiç kontrol etmiyor.**
Bu, "bağlanmamış" değil, gerçek bir **güvenlik açığı**: sorgu sadece
`su.Email == dto.UserNameOrEmailAddress || su.Name == dto.UserNameOrEmailAddress`
ve `su.IsActive` şartına bakıyor, `dto.Password` ile `PasswordHash`
karşılaştırması **hiç yapılmıyor**. Yani bugün doğru e-posta + herhangi bir
şifre (hatta boş şifre) ile token alınabilir. Bu, aşağıdaki bağlama planından
bağımsız, öncelikli bir düzeltme.

## 4. Bağlama planı (somut adımlar)

1. **Şifre kontrolünü ekle** (öncelik #1, güvenlik). `SystemUserController.LoginAsync`
   içinde `user` bulunduktan sonra, token üretmeden önce:
   ```csharp
   if (user == null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
   {
       response.SetSuccess(false).AddExtraMessage(/* ... */);
       return new JsonResult(response);
   }
   ```
   (Hydra'da hazır bir `PasswordHasher` yoksa `Microsoft.AspNetCore.Identity.PasswordHasher<T>`
   kullanılabilir — `SystemUser.PasswordHash` zaten `string?` olarak entity'de var.)

2. **WebApi: JWT bearer şemasını kaydet.** `HydraTentacle.WebApi/Program.cs`,
   `AddControllers()`'tan önce:
   ```csharp
   builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = true, ValidIssuer = "hydra-api",
               ValidateAudience = true, ValidAudience = "hydra-clients",
               ValidateLifetime = true,
               IssuerSigningKey = new SymmetricSecurityKey(
                   Encoding.UTF8.GetBytes(builder.Configuration["JwtSecretKey"]!))
           };
       });
   ```
   ve middleware sırasına (CORS'tan sonra, `UseAuthorization()`'dan önce)
   `app.UseAuthentication();` ekle. `appsettings.json`'a gerçek bir
   `JwtSecretKey` koy (repo'ya commit edilecek dosyaya değil — User Secrets
   veya ortam değişkeni).

3. **Blazor: `Login.razor`'ı gerçek servise bağla.**
   ```razor
   @inject Hydra.RazorClassLibrary.Services.Authentication.IAuthenticationService AuthService

   private async Task HandleLogin()
   {
       var result = await AuthService.Login(new LoginViewDTO
       {
           UserNameOrEmailAddress = _email,
           Password = _password
       });

       if (result != null)
           Navigation.NavigateTo("Dashboard");
       else
           _loginError = "E-posta veya şifre hatalı.";
   }
   ```
   (`@onsubmit="HandleLogin"` zaten `async Task` dönen metodu destekler, imzayı
   `void`'den `Task`'a çevirmek yeterli.)

4. **Route koruması ekle.** `App.razor`'da `<Routes>`'u
   `<CascadingAuthenticationState>` ile sar, `Routes.razor`'da `<RouteView>`
   yerine `<AuthorizeRouteView>` + `<NotAuthorized>` kullan. Genel kuralı
   (`Home`/`Login`/`Dashboard` hariç her şey kimlik ister) her sayfaya tek tek
   `[Authorize]` yazmak yerine `Routes.razor` seviyesinde tanımlamak bakımı
   kolaylaştırır.

5. **Menüyü role göre daralt.** `NavMenu.razor`'daki "Administration" grubunu
   (`SystemUser`/`Role`/`Permission`) `<AuthorizeView Roles="...">` ile sar —
   bugün herkes görüyor, oysa `SystemUserController.LoginAsync` zaten
   `ClaimTypes.Role` claim'lerini (rol Id'leri) token'a yazıyor.

6. **Doğrula.** `api/SystemUser/Login`'i Postman'de dene → dönen token'ı
   `jwt.io`'da aç, claim'leri kontrol et → Blazor'da giriş yap → LocalStorage'da
   `authToken`'ın yazıldığını, sayfa yenilemesinde oturumun düştüğünü/düşmediğini
   (`HydraAuthenticationStateProvider.GetAuthenticationStateAsync`) doğrula.

## 5. Dosya haritası

| Sorumluluk | Yol |
|---|---|
| Login sayfası | `Source/HydraTentacle.Blazor/Pages/Login.razor` |
| Dashboard sayfası | `Source/HydraTentacle.Blazor/Pages/Dashboard.razor` |
| Router / kabuk | `Source/HydraTentacle.Blazor/Routes.razor`, `App.razor` |
| Sol menü | `Source/HydraTentacle.Blazor/Layouts/NavMenu.razor` |
| Login endpoint + JWT üretimi | `Hydra.WebApi/Controllers/SystemUserController.cs`, `Hydra/AccessManagement/Jwt/JwtTokenManager.cs` |
| İstemci auth servisleri | `Hydra.RazorClassLibrary/.../Services/Authentication/*.cs` |
| WebApi pipeline | `Source/HydraTentacle.WebApi/Program.cs` |
| Derin mimari anlatım | `AIRepos/Hydra/Academy/05-Access-Management-Story/` |
