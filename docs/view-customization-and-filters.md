# Hydra ViewDTO Özelleştirmeleri, Filtre Çubuğu ve Aktif Filtre Paneli

Bu doküman, Hydra framework ve Tentacle uygulamasında uygulanan UI/Grid ve ViewDTO geliştirmelerini, teknik mimari kararlarını ve kullanım kalıplarını belgeler.

---

## 1. ViewDTO'da BaseObject Alanlarını Hariç Tutma (Lambda Exclusion)

`BaseObject` üzerindeki ortak alanlar (`ModifiedDate`, `AddedDate`, `Description`, `IsActive`) varsayılan olarak `SetConfigurationsForBaseObjectMembers()` aracılığıyla hem `ListViewConfiguration` (grid kolonu ve filtre) hem de detay/düzenleme konfigürasyonlarına dahil edilir.

Belirli bir ekranda bu alanlardan birini (örneğin `ModifiedDate`) **hem grid tablosundan hem de filtre çubuğundan tamamen kaldırmak** için lambda destekli aşırı yükleme (overload) kullanılır:

```csharp
[RegisterAsViewDTO("RequestCategory")]
public class RequestCategoryDTO : Hydra.DTOs.ViewDTOs.ViewDTO
{
    public bool IsAssignable { get; set; }

    public override Hydra.DTOs.DTO LoadConfigurations()
    {
        SetConfigurationsViaStringPropertyInfo(
            propertyInfo: ReflectionHelper.GetPropertyOf<RequestCategoryDTO>(category => category.Name),
            displayName: "Name",
            attributeToFilter: new AttributeToFilter(nameof(ContainsFilter))
        );

        // ModifiedDate alanını hem filtreden hem de grid kolonlarından tamamen hariç tut:
        SetConfigurationsForBaseObjectMembers(category => category.ModifiedDate);

        return this;
    }
}
```

---

## 2. Tip Güvenli ve Açık İsimlendirilmiş Navigasyon Tanımları

Foreign Key ilişkilerinin UI üzerinde otomatik lookup ve link olarak bağlanması için `SetConfigurationsForNavigations` metodu generic lambda overload'ı ile genişletilmiştir:

```csharp
SetConfigurationsForNavigations<UserViewPreferenceDTO, SystemUser>(
    currentDto => currentDto.SystemUserId!,
    systemUser => systemUser.Name!,
    displayName: "User"
);
```

- Parametrelerde `x`, `u` gibi belirsiz kısaltmalar yerine `currentDto` ve hedef varlık türünü belirten açık isimler (`systemUser`) kullanılır.
- Tip güvenli (`Expression<Func<TSourceDto, object>>` ve `Expression<Func<TTargetEntity, object>>`) olduğundan IDE üzerinden F2 ile yeniden adlandırma (rename refactoring) tam desteklenir.
- **Önemli Kural**: Hydra nesne bağlama mimarisi gereği, DTO üzerinde `{AliasPrefix}Id`, `{AliasPrefix}_{ColumnNameToDisplay}` ve `{AliasPrefix}_Id` alanları tanımlanmış olmalıdır.

---

## 3. Responsive Filtre Çubuğu (FilterBar) Izgarası ve Kompakt (Small Input) Standardı

Filtre çubuğunda tüm alanların (`Name`, `Description`, `Added Date`, `Active`) eşit genişlikte bir ızgaraya oturması ve dikey alanı tasarruflu kullanarak okunaklılığı artırması sağlanmıştır:

### 3.1 Kompakt Input Ölçüsü (form-control-sm Standardı):
- **Yükseklik (Height)**: Hantal ve büyük kutular yerine Bootstrap `form-control-sm` standardı olan `1.875rem` (30px) sabit yükseklik uygulandı.
- **İç Boşluk (Padding)**: `0.25rem 0.5rem` ile dikey boşluk israfı önlendi, metinler ortalandı.
- **Yazı Tipi & Okunabilirlik**: `0.8125rem` (13px) net font boyutu ve `1.35` satır yüksekliği ile hem dar alanlarda taşma engellendi hem de okunabilirlik korundu.
- **Karanlık Tema Paleti (Tentacle)**: Gözü yoran beyaz parlamalar kaldırılarak `#0f172a` (koyu slate) zemin, `#334155` ince çerçeve, `#f1f5f9` yüksek kontrastlı metin ve `#94a3b8` net placeholder renkleri sağlandı.
- **Tarih Aralığı (BetweenFilter)**: İki tarih kutusu da aynı `1.875rem` (30px) yüksekliğe ve `0.78rem` fonta çekildi; takvim simgesi koyu temaya uygun olarak ters çevrildi (`filter: invert(0.8)`).
- **Aksiyon Butonları**: `Clear` ve `Apply` butonları da `.hydra-btn--sm` ile 30px kompakt yüksekliğe getirildi.

| Cihaz / Çözünürlük | Kolon Sayısı | Her Filtre Alanı (`Name`, `BetweenFilter` vb.) |
|---|---|---|
| **Masaüstü (Desktop)** | 4 Kolon (`grid-template-columns: repeat(4, 1fr)`) | `grid-column: span 1` (Tümü eşit 1 sütun) |
| **Tablet (<= 1024px)** | 2 Kolon (`grid-template-columns: repeat(2, 1fr)`) | `grid-column: span 1` (Tümü eşit 1 sütun) |
| **Mobil (<= 600px)** | 1 Kolon (`grid-template-columns: 1fr`) | `grid-column: span 1` (Tam genişlik) |

---

## 4. DevExpress Stili Aktif Filtre Paneli (Human-Readable Filter Expressions)

Grid tablosunun alt kısmında, o anda tabloda uygulanan filtrelerin insan tarafından okunabilir bir özetini gösteren ve tek tıkla temizleme imkanı sunan bir panel eklenmiştir:

- **Görünüm Örneği**: `[Name] contains 'Seed' AND [Active] is 'Yes'`
- **Temizleme Butonu**: Sağ tarafta yer alan `[X]` butonu tüm aktif parametreleri sıfırlayarak tabloyu başlangıç durumuna döndürür.
- **Filtre Tipi Dönüşümleri**:
  - `ContainsFilter` → `contains`
  - `BetweenFilter` → `between 'Tarih1' and 'Tarih2'`
  - `EqualFilter` (Boolean) → `is 'Yes'` / `is 'No'`
  - Genel Filtreler → `equals`

---

## 5. Dinamik Kolon Seçici ve Aday Kolon Mimarisi (Column Chooser & IsVisibleInColumnChooser)

Kullanıcının varsayılan ekranda yer almayan alanları (örneğin `ModifiedDate`) sonradan Grid'e veya Filtre Çubuğuna ekleyebilmesi için iki katmanlı bayrak mimarisi kurulmuştur:

### 5.1 Bayraklar ve Görevleri:
- **`IsVisibleInListView` (bool, default: true)**: Alanın başlangıçta (sayfa ilk açıldığında) grid tablosunda ve filtre çubuğunda görünüp görünmeyeceğini belirler. `false` yapıldığında alan başlangıç tablosunda ve filtre barında yer almaz.
- **`IsVisibleInColumnChooser` (bool, default: true)**: Alanın `ColumnChooserModal` (Kolon ve Filtre Özelleştirme) penceresinde kullanıcıya bir seçenek/aday kolon olarak sunulup sunulmayacağını belirler. `false` yapıldığında (ör. hassas hash'ler veya dahili join id'leri) modalda listelenmez.

### 5.2 Aday Kolon (Candidate Column) Yaşam Döngüsü:
1. `SetConfigurationsForBaseObjectMembers(category => category.ModifiedDate)` çağrıldığında `ModifiedDate` için:
   - `IsVisibleInListView = false`
   - `IsVisibleInColumnChooser = true`
2. `TableDTO.BuildMetaColumnsFromConfiguration` ve `PrepareUsingConfigurations`:
   - `IsVisibleInListView == true` olanları `SelectedColumn` olarak ekler (SQL SELECT'e girer, gridde görünür).
   - `IsVisibleInListView == false` ve `IsVisibleInColumnChooser == true` olanları `MetaColumn` (aday kolon) olarak ekler (SQL SELECT'e girmez, gridde görünmez ama meta-veride taşınır).
   - Filtre bileşeni için `CreateFilterComponentFromThis = false` set edilir (filtre barında ilk açılışta çıkmaz).
3. `ColumnChooserModal.razor`:
   - Modalda `ModifiedDate` listelenir, grid ve filtre onay kutuları (checkbox) **işaretsiz (unchecked)** olarak kullanıcının seçimine hazırdır.
   - Kullanıcı `ModifiedDate` için "Show in Grid" veya "Filter Bar" işaretleyip **Apply** dediğinde; aday kolon `SelectedColumn` tipine dönüştürülür ve `TableChanged` ile API'ye gönderilir.
   - API sorguyu yeniler, SQL `SELECT RequestCategory.ModifiedDate` işletilir ve hem tabloya hem filtre çubuğuna anında eklenir!
   - Kullanıcı **Reset Defaults** dediğinde, tüm alanlar `MetaColumn.IsVisibleInListView` başlangıç değerlerine anında geri döner.

- **`UserViewPreference`**: Kullanıcının gridlerde açıp kapattığı kolonları ve aktif filtreleri veritabanında saklayan entity. `SystemUserId` üzerinden kullanıcıya, `ViewKey` üzerinden ilgili ekrana bağlanır.

---

## 6. Giriş Ekranı (Login) FOUC İyileştirmesi ve Hızlı Test Kolaylığı

1. **FOUC (Flash of Unstyled Content) Engellendi:**
   - İlk açılışta harici Tailwind CDN betiğinin geç yüklenmesi nedeniyle oluşan bozuk/kaymış görünüm ve devasa Google/Microsoft SVG ikonları sorunu çözüldü.
   - `wwwroot/css/login.css` dosyası bağımsız saf CSS kuralları ile genişletildi ve `App.razor`'da statik olarak `<head>` içerisine bağlandı.
   - Inline SVG bileşenlerine `width="20" height="20"` nitelikleri eklenerek stil gecikmelerinde dahi taşmaları önlendi.

2. **Geliştirici Hızlı Giriş (⚡ 1-Click Admin Login):**
   - Test ve geliştirme süreçlerinde her seferinde `admin@tentacle.com` ve `ChangeMe123!` yazma zahmetini ortadan kaldırmak için login formunun hemen üzerine bir **Hızlı Test Çubuğu** eklendi.
   - Tek tıkla admin kimlik bilgilerini yükleyip doğrudan JWT oturumu açar ve Dashboard sayfasına yönlendirir.

---

## 4. Grid Toolbar Temizliği ve Modern UI Deneyimi

### 4.1. Sol Üstteki Client-Side Arama Kutusunun Kaldırılması
- **Gerekçe:** Kullanıcı sunucu taraflı filtreleme (FilterBar) kullanırken, grid üzerindeki lokal "Search this page..." arama kutusu gereksiz karmaşa yaratıyordu.
- **Uygulama:** `HydraGrid.razor` üzerinde `ShowSearchBox` parametresi varsayılan olarak `false` yapıldı. Sayfa kayıt sayacı (`RecordCountText`) ise `margin-left: auto` ile sağ tarafa hizalanarak toolbar temiz ve dengeli bir yerleşime kavuşturuldu.

---

## 5. Modern & Şık Scrollbar (Sidebar ve Global)

### 5.1. Sidebar (Navigasyon Konteyneri) Scrollbar İyileştirmesi
- **Sorun:** Windows üzerinde Chromium tarayıcılarının sol sidebar (`.tnt-sidebar`) için çizdiği 17 piksellik gri ve oklu varsayılan scrollbar, koyu tema bütünlüğünü bozuyordu.
- **Çözüm:**
  - `tentacle-theme.css` ve `hydra-default.css` içerisine modern webkit ve W3C scrollbar kuralları uygulandı.
  - Sidebar scrollbar genişliği `4px` olarak ayarlandı, rayı (track) şeffaf yapıldı.
  - Thumb rengi yarı saydam nötr (`rgba(133, 147, 151, 0.2)`), hover durumunda ise canlı cyan (`--tnt-cyan`) renge geçiş yapacak şekilde tasarlandı.
  - Firefox ve standart uyumluluk için `scrollbar-width: thin` ve `scrollbar-color` eklendi.

---

## 6. GedenLines Referanslı 3 Aşamalı Kolon Sıralama (3-State Sorting Cycle)

### 6.1. Sıralama Yaşam Döngüsü
Referans proje `GedenLines` mimarisi örnek alınarak `HydraGrid.razor` bileşeninde 3 durumlu sıralama döngüsü kuruldu:
1. **Durum 0 (Nötr / Yönsüz):**
   - Kolon sıralanmamıştır (`IsOrdered = false`).
   - Başlıkta soluk/nötr (`opacity: 0.35`) yönsüz sıralama ikonu (`bi bi-arrow-down-up`) görünür. Kullanıcıya kolonun sıralanabilir olduğunu hissettirir.
2. **Durum 1 (İlk Tıklama - Ascending):**
   - Kolon artan sırada sıralanır (`IsOrdered = true`, `Direction = Ascending`).
   - Diğer kolonların sıralaması temizlenir (tekil sıralama kuralı).
   - Başlıkta aktif cyan renkte ve parıltılı `bi bi-sort-alpha-down` ikonu belirir.
3. **Durum 2 (İkinci Tıklama - Descending):**
   - Kolon azalan sırada sıralanır (`IsOrdered = true`, `Direction = Descending`).
   - Başlıkta aktif cyan renkte `bi bi-sort-alpha-up-alt` ikonu belirir.
4. **Durum 3 (Üçüncü Tıklama - Nötr / Sıfırlama):**
   - Kolon sıralamadan çıkartılır (`existingSort.IsOrdered = false`).
   - İkon yeniden yönsüz soluk `bi bi-arrow-down-up` durumuna döner.
   - Backend `Table.GetOrderedMetaColumnsIncludingJoins` sorgusundan ilgili kolon düştüğü için SQL'deki `ORDER BY` ifadesi kalkar ve veriler orijinal sıralamasına döner.

---

## 7. Sayfalar Arası Filtre ve Görünüm Tercihlerinin Korunması (`TableStateManager`)

### 7.1. Sorun Analizi
Kullanıcı bir listede (örneğin `RequestCategory`) filtre uygulayıp başka bir menüye (örneğin Dashboard) geçip geri geldiğinde:
- Blazor router'ı yeni bir `GenericListView` bileşeni oluşturuyor ve `Table` nesnesi `null` olarak başlıyordu.
- Bu durum, kullanıcının filtreyi temizlemeden ayrılmış olmasına rağmen önceki arama kriterlerinin (`[Name] contains 'seed'`) kaybolmasına ve listenin tekrar filtresiz yüklenmesine neden oluyordu.

### 7.2. Çözüm Mimarisi: Katmanlı State ve Tercih Yönetimi
`Hydra.RazorClassLibrary` katmanına `ITableStateManager` / `TableStateManager` servisi eklendi:
1. **In-Memory Cache (Anında Hatırlama):** Scoped servis seviyesinde tutulan `ConcurrentDictionary<string, ViewPreferenceState>` sayesinde SPA içerisinde sayfalar arası gezinirken hiçbir gecikme olmadan son filtre değerleri, sayfalama ve sıralama korunur.
2. **LocalStorage Senkronizasyonu (Kalıcılık):** Sayfa yenilendiğinde (F5) veya sekme kapatılıp açıldığında kullanıcının filtre kriterleri tarayıcı yerel deposundan otomatik geri yüklenir.
3. **Filtre Barı ve Aktif Filtre Paneli Entegrasyonu:**
   - Sayfa ilk açıldığında saklanan filtreler `TableDTO`'ya giydirilir; `FilterBarComponent` içerisindeki inputlara kullanıcının yazdığı değerler otomatik doldurulur.
   - Sol alttaki DevExpress tarzı aktif filtre paneli (`[Name] contains 'seed'`) görünmeye devam eder.
   - Kullanıcı filtreyi `Clear` butonu veya sol alttaki `[X]` ikonuyla temizlediğinde state otomatik olarak güncellenir.


