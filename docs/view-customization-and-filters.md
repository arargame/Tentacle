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

## 3. Responsive Filtre Çubuğu (FilterBar) Izgarası

Filtre çubuğunda tüm alanların (`Name`, `Description`, `Added Date`, `Active`) eşit genişlikte bir ızgaraya oturması sağlanmıştır. Tarih aralığı filtreleri (`BetweenFilter`) diğer tekil filtrelerden daha fazla sütun kaplamaz (`span 1`); iki tarih kutusu ve aralarındaki ayırıcı (`–`) tek bir sütun hücresine kompakt şekilde sığdırılır:

| Cihaz / Çözünürlük | Kolon Sayısı | Her Filtre Alanı (`Name`, `BetweenFilter` vb.) |
|---|---|---|
| **Masaüstü (Desktop)** | 4 Kolon (`grid-template-columns: repeat(4, 1fr)`) | `grid-column: span 1` (Tümü eşit 1 sütun) |
| **Tablet (<= 1024px)** | 2 Kolon (`grid-template-columns: repeat(2, 1fr)`) | `grid-column: span 1` (Tümü eşit 1 sütun) |
| **Mobil (<= 600px)** | 1 Kolon (`grid-template-columns: 1fr`) | `grid-column: span 1` (Tam genişlik) |

`BetweenFilter` içindeki iki `date` input'u `flex: 1 1 0; min-width: 0; font-size: 0.75rem; text-align: center;` kurallarıyla boyutlandırılır ve `Name` alanıyla tam olarak aynı sütun genişliğini korur.

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
