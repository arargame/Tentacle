# Validation and Secure Error Handling Architecture

This document describes the end-to-end validation, error suppression, and security architecture established across Hydra Core, Hydra.RazorClassLibrary, and Tentacle.

---

## 1. Architectural Principles

1. **Information Disclosure Prevention (CWE-209 / OWASP Compliance):**
   - End users and external API consumers MUST NEVER receive raw SQL syntax, internal table names, column names, or database constraint identifiers (such as `FK_Employee_Position_PositionId`).
   - Detailed stack traces, raw SQL error codes, and technical diagnostics are strictly routed to the database developer log (`Log` table).
   - User-facing responses receive clean, human-readable, universal messages explaining the condition without revealing internal system topologies.

2. **Defense-in-Depth (Dual-Layer Validation):**
   - **Frontend (Client-side):** Evaluated immediately inside `GenericFormView.razor` upon clicking Save/Create. If required properties or foreign keys (`Guid.Empty`) are missing, the HTTP request is suppressed, eliminating server overhead and rollback penalties.
   - **Backend (Server-side):** Evaluated in `Service<T>.CreateAsync` and `UpdateAsync` using `entity.IsValid(out var validationResults)`. Even if an HTTP client bypasses the frontend, the entity rejects invalid state before touching the database.

3. **DRY & C# Shared Rules (`IValidatableObject`):**
   - Both Blazor and WebApi execute the exact same C# `Validate()` method declared on the entity.
   - Rules are stated once using compile-time safe `nameof(...)` syntax.

4. **Localization Readiness:**
   - Message templates are routed through `ValidationManager.FormatMessage` with a pluggable `Localizer` delegate, ready for multi-language dictionary integration.

---

## 2. Key Components

### A. Security Shield: `SqlExceptionHelper.cs` (`Hydra/Utils/`)
- `ToTechnicalDetails(Exception ex)`: Parses foreign key and unique violations to extract table, column, and constraint names for the developer `Log` table.
- `ToSafeUserMessage(Exception ex)`: Converts exceptions into safe, sanitized messages:
  - FK violation on INSERT/UPDATE: `"A selected related record does not exist or has been removed."`
  - FK violation on DELETE: `"This record cannot be deleted because it is currently referenced by other records."`
  - Unique/Duplicate constraint: `"A record with this information already exists."`
  - Deadlock/Timeout: `"The system is currently busy. Please try again in a few moments."`

### B. Unit of Work & Service Error Propagation
- `UnitOfWork.CommitAsync`: Captures exceptions, logs the full technical diagnostic to `_logService.SaveAsync(...)`, and populates `LastErrorSafeMessage = SqlExceptionHelper.ToSafeUserMessage(ex)`.
- `Service.Command.cs`: Inspects `_unitOfWork.LastErrorSafeMessage` when `!isCommitted` and delivers the sanitized message inside `response.Fail(...)` instead of generic fallbacks.

### C. Validation Engine: `ValidationManager.cs` (`Hydra/ValidationManagement/`)
- `Required(string propertyName, string? displayName = null)`: Produces `"{displayName} is required."` with `memberNames = [propertyName]`.
- `MustBeSelected(string propertyName, string? displayName = null)`: Produces `"{displayName} must be selected."` for dropdown/foreign key properties.
- `Localizer` hook: Pluggable `Func<string, object[], string>?` for global i18n support.

### D. Entity Declarations: `BaseObject.cs` & `Employee.cs`
Entities declare their validation contract by overriding `Validate`:
```csharp
public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
{
    foreach (var result in base.Validate(validationContext))
        yield return result;

    if (string.IsNullOrWhiteSpace(Name))
        yield return ValidationManager.Required(nameof(Name), "Full Name");

    if (PositionId == Guid.Empty)
        yield return ValidationManager.MustBeSelected(nameof(PositionId), "Position");
}
```

### E. Blazor Form Engine: `GenericFormView.razor` (`Hydra.RazorClassLibrary`)
- `ValidateForm()`:
  - Invokes `IValidatableObject.Validate` directly on the model.
  - Verifies `IsRequired(field)` for any unhandled value types or `[Required]` properties.
  - Maps `memberNames` to `_fieldErrors` dictionary (matching both property name and foreign key suffixes).
- Field markup receives `.hydra-form__field--invalid` (red border) and `<span class="hydra-form__field-error">`.
- `SetModelValue`: Clears field-level errors dynamically as the user types or selects options.
- Theming: High-contrast red accents styled in both `hydra-default.css` and `tentacle-theme.css`.
