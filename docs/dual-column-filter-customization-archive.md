# Arşiv Dokümanı: İkili Durum Kolon ve Filtre Özelleştirme Mimarisi (Dual Column-Filter Customization)

> **Arşiv Notu:** Bu doküman, Column Chooser bileşeninde "Show in Grid" ve "Filter Bar" durumlarının kullanıcı tarafından bağımsız iki ayrı checkbox ile yönetildiği mimariyi ve kod referanslarını ileride ihtiyaç duyulması ihtimaline binaen kayıt altına almak üzere oluşturulmuştur.

---

## 1. Mimarinin Amacı ve Kapsamı
Kullanıcının tek bir modal üzerinden hem tablodaki sütun görünürlüğünü (`SelectedColumn`) hem de arama panelindeki filtre bileşenlerinin görünürlüğünü (`FilterDTO.CreateFilterComponentFromThis`) birbirinden bağımsız olarak açıp kapatabilmesi hedeflenmişti.

### Temel Senaryo
- Bir kolon tabloda görünmeyebilir (örneğin `ModifiedDate`), ancak filtre barında bu alana göre tarih aralığı araması yapılmak istenebilir.
- Veya bir kolon sadece tabloda gösterilir, ancak filtre barında yer işgal etmemesi için filtresi kapatılır.

---

## 2. İkili Durum Veri Modeli ve UI Yapısı

### 2.1. `ColumnChooserItem` Modeli
```csharp
public class ColumnChooserItem
{
    public string ColumnName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsVisibleInGrid { get; set; }
    public bool IsVisibleInFilterBar { get; set; }
    public bool SupportsFilter { get; set; }
    public MetaColumnDTO MetaColumn { get; set; } = default!;
}
```

### 2.2. Modal Başlığı ve Satır Markup'ı
```html
<div class="hydra-column-chooser__table-header">
    <span class="hydra-column-chooser__col-name">Field / Column</span>
    <span class="hydra-column-chooser__col-opt">Show in Grid</span>
    <span class="hydra-column-chooser__col-opt">Filter Bar</span>
</div>

<div class="hydra-column-chooser__list">
    @foreach (var item in FilteredItems)
    {
        <div class="hydra-column-chooser__item">
            <div class="hydra-column-chooser__field-info">
                <span class="hydra-column-chooser__label">@item.DisplayName</span>
                <span class="hydra-column-chooser__prop-name">@item.ColumnName</span>
            </div>

            <!-- 1. Durum: Grid Görünürlüğü -->
            <div class="hydra-column-chooser__checkbox-cell">
                <input type="checkbox"
                       class="hydra-checkbox"
                       checked="@item.IsVisibleInGrid"
                       @onchange="e => OnGridVisibilityChanged(item, e.Value)" />
            </div>

            <!-- 2. Durum: Filtre Barı Görünürlüğü -->
            <div class="hydra-column-chooser__checkbox-cell">
                @if (item.SupportsFilter)
                {
                    <input type="checkbox"
                           class="hydra-checkbox"
                           checked="@item.IsVisibleInFilterBar"
                           @onchange="e => OnFilterVisibilityChanged(item, e.Value)" />
                }
                else
                {
                    <span class="hydra-text-muted">—</span>
                }
            </div>
        </div>
    }
</div>
```

---

## 3. Değişikliklerin Uygulanması (`ApplyChanges`)

```csharp
private async Task ApplyChanges()
{
    if (Table?.MetaColumns == null) return;

    foreach (var item in _items)
    {
        var matchingCols = Table.MetaColumns.Where(c => c.Name == item.ColumnName).ToList();

        // 1. Grid Görünürlüğü (SelectedColumn <-> MetaColumn dönüşümü)
        var gridCol = matchingCols.FirstOrDefault(c => c.TypeName == nameof(SelectedColumn) || c.TypeName == nameof(MetaColumn));
        if (gridCol != null)
        {
            gridCol.TypeName = item.IsVisibleInGrid ? nameof(SelectedColumn) : nameof(MetaColumn);
        }

        // 2. Filtre Barı Görünürlüğü
        foreach (var col in matchingCols.Where(c => c.FilterDTO != null))
        {
            col.FilterDTO!.CreateFilterComponentFromThis = item.IsVisibleInFilterBar;
        }
    }

    await TableChanged.InvokeAsync(Table);

    // Kalıcı Tercih JSON Formatı
    var json = System.Text.Json.JsonSerializer.Serialize(new
    {
        visibleColumns = _items.Where(i => i.IsVisibleInGrid).Select(i => i.ColumnName).ToList(),
        filterColumns = _items.Where(i => i.IsVisibleInFilterBar).Select(i => i.ColumnName).ToList()
    });

    if (OnPreferenceSaved.HasDelegate)
        await OnPreferenceSaved.InvokeAsync(json);

    await Close();
}
```

---

## 4. Kullanıcı Deneyimi Geri Bildirimi ve Basitleştirme Kararı
- Kullanıcı testlerinde, gridde yer almayan bir sütunun filtre barında bağımsız olarak açık kalabilmesi görsel karmaşaya ve tutarsızlık hissine yol açtı ("gridde olmayan filtrede olmasın").
- Bu nedenle sistem basitleştirilerek tekil seçim modeline geçirilmiştir: Bir sütun açıldığında hem gridde görünür hem filtre barında aktifleşir; kapatıldığında her ikisinden de çıkarılır.
- İleride bağımsız filtre yapılandırmasına yeniden ihtiyaç duyulması durumunda bu dokümandaki iki kolonlu checkbox şablonu ve `IsVisibleInFilterBar` ayrımı tekrar devreye alınabilir.
