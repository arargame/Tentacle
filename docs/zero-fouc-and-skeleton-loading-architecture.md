# Dashboard Zero-FOUC & Shimmer Skeleton Architecture

## 1. Problem Tanımı ve Kök Neden (Root Cause Analysis)

`/Dashboard` rotasına ilk giriş veya sayfa yenileme (F5 / hard reload) yapıldığında anlık olarak (~500ms - 1500ms) aşağıdaki görsel bozulmalar (layout glitch & shift) gözlemleniyordu:

1. **Header Layout Collapse (FOUC):**
   - Sayfa başlığı `Dashboard` ve aksiyon butonu `+ NEW REQUEST` iç içe geçip sıkışmış (`Dashboard+NEW REQUEST`) tek bir blok halinde görünüyordu.
   - Arama kutusu ve hizalamalar henüz devreye girmemişti.
2. **Karanlık Boşluk ve Hizasız Yükleme Göstergesi:**
   - Sayfa yüklenirken (`IsLoading = true`) tüm dashboard içeriği DOM'dan düşürülüyor ve sol üst köşede `C Loading...` şeklinde ortalanmamış, ham bir metin satırı görünüyordu.
3. **İkon ve Yazı Tipi Kararsızlığı (Font Pop-in / Ligature Flash):**
   - Sayfada Google Fonts üzerinden yüklenen `Material Symbols Outlined` kullanılıyordu. `display=swap` direktifi nedeniyle font inene kadar `progress_activity`, `rule`, `category`, `account_tree` gibi kelime ligatürleri veya anlamsız glifler görünüyordu (`rule` ikonunun `==` şeklinde ruler çizmesi gibi).
   - Tentacle'ın geri kalanındaki temiz Bootstrap Icons (`bi bi-*`) ile tasarım uyumsuzluğu vardı.

### Kök Neden: Tailwind Play CDN ve Çalışma Zamanı Derlemesi
- `Dashboard.razor`, `<HeadContent>` içerisinde `cdn.tailwindcss.com` Play CDN kütüphanesini yüklüyordu.
- Blazor Server interaktif modunda, ilk sunucu render'ında (Frame 0) HTML istemciye ulaştığında Tailwind JS derleyicisi henüz indirilmemiş ve çalıştırılmamıştı.
- Dolayısıyla `flex`, `h-16`, `justify-between`, `items-center`, `hidden`, `w-64`, `grid` gibi sınıflar CSSOM'da tanımlı olmadığından tarayıcı varsayılan akışla (unstyled inline/block flow) render alıyor, CDN yüklendikten sonra aniden sıçrama (Cumulative Layout Shift - CLS) yapıyordu.

---

## 2. Çözüm Mimarisi: Critical CSS Shell + Xbox Live Shimmer Skeleton

### A. Sıfırıncı Kare (Frame 0) Kritik Düzen CSS'i (`dashboard.css` + `App.razor`)
- Kritik düzen kuralları Tailwind CDN'in inisiyatifinden çıkarıldı ve statik `wwwroot/css/dashboard.css` dosyasına taşındı.
- `dashboard.css`, `App.razor`'ın `<head>` bloğuna eklenerek ilk HTTP yanıtında tarayıcıya iletilmesi sağlandı:
  ```html
  <link rel="stylesheet" href="@Assets["css/dashboard.css"]" />
  ```
- **Kritik Sınıflar:**
  - `.dashboard-shell`: 100vh flex kapsayıcı (`#0b1120`).
  - `.dashboard-sidebar`: 16rem sabit genişlikli, border ve flex navigasyon barı (`#0b0f10`).
  - `.dashboard-header`: 4rem yüksekliğinde `display: flex; justify-content: space-between; align-items: center;` ile başlık ve butonları Frame 0'dan itibaren kilitleyen üst bar.
  - `.dashboard-header__search`: Arama kutusu mutlak ikon ve focus glow stilleri.
  - `.dashboard-header__btn-new`: Cyan aksiyon butonu.
  - `.dashboard-canvas`: Esnek kaydırılabilir çalışma alanı.

### B. Bootstrap Icons Entegrasyonu (Sıfır Ligatür / Tam Birlik)
- Sol menüdeki, tablolardaki ve üst bardaki Material Symbols glifleri, uygulamanın ana menüsü (`NavMenu.razor`) ile birebir eşleşecek şekilde **Bootstrap Icons** (`bi bi-*`) ile değiştirildi:
  - Marka: `<i class="bi bi-broadcast"></i>` (Tentacle Work Orchestration)
  - Talepler: `bi bi-card-checklist`
  - Kategoriler: `bi bi-tags`
  - Kategori Sorumlulukları: `bi bi-diagram-3` (önceki anlamsız `==` kalktı)
  - İK & Yönetim Modülleri: `bi bi-people`, `bi bi-person-badge`, `bi bi-building`, `bi bi-shield-check`, `bi bi-key`
  - Aksiyonlar: `bi bi-search`, `bi bi-plus-lg`, `bi bi-eye`, `bi bi-funnel`, `bi bi-arrow-right`
- Bootstrap Icons CSS pseudo-element (`::before`) mimarisi kullandığından hiçbir kelime sızıntısı veya ligature bozulması yaşanmaz.

### C. Xbox Live / AAA-Grade Dark Shimmer Skeleton
`IsLoading = true` iken boş bir ekran ve tek bir spinner basmak yerine, gerçek dashboard grid geometrisini birebir taklit eden koyu iskelet (skeleton) kartları devreye sokuldu:
- **4 KPI İskelet Kartı:** Toplam, Açık, Sürüyor, Tamamlandı kartlarının tam yüksekliğinde ve konumunda.
- **Dağılım Çubuğu İskeleti:** Global durum çubuğu iskeleti.
- **Orta Panel İskeletleri:** Donut grafik ve öncelik listesi panellerinin iskeletleri.
- **Animasyon:** `@keyframes dashboard-pulse` ile 1.8 saniyelik yumuşak koyu nabız/shimmer efekti.
- Veriler API'den geldiğinde DOM düzeninde sıfır kayma (Zero CLS) ile gerçek kartlar yerini alır.
