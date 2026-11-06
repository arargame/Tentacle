# HydraTentacle — Dokümantasyon

HydraTentacle, kurumsal bir organizasyon içinde talep (Request), birim (Unit) ve pozisyon (Position) hiyerarşisini ve iş akışlarını yönetmek üzere geliştirilmiş bir **İş ve Talep Takip Sistemi**'dir (Work Tracking System). .NET 9 tabanlıdır ve **Hydra** framework mimarisi üzerine inşa edilmiştir.

---

## 1. Proje Mimarisi ve Çözüm Bileşenleri

HydraTentacle çok katmanlı ve modüler bir mimariye sahiptir:

| Bileşen | Açıklama |
|---|---|
| **HydraTentacle.Core** | Domain modelleri (`Request`, `RequestCategory`, `Unit`, `Position`), DTO'lar, Repository ve Data Context katmanı. |
| **HydraTentacle.WebApi** | REST API uçlarını barındıran servis katmanı. Hydra altyapısındaki generic `MainController<T>`, kimlik doğrulama/yetkilendirme ve loglama altyapısını kullanır. |
| **HydraTentacle.Blazor** | Modern Blazor Server / Web bileşenleri. Hydra Razor Class Library (`HydraGrid`, `GenericListView`, generic CRUD görünüşleri) ile zenginleştirilmiş kullanıcı arayüzü. |

---

## 2. Kardeş Repo (Sibling) Bağımlılıkları

Tentacle, relative path'ler üzerinden `Hydra` framework kütüphanelerini doğrudan referans alır. Bu nedenle `AIRepos` kök dizininde aşağıdaki kardeş klasör yapısı korunmalıdır:

```
AIRepos/
├── Hydra/                          (Core çekirdek kütüphane)
├── Hydra.WebApi/                   (Generic WebApi controller'ları & middleware)
├── Hydra.RazorClassLibrary/        (Generic Blazor bileşenleri, grid & CRUD)
├── Hydra.TestProject/              (xUnit testleri)
├── Hydra.ConsoleApp/               (Konsol araçları)
├── global.json                     (SDK sürüm kilidi: .NET 9)
└── Tentacle/                       (Bu proje)
    ├── Deploy/                     (Hızlı başlatma ve dağıtım scriptleri)
    ├── docs/                       (Detaylı dokümantasyon)
    └── Source/
        ├── HydraTentacle.Core/
        ├── HydraTentacle.WebApi/
        └── HydraTentacle.Blazor/
```

---

## 3. Port ve URL Yapılandırması

| Servis | Protokol | URL | Açıklama |
|---|---|---|---|
| **HydraTentacle.WebApi** | HTTP | `http://localhost:5132` | REST API servisi (`/api/...`) |
| **HydraTentacle.WebApi** | HTTPS | `https://localhost:7215` | Güvenli API profili |
| **HydraTentacle.Blazor** | HTTP | `http://localhost:5121` | Web Kullanıcı Arayüzü (UI) |
| **HydraTentacle.Blazor** | HTTPS | `https://localhost:7238` | Güvenli Web UI profili |

> Blazor ön yüzü varsayılan olarak WebApi ile `http://localhost:5132/api/` adresi üzerinden haberleşir.

---

## 4. Hızlı Başlangıç

Sistemi en hızlı şekilde ayağa kaldırmak için `Deploy/` klasöründeki scriptleri kullanabilirsiniz:

1. **Her Şeyi Birlikte Başlatmak:**
   `Deploy/0_Run_All.bat` dosyasını çalıştırın. Bağımlılıkları kontrol eder, projeleri derler ve WebApi ile Blazor'ı ayrı pencerelerde açar.
2. **Yalnızca WebApi Başlatmak:**
   `Deploy/1_Run_WebApi.bat`
3. **Yalnızca Blazor UI Başlatmak:**
   `Deploy/2_Run_Blazor.bat`
4. **Veri Tabanı Migration'larını Uygulamak:**
   `Deploy/3_Update_Database.bat`

---

## 5. Dokümanlar

| Doküman | İçerik |
|---|---|
| [adding-a-new-object.md](adding-a-new-object.md) | **Yeni bir obje (entity) ekleme adımları** — entity'den şemaya, controller'dan Blazor ekranına, menüden izne kadar uçtan uca kontrol listesi. Örnek: `Hydra.MailManagement` (Mail / MailTemplate / MailAttachment) entegrasyonu. |
| [login-to-dashboard-flow.md](login-to-dashboard-flow.md) | Login → JWT → Dashboard akışı ve Tentacle tarafındaki bağlantı adımları. |
| [DEPLOYMENT_SCRIPTS.md](DEPLOYMENT_SCRIPTS.md) | `Deploy/` altındaki başlatma ve dağıtım scriptleri. |

---

## 6. Kurulu Modüller

| Modül | Nesneler | Ekran |
|---|---|---|
| **Hydra.MailManagement** | `Mail`, `MailTemplate`, `MailAttachment`, `BaseObjectMail` | Dashboard → **Mail Management** (`/Mail`, `/MailTemplate`, `/MailAttachment`) |

Mail modülü `Hydra/Hydra.MailManagement` ve `Hydra/Hydra.MailManagement.WebApi` projelerinden
gelir; controller'lar ApplicationPart olarak bağlanır, Tentacle'da tek satır mail controller
kodu yoktur. Ayarlar `appsettings.json` → `MailSettings`. Modülün kendi kılavuzu:
`Hydra/Hydra.MailManagement/INTEGRATION_PLAYBOOK.md`.
