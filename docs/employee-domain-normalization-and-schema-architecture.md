# Employee Domain Normalization & Schema Integrity Architecture

## 1. Domain Problemi ve Kök Neden (Domain Normalization)

Önceki tasarımda `Employee` nesnesi `BaseObject<Employee>` sınıfından gelen `Name` alanını tek başına kullanıyor ve UI üzerinde yapay olarak `"Full Name"` etiketiyle sunuluyordu.

### Eksiklikler:
1. **İkili Veri İhtiyacı:** Çalışanlar (gerçek insanlar) için ad ve soyadın tek bir string alana sıkıştırılması veri madenciliği, filtreleme, alfabetik sıralama ve e-posta/bildirim kişiselleştirmelerini (ör. *"Sayın {Surname}"*) imkansız kılıyordu.
2. **Veritabanı Seviyesinde Nullable Boşluğu:** `BaseObject<T>` polimorfik olarak `Name` alanını `nullable` tanımladığı için veritabanında da `Name NVARCHAR(MAX) NULL` olarak kalmıştı. Bu durum harici veri yüklemelerinde veya doğrudan SQL işlemlerinde adı olmayan "hayalet çalışanlar" oluşma riskini barındırıyordu.

---

## 2. Mimari Çözüm ve Uygulanan Standartlar

### A. Varlık Katmanı (`Hydra/Core/HumanResources/Employee.cs`)
- `Name` (Ad / First Name): `BaseObject` üzerinden miras alınmaya devam eder, ancak anlamsal olarak yalnızca **Ad** bilgisini tutar.
- `Surname` (Soyad / Last Name): Sınıfa `public string? Surname { get; set; }` kolonu eklenmiştir.
- `FullName` (Computed Domain Property):
  ```csharp
  [NotMapped]
  public string FullName => $"{Name} {Surname}".Trim();
  ```
- **Domain Validasyonu (`IValidatableObject`):**
  ```csharp
  if (string.IsNullOrWhiteSpace(Name))
      yield return ValidationManager.Required(nameof(Name), "First Name");

  if (string.IsNullOrWhiteSpace(Surname))
      yield return ValidationManager.Required(nameof(Surname), "Surname");

  if (PositionId == Guid.Empty)
      yield return ValidationManager.MustBeSelected(nameof(PositionId), "Position");
  ```

### B. Veritabanı Bütünlüğü (EF Core & SQL Server)
1. **EF Core Fluent API (`HydraDbContext.cs`):**
   ```csharp
   modelBuilder.Entity<Employee>(entity =>
   {
       entity.HasOne(e => e.Position)
           .WithMany(p => p.Employees)
           .HasForeignKey(e => e.PositionId)
           .OnDelete(DeleteBehavior.Restrict);

       entity.Property(e => e.Name)
           .IsRequired()
           .HasMaxLength(100);

       entity.Property(e => e.Surname)
           .IsRequired()
           .HasMaxLength(100);
   });
   ```
2. **SQL Server Şeması (`TentacleDatabase`):**
   - `Surname` sütunu eklendi (`NVARCHAR(100) NOT NULL`).
   - `Name` sütunu `NVARCHAR(100) NOT NULL` olarak sıkılaştırıldı.
   - Mevcut veriler (ör. "Seed Employee") `Name = 'Seed'`, `Surname = 'Employee'` olarak normalize edildi.

### C. Görünüm Katmanı (`EmployeeDTO.cs` & `GenericFormView.razor`)
Form 12 sütunlu CSS grid yapısında düzenli ve simetrik hale getirildi:
- **Satır 1 (col-6 + col-6):** `Name` (First Name) & `Surname` (Surname) yan yana.
- **Satır 2 (col-6 + col-6):** `Position` dropdown & `IsActiveEmployee` switch.
- **Satır 3 (col-12):** `Description` tam genişlik textarea.
