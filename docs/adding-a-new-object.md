# Yeni Bir Obje (Entity) Ekleme Adımları

Bu doküman, Hydra tabanlı bir uygulamaya — burada Tentacle'a — **yeni bir nesnenin uçtan uca
nasıl ekleneceğini** anlatır: entity tanımından veritabanı şemasına, REST ucundan Blazor
ekranına, menüden izne kadar.

Örnek olarak uydurma bir `Product` kullanmıyoruz. Anlatılan her adımın karşılığı
**gerçekten yapılmış** bir işte var: `Hydra.MailManagement` modülünün Tentacle'a
entegrasyonu (`Mail`, `MailTemplate`, `MailAttachment`, `BaseObjectMail`). Her bölümün
sonunda "bu adım mail modülünde neye karşılık geldi" notu var; dosyaları açıp
karşılaştırabilirsiniz.

> **İki senaryo var, karıştırmayın.**
> **(A) Uygulamanın kendi nesnesi** — `Request` gibi, `HydraTentacle.Core` içinde yaşar.
> **(B) Paylaşılan bir modülün nesnesi** — `Mail` gibi, `Hydra/` altında ayrı bir kütüphanede
> yaşar ve birden çok uygulama tarafından kullanılır.
> Adımların çoğu ortak; ayrıştıkları yerler açıkça işaretlendi.

---

## Adım 0 — Önce şuna karar verin: entity nerede yaşayacak?

| Soru | Cevap "evet" ise |
|---|---|
| Bu nesne yalnızca bu uygulamanın işine mi yarar? | (A) `HydraTentacle.Core/Models/` |
| Başka bir Hydra uygulaması da bunu aynen kullanır mı? | (B) `Hydra/Hydra.<Modül>/Entities/` |

Yanlış tarafa koymanın bedeli simetrik değildir: uygulamaya özel bir şeyi çekirdeğe koymak
onu herkesin taşıması gereken bir yüke çevirir; paylaşılabilir bir şeyi uygulamada tutmak ise
ikinci uygulamada kopyala-yapıştıra zorlar. Şüphedeyseniz (A) ile başlayın — taşımak, geri
almaktan kolaydır.

**Ne olursa olsun ekran yapılandırması (ViewDTO) tek istisnadır** ve entity'nin yanında durur.
Mail modülünde `MailViewDTO`, `MailAttachmentViewDTO` kütüphanenin içindedir; çünkü "Body
listede görünmesin", "Status enum etiketiyle çizilsin" kararları modülün kendi bilgisidir,
uygulamaya özel değil.

---

## Adım 1 — Entity: `BaseObject<T>`'den türetin

```csharp
public class MailTemplate : BaseObject<MailTemplate>
{
    [Required]
    public string Code { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public virtual ICollection<Mail> Mails { get; set; } = new List<Mail>();
}
```

`BaseObject<T>` size şunları **hazır** verir; bunları tekrar tanımlamayın:

| Üye | Kim doldurur |
|---|---|
| `Id` | Ctor — `Guid.CreateVersion7()` (sıralı UUID; index parçalanmasını önler) |
| `Name`, `Description` | Siz / kullanıcı |
| `AddedDate`, `ModifiedDate` | **`Repository<T>`**, `HydraDbContext.SaveChangesAsync` değil |
| `IsActive` | Varsayılan `true` |
| `RowVersion` | `[Timestamp]` — EF eşzamanlılık denetimi |

> ⚠️ **`Id`'yi elle `Guid.NewGuid()` yapmayın.** UUIDv7 sıralı olduğu için clustered index'e
> sona ekler; `NewGuid()` rastgeledir ve sayfa bölünmesi üretir.

> ⚠️ **Repository katmanını atlarsanız `AddedDate`/`ModifiedDate` boş kalır.** Damgayı
> `Repository<T>.AddAsync` vuruyor (`Hydra/DAL/Core/Repository.Command.cs`), `SaveChangesAsync`
> değil. Seed kodu gibi doğrudan `DbContext` kullanan yerlerde bu iki alanı elle yazın.

**FK ve Navigation alanları için kurallar:**
- **FK isimlendirmesi:** `{RightTableName}Id` (ör. `PositionId`, `MailTemplateId`). ViewDTO'daki `SetConfigurationsForNavigations` bu ismi varsayar; uyarsanız tek satırla navigation kurarsınız.
- ⚠️ **Navigation property'ler MUTLAKA nullable (`?`) olmalıdır:**
  ```csharp
  public Guid PositionId { get; set; }

  [ForeignKey("PositionId")]
  public Position? Position { get; set; } // ASLA `public Position Position { get; set; } = null!;` yapmayın!
  ```
  **Neden?** ASP.NET Core nullable reference types (`<Nullable>enable</Nullable>`) açıkken non-nullable referans tiplerini örtük olarak `[Required]` kabul eder. Create ve Update isteklerinde form sadece `PositionId` (FK) yollar, `Position` nesnesi `null` gelir. Eğer navigation property nullable (`?`) yapılmazsa ASP.NET Core model doğrulayıcısı `400 Bad Request` ("The Position field is required.") hatası döndürür.

- ⚠️ **`IsActive` Alanını Tekrar Tanımlamayın:**
  `BaseObject<T>` zaten `IsActive` alanını sağlar ve ViewDTO'daki `SetConfigurationsForBaseObjectMembers()` bunu standart bir toggle olarak forma ekler. Entity içerisine `IsActiveEmployee` gibi mükerrer alanlar ekleyip ViewDTO'da ayrıca konfigüre ederseniz ekranda aynı isimde iki adet "Active" kontrolü belirir.


📌 *Mail modülünde:* `Hydra/Hydra.MailManagement/Entities/` altında dört entity —
`Mail`, `MailTemplate`, `MailAttachment`, `BaseObjectMail`.

---

## Adım 2 — Şema: `DbSet` + `OnModelCreating`

Entity nerede yaşarsa yaşasın, **şema kararı tüketen uygulamanın `DbContext`'inde verilir.**
Modül kendi `DbContext`'ini getirmez; böylece mailler uygulamanın öz veritabanında, aynı
transaction sınırları içinde durur.

```csharp
public DbSet<Mail> Mail { get; set; }
public DbSet<MailTemplate> MailTemplate { get; set; }
```

> ⚠️ **`DbSet` adı TEKİL olmalı.** Hydra'nın `TableService`'i tablo adını controller/entity
> adından üretiyor: `"Mail"` controller → `"Mail"` tablosu. `Mails` derseniz ham SQL tarafı
> tabloyu bulamaz.

`OnModelCreating` içinde üç şeye karar verin:

**1. Silme davranışı.** Varsayılana güvenmeyin, niyetinizi yazın:

```csharp
modelBuilder.Entity<Mail>()
    .HasOne(x => x.MailTemplate).WithMany(t => t.Mails)
    .HasForeignKey(x => x.MailTemplateId)
    .OnDelete(DeleteBehavior.Restrict);   // şablon silinince mailler GİTMESİN

modelBuilder.Entity<MailAttachment>()
    .HasOne(x => x.Mail).WithMany(m => m.Attachments)
    .HasForeignKey(x => x.MailId)
    .OnDelete(DeleteBehavior.Cascade);    // ek, mailin parçasıdır
```

Ayrım şu soruyla netleşir: **"Sağ taraf silinince soldakinin ayrı bir varlık olarak anlamı
kalıyor mu?"** Gönderim kaydı denetim verisidir, şablon silindi diye kaybolmamalı → `Restrict`.
Ek, mailin parçasıdır, tek başına anlamsız → `Cascade`.

**2. Tekillik.** Kod içinden `GetByCodeAsync("WELCOME_USER")` gibi tek kayıt bekleyen bir
erişim varsa, tekilliği veritabanında zorlayın — yoksa hangisinin döneceği rastgele olur:

```csharp
modelBuilder.Entity<MailTemplate>().HasIndex(x => x.Code).IsUnique();
```

**3. Index.** Liste ekranının filtrelediği, dashboard'un saydığı kolonlar:
`HasIndex(x => x.Status)`.

📌 *Mail modülünde:* `HydraTentacle.Core/DAL/Contexts/TentacleDbContext.cs` →
`ConfigureMailManagement(ModelBuilder)`.

### Polimorfik bağ (FK kurulamayan durum)

`BaseObjectMail` bir maili sistemdeki **herhangi** bir kayda bağlar: `EntityType` + `EntityId`.
Hedef tablo çalışma anında belli olduğu için FK **yoktur ve olamaz**. Bedeli: hedef kayıt
silinince bağ öksüz kalır. Karşılığı: modül hiçbir uygulamanın tablolarını tanımak zorunda
kalmaz. Bu takas bilinçlidir; sorgulanabilir kalması için ikili index konur:

```csharp
entity.HasIndex(x => new { x.EntityType, x.EntityId });
```

---

## Adım 3 — Migration

```powershell
cd Tentacle\Source\HydraTentacle.WebApi
dotnet ef migrations add AddHydraMailManagement --project ..\HydraTentacle.Core
dotnet ef database update --project ..\HydraTentacle.Core
```

veya `Deploy/3_Update_Database.bat`.

> ⚠️ `DbInitializer.InitializeAsync` açılışta `Database.Migrate()` çağırıyor. Yani
> **migration dosyasını üretmezseniz tablolar oluşmaz** ve yeni ekranlar "veri yüklenemedi"
> der. Bu adım opsiyonel değildir.

---

## Adım 4 — ViewDTO: ekranın sözleşmesi

Entity "veri ne", ViewDTO "kullanıcı bunu nasıl görecek". `[RegisterAsViewDTO(nameof(X))]`
ile işaretlenir ve `LoadConfigurations()` içinde kolonlar tarif edilir.

```csharp
[RegisterAsViewDTO(nameof(Mail))]
public class MailViewDTO : ViewDTO
{
    public string Subject { get; set; } = string.Empty;
    public MailStatus Status { get; set; } = MailStatus.Draft;

    public Guid? MailTemplateId { get; set; }
    public string MailTemplate_Code { get; set; } = string.Empty;   // join kolonu
    public Guid MailTemplate_Id { get; set; }

    public MailViewDTO() => SetControllerName("Mail");

    public override DTO LoadConfigurations()
    {
        SetConfigurationsViaStringPropertyInfo(
            propertyInfo: ReflectionHelper.GetPropertyOf<MailViewDTO>(x => x.Subject),
            displayName: "Subject",
            attributeToFilter: new AttributeToFilter(nameof(ContainsFilter)));

        SetConfigurationsViaEnumPropertyInfo(
            propertyInfo: ReflectionHelper.GetPropertyOf<MailViewDTO>(x => x.Status),
            displayName: "Status");

        // Guid yerine okunabilir kolon + form ekranında dropdown
        SetConfigurationsForNavigations(
            leftTableKeyName: "MailTemplateId",
            rightTableName: "MailTemplate",
            columnNameToDisplay: "Code",
            displayName: "Template");

        SetConfigurationsForBaseObjectMembers();

        return this;
    }
}
```

Doğru araç seçimi:

| Kolon tipi | Çağrı |
|---|---|
| string | `SetConfigurationsViaStringPropertyInfo` |
| enum | `SetConfigurationsViaEnumPropertyInfo` (dropdown + etiket) |
| bool | `SetConfigurationsViaBooleanPropertyInfo` |
| tarih / sayı / özel | `SetConfigurationsViaPropertyInfo` + elle `IConfiguration` listesi |
| FK | `SetConfigurationsForNavigations` |
| `Id/Name/AddedDate/...` | `SetConfigurationsForBaseObjectMembers` (bir kez, sonda) |

**ViewDTO'ya her alanı koymayın.** İki gerçek örnek:

- `MailAttachment.Data` (`byte[]`) DTO'da **yok**. 20 satırlık bir liste yüzlerce MB'lık bir
  `TableDTO` üretirdi. Dosyanın kendisi ayrı bir uçtan alınır
  (`GET api/MailAttachment/Download/{id}`).
- `Mail.Body` listede **yok**, yalnızca Create/Edit/Details'te `TextArea`. HTML gövde bir
  grid satırını onlarca satır yüksekliğe çıkarırdı.

Hangi görünüme hangi konfigürasyonu verdiğiniz, o kolonun o ekranda olup olmayacağını
belirler: `ListViewConfiguration` yoksa listede çıkmaz, `CreateViewConfiguration` yoksa
oluşturma formunda çıkmaz.

### ViewDTO'yu kaydettirmeyi unutmayın

`AddHydraDependencies`, ViewDTO taraması için **yalnızca** Hydra core ve `Hydra.WebApi`
assembly'lerine bakar — verdiğiniz ek assembly parametresini bu iş için kullanmaz. Uygulamanın
ve modüllerin ViewDTO'ları elle yüklenir:

```csharp
ViewDTORegistryLoader.LoadAllViewDTOs(typeof(RequestModel).Assembly);
ViewDTORegistryLoader.LoadAllViewDTOs(typeof(MailViewDTO).Assembly);
```

Unutulursa hata almazsınız — ekran sessizce çıplak entity kolonlarına düşer: navigation'lar
Guid olarak, enum'lar sayı olarak görünür, gizlemek istediğiniz `Body` geri gelir. Yeni bir
ekran "neden bu kadar ham görünüyor" diyorsanız önce buraya bakın.

📌 *Mail modülünde:* `HydraTentacle.WebApi/Extensions/ServiceCollectionExtensions.cs`.

---

## Adım 5 — Servis: çoğu zaman yazmanız gerekmez

DI'da `IService<>` **açık generic** olarak kayıtlıdır:

```csharp
services.AddScoped(typeof(IService<>), typeof(Service<>));
```

Yani `IService<MailAttachment>` hiçbir şey yazmadan çalışır — CRUD, filtreleme, sayfalama,
cache, audit hepsi gelir. **Kendi servisinizi ancak iş kuralınız varsa yazın.**

```csharp
[RegisterAsService(typeof(IService<Mail>))]
public class MailService : Service<Mail>
{
    public MailService(ServiceInjector injector) : base(injector) { }

    public async Task<IResponseObject> SendAsync(Guid mailId, CancellationToken ct = default)
    { /* Queued → gönderim → Sent/Failed */ }
}
```

`[RegisterAsService(typeof(IService<Mail>))]` ile kapalı generic kaydı açık generic kaydın
önüne geçer. Bunun çalışması için **assembly'nin taranıyor olması** gerekir (Adım 6).

> `MailAttachment` ve `BaseObjectMail`'in servisi **yoktur** — iş kuralları olmadığı için.
> Boş bir `MailAttachmentService` yazmak soyutlama değil, gürültüdür.

> Servisler birbirini somut tip üzerinden çağırır (`injector.GetServiceLazy<MailTemplateService>()`).
> Bu yüzden attribute tarayıcısı hem arayüzü hem somut tipi kaydeder ve ikisini **aynı**
> scoped instance'a bağlar; iki ayrı kayıt iki ayrı LRU cache demek olurdu.

---

## Adım 6 — Controller ve DI kaydı

**(A) Uygulamanın kendi nesnesi:**

```csharp
[ApiController]
[Route("api/[controller]")]
public class MailAttachmentController : MainController<MailAttachment>
{
    public MailAttachmentController(IControllerInjector injector) : base(injector) { }
}
```

`MainController<T>` hazır uçları verir: `Select`, `Details`, `Get`, `Create`, `Update`,
`Delete`, `DeleteRange`, `LinkToCollection`, `UnlinkFromCollection`.

> ⚠️ Bir base action'ı ezecekseniz base'i `virtual` yapıp `override` kullanın. `new` ile
> gizlerseniz MVC aynı rotada iki action görür ve `AmbiguousMatchException` fırlatır.

**(B) Modül controller'ları — kopyalamayın, bağlayın:**

```csharp
builder.Services.AddControllers()
    .AddApplicationPart(typeof(MainController<>).Assembly)
    .AddHydraMailManagementControllers();   // ApplicationPart
```

Modülün `MvcBuilderExtensions`'ı controller assembly'sini ApplicationPart olarak ekler.
Böylece uygulamada tek satır mail controller kodu olmaz; modül güncellenince uçlar da güncellenir.

**DI kayıtları — sıra önemli:**

```csharp
// 1) Modülün kendi kayıtları (FluentEmail/SMTP + somut servisler)
builder.Services.AddHydraMailManagement(builder.Configuration);

// 2) Hydra bağımlılıkları; modül assembly'si [RegisterAsService] taraması için verilir
builder.Services.AddHydraDependencies(
    builder.Configuration,
    typeof(Request).Assembly,
    typeof(Mail).Assembly);
```

Modül kaydı **önce** çağrılır: attribute tarayıcısı "zaten kayıtlı mı" diye baktığı için
somut tip tek kez kaydolur, scope başına tek instance kalır. Ters sırada iki kayıt oluşurdu.

---

## Adım 7 — Konfigürasyon

Modülün ihtiyacı olan ayar bloğunu `appsettings.json`'a ekleyin:

```json
"MailSettings": {
  "Host": "localhost",
  "Port": 25,
  "EnableSsl": false,
  "DefaultFromEmail": "noreply@tentacle.com",
  "DefaultFromName": "Tentacle"
}
```

> Geliştirmede gerçek e-posta göndermemek için Papercut / MailHog / Mailtrap gibi yerel bir
> SMTP yakalayıcı kullanın (`Host: "localhost"`, `Port: 25`).

> ⚠️ `ConnectionStrings` değerlerine dokunmayın; onlar makineye özeldir ve repoda boş durur.

---

## Adım 8 — Blazor sayfaları

Her nesne için dörtlü: `Index` / `Details` / `Create` / `Edit`. Sayfalar
`HydraTentacle.Blazor/Pages/Crud/<Entity>/` altında yaşar ve neredeyse tamamen tek satırdır.

```razor
@page "/Mail"
@attribute [HydraRequirePermission("Mail", "Select")]
@using Hydra.MailManagement.Entities

<PageTitle>Mails</PageTitle>

<GenericListView T="Mail" Client="Client" Title="Mails" />

@code {
    [Inject] public ApiClient<Mail> Client { get; set; } = default!;
}
```

Rota konvansiyonu — generic bileşenlerin ürettiği linkler bunu varsayar, uyun:

| Sayfa | Rota | İzin action'ı |
|---|---|---|
| Index | `/<Entity>` | `Select` |
| Details | `/<Entity>/Details/{Id:guid}` | `Details` |
| Create | `/<Entity>/Create` | `Create` |
| Edit | `/<Entity>/Update/{Id:guid}` | `Update` |

`ApiClient<T>` de açık generic kayıtlıdır (`services.AddScoped(typeof(ApiClient<>))`), ayrıca
bir şey yazmanız gerekmez. Tek koşul: Blazor projesinin entity tipini görebilmesi için ilgili
projeye `ProjectReference` eklenmiş olması.

### İlişkileri ekrana taşımak: `CollectionViewSection`

`GenericDetailsView` içine konan her `CollectionViewSection` bir sekme olur. **İki mod var ve
seçim ilişkinin doğasına bağlı:**

**Basit FK modu** — bağımlı kayıtlar (ek, bağlı kayıt, alt kalem). Bunlar ana kaydın parçası
olarak doğar, dolayısıyla "New" akışı doğrudur:

```razor
<GenericDetailsView T="Mail" Client="Client" Id="@Id" Title="Mail Details">
    <CollectionViewSection Title="Attachments"
                           Controller="MailAttachment"
                           ForeignKeyName="MailId"
                           ParentId="@Id" />
</GenericDetailsView>
```

**Cross-table (Link/Unlink) modu** — çoka-çok köprüsü. Burada iki taraf da bağımsız var olur,
yapılan iş "bağlamak"tır; `RightKeyName`/`RightTableName` verildiğinde başlıkta çoklu seçim
modalı açan **Link**, satırlarda **Unlink** çıkar:

```razor
<CollectionViewSection Title="Permissions"
                       Controller="RolePermission"
                       ForeignKeyName="RoleId"
                       ParentId="@Id"
                       RightKeyName="PermissionId"
                       RightTableName="Permission"
                       RightDisplayColumn="Name" />
```

Yanlış modu seçmenin belirtisi: kullanıcıdan çoka-çok köprü kaydını **elle form doldurarak**
açmasını istiyorsanız, cross-table moduna geçmeniz gerekiyordur.

---

## Adım 9 — Menü ve Dashboard

Menü kullanıcının izinlerine göre çizilir; yeni modülü aynı kalıpla ekleyin
(`Layouts/NavMenu.razor`):

```razor
@if (CanSeeAny(auth.User, "Mail", "MailTemplate", "MailAttachment"))
{
    <div>
        <h3 class="tnt-nav__group-title">Mail Management</h3>
        <ul class="tnt-nav__list">
            @if (CanSee(auth.User, "Mail"))
            {
                <li><NavLink class="tnt-nav__link" href="Mail">
                    <i class="bi bi-envelope"></i> Mails</NavLink></li>
            }
        </ul>
    </div>
}
```

> Bu bir **güvenlik önlemi değildir** — kullanıcıyı 403 duvarına yürütmeme nezaketidir.
> Asıl karar her istekte sunucuda verilir.

`Pages/Dashboard.razor` kendi kabuğunu taşır (Tailwind, ayrı sidebar); oraya hem menü grubunu
hem özet kartlarını eklemek gerekir. Sayaçlar filtreli `TableDTO` ile alınır:

```csharp
var table = new TableDTO("Mail") { PageSize = 1, PageNumber = 1, ViewType = ViewType.ListView };
table.AlterOrAddMetaColumn(
    MetaColumnDTO.CreateColumnDTOWithEqualFilter("Status", null, (int)MailStatus.Failed,
                                                 useFilterToCreateViewComponent: false));
var count = (await MailApi.SelectAsync(table))?.FilteredTotalRecordsCount ?? 0;
```

> ⚠️ Bu desen statü başına bir istek atar (N+1). Kayıt azken görünmez, ölçeklenmez. Kalıcı
> çözüm API'ye tek çağrıda group-by dönen bir istatistik ucu eklemektir.

> Mail özeti `try/catch` içinde: migration atılmamışsa tablolar yoktur ve sorgu hata verir —
> bu durumda tüm dashboard'un çökmesi yerine kartlar 0 kalır.

---

## Adım 10 — İzinler

Sistem **deny-by-default**'tur: eşleşen bir `Permission` kaydı yoksa istek reddedilir. Üç kapı
vardır — `[AllowAnonymous]`, `[HydraAllowAuthenticated]` (kimlik yeter, özel izin aranmaz) ve
eşleşen bir `Permission`.

İzin kayıtlarını elle açmanız gerekmez. Açılışta tarayıcı controller'ları gezip eksik
`Permission` satırlarını yazar — **yeni modülün assembly'sini vermeyi unutmayın**:

```csharp
Hydra.Services.DbInitializer.InitializeAsync<TentacleDbContext>(app.Services, app.Configuration);

app.SyncHydraPermissions(
    typeof(Request).Assembly,
    typeof(MailController).Assembly);
```

Tarayıcı idempotenttir: yalnızca yeni uçları ekler, mevcut kayıtlara ve rol bağlantılarına
dokunmaz. `DbInitializer`'dan **sonra** çağrılmalıdır (tablolar oluşmadan izin yazılamaz).

Sonra: **Role → Details → Permissions** sekmesinden **Link** ile ilgili uçları role bağlayın.
Kullanıcı o rolü aldığında menüde grup belirir, sayfalar açılır.

Beş katmanın tamamı için bkz. Academy 5.5, 5.7 ve 5.8.

---

## Kontrol listesi

```
[ ] 1  Entity : BaseObject<T>, FK adı {RightTable}Id
[ ] 2  DbSet (TEKİL ad) + OnModelCreating: DeleteBehavior / unique index / index
[ ] 3  dotnet ef migrations add ... && database update
[ ] 4  ViewDTO + [RegisterAsViewDTO] + ViewDTORegistryLoader.LoadAllViewDTOs(assembly)
[ ] 5  Servis — yalnızca iş kuralı varsa; [RegisterAsService(typeof(IService<X>))]
[ ] 6  Controller : MainController<X>  (modülse ApplicationPart ile bağla)
       DI sırası: modül kaydı ÖNCE, AddHydraDependencies SONRA (assembly'yi geç)
[ ] 7  appsettings.json — modül ayar bloğu (ConnectionStrings'e DOKUNMA)
[ ] 8  Pages/Crud/<Entity>/{Index,Details,Create,Edit}.razor + [HydraRequirePermission]
       ProjectReference: Blazor projesi entity tipini görebiliyor mu?
[ ] 9  NavMenu.razor grubu + Dashboard.razor (sidebar + özet kartları)
[ ] 10 app.SyncHydraPermissions(..., yeniAssembly) → Role/Details'ten Link
```

---

## Sık yapılan hatalar

| Belirti | Sebep |
|---|---|
| Ekran çıplak: Guid'ler, enum sayıları, gizlenmesi gereken kolonlar görünür | ViewDTO assembly'si `ViewDTORegistryLoader`'a verilmemiş |
| "Veri yüklenemedi" / SQL'de tablo yok | Migration üretilmemiş (Adım 3) |
| `No service for type '...Service' has been registered` | Assembly `AddHydraDependencies`'e geçilmemiş |
| Sayfa açılıyor ama 403 | Uç için `Permission` kaydı var, role bağlanmamış |
| Sayfa hiç açılmıyor, login'e atıyor | `[HydraRequirePermission]` eşleşmiyor ya da token süresi dolmuş |
| `AmbiguousMatchException` | Base action `new` ile gizlenmiş, `override` yerine |
| Liste ekranı çok yavaş / dev yanıt | `byte[]` ya da uzun metin kolonu ViewDTO'da bırakılmış |
| `AddedDate` boş | Repository katmanı atlanmış, doğrudan `DbContext` kullanılmış |

---

## İlgili dokümanlar

- `docs/README.md` — proje mimarisi, portlar, hızlı başlangıç
- `docs/login-to-dashboard-flow.md` — Login → JWT → Dashboard akışı
- `Hydra/Hydra.MailManagement/INTEGRATION_PLAYBOOK.md` — mail modülünün kendi entegrasyon kılavuzu
- `Hydra/Academy/05-Access-Management-Story/` — beş katmanlı yetkilendirme mimarisi
- `Hydra/Academy/04-Sample-Project-Book/` — sıfırdan örnek modül kitabı
