# Form Grid Layout (Row/Column Architecture) & Dark Theme Consistency

## 1. Problem Tanımı ve Kök Nedenler

Edit/Create ekranlarında (`GenericFormView`, `GenericEditView`, `GenericCreateView`) iki temel sorun yaşanmaktaydı:

### A. Input Alanının Açık Renk (Beyaz) Kalması ve Biçimsizliği
- **Kök Neden 1 (Koyu Tema Eksikliği):** `tentacle-theme.css` içinde `.hydra-textarea` için koyu arkaplan (`background: var(--tnt-bg);`) tanımlanmışken, form içi `.hydra-input` için bu tanımlama eksik kalmıştı.
- **Kök Neden 2 (Tarayıcı Autofill & Color Scheme):** Chromium tabanlı tarayıcılarda (Chrome, Brave, Edge) sayfa genelinde `color-scheme: dark;` ve `-webkit-autofill` arkaplan gölge hilesi (`-webkit-box-shadow: 0 0 0 1000px ... inset`) bulunmadığında; tarayıcı kullanıcı aracısı (user-agent) formu açık tema sanıp `field` (beyaz) arkaplan ve koyu metin zorlamaktaydı.
- **Kök Neden 3 (Dar Input Genişliği):** `ColumnFieldComponent` içindeki `.hydra-field` sarmalayıcısı `display: flex;` olup `width: 100%` almadığı için, standart HTML `<input>` öğesi tarayıcının yerel `size="20"` genişliğine (~210px) büzüşmekteydi.

### B. Input'ların Rastgele Dağılması ("Rastgele Gitmesi")
- **Kök Neden (Fluid Auto-Fit Grid):** `hydra-default.css` içerisindeki `.hydra-form__grid`, `grid-template-columns: repeat(auto-fit, minmax(260px, 1fr))` kullanıyordu. Bu akışkan yapı:
  - Ekran genişliğine göre belirsiz sayıda (2, 3 veya 4) kolon üretmekteydi.
  - `Description` (TextArea) tam satır (`grid-column: 1 / -1`) aldığında, kendisinden önceki tekil input (`Full Name`) ilk kolonda yapayalnız kalıp sağında boş delikler bırakmaktaydı.
  - `Position` ve `Active` (toggle switch) alt satırda yan yana düşüp asimetrik ve orantısız bir görüntü oluşturmaktaydı.

---

## 2. GedenLines Projesi Mimarisi ile Karşılaştırma

`GedenLines` projesinde (`FilteredMetaColumnComponent.razor` ve `BlazorHelper.cs`):
- Form alanları belirli satır ve sütunlara (`<div class="row">` ve `<div class="col-md-X">`) bölünerek yerleştirilmekteydi.
- Normal alanlar çiftler halinde yan yana (Bootstrap `col-md-6`), çok satırlı açıklamalar ise tam satır (`col-md-12`) olarak render edilmekteydi.

---

## 3. Uygulanan Çözüm Mimarisi

CSS framework bağımsızlığını koruyarak (semantik `hydra-*` mimarisi) **12 Kolonluk CSS Grid Sistemi** inşa edildi:

### 1. 12-Kolonluk Grid & Akıllı Yerleşim (`hydra-default.css` & `tentacle-theme.css`)
```css
.hydra-form__grid {
    display: grid;
    grid-template-columns: repeat(12, 1fr);
    gap: 1.25rem 1.5rem;
    grid-auto-flow: dense;
}

.hydra-form__field {
    grid-column: span 6; /* 2 kolon düzeni (GedenLines col-md-6 standardı) */
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    min-width: 0;
    width: 100%;
}

.hydra-form__field--wide {
    grid-column: span 12; /* Tam satır (GedenLines col-md-12 standardı) */
}

@media (max-width: 768px) {
    .hydra-form__grid {
        grid-template-columns: 1fr;
    }
    .hydra-form__field,
    .hydra-form__field--wide {
        grid-column: span 1;
    }
}
```

### 2. Form Kontrolleri ve Koyu Tema Uyumu (`tentacle-theme.css`)
- `color-scheme: dark;` kuralı `:root, html, body, .hydra-form, .hydra-input` seviyesine eklendi.
- `.hydra-field`, `.hydra-input`, `.hydra-select`, `.hydra-textarea` öğelerine `width: 100% !important;` ve `background-color: var(--tnt-bg) !important;` verildi.
- Chrome/Brave/Edge autofill kutularının beyazlaşmasını önleyen `-webkit-box-shadow ... inset` kuralı eklendi.
- `ColumnFieldComponent.razor` input'larına `autocomplete="off"` eklendi.

### 3. DTO Alan Sıralaması Mantığı
DTO yapılandırmalarında (`EmployeeDTO`, `PositionDTO`, `RequestCategoryDTO`):
- 1. Satır: İki standart alan (`Full Name` + `Position`) yan yana (`span 6` + `span 6` = 12).
- 2. Satır: `Description` (TextArea) tam satır (`span 12`).
- 3. Satır: `Active` (veya ilişkili bayraklar) temiz hizalı olarak yerleşir.
