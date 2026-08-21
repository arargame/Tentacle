# Prompt for Google Stitch — Tentacle CRUD / Master-Detail Screens

> Companion to `tentacle-stitch-prompt.md` (Landing / Login / Dashboard). This one covers the actual
> **application screens**: List, Details (with tabbed collections), Create/Edit forms, and the two
> reusable modals. It uses the same design tokens (colors, type, spacing) as the first prompt, plus a
> few new conventions the real Blazor components now follow — see the notes at the bottom before you
> paste this in, they matter.

---

## The prompt

```
Continue the "Tentacle" design system (deep navy/teal base, bright accent color, rounded corners,
soft shadows, calm SaaS aesthetic — same tokens as the Landing/Login/Dashboard screens already
designed). Now design the CRUD application screens: List, Details, Create/Edit form, and two
reusable modals.

COLOR CONVENTION (apply consistently across every screen below — this is a strict system, not a
suggestion):
- CREATE actions (new record buttons, the Save button while creating) → green (success color).
- EDIT actions (edit buttons, the Save button while editing an existing record) → blue (primary
  accent color).
- DELETE actions → red (danger color).
- DETAILS / VIEW actions (the "view row" icon button) → a distinct fourth color, teal/cyan (info
  color) — visually different from both the primary blue and the accent color used elsewhere, so a
  user can tell "view" apart from "edit" at a glance.
- BACK / CANCEL / secondary navigation → neutral grey (outline/secondary style), always present in
  the top-left or top-right of every screen below.

────────────────────────────────────────
SCREEN 1 — LIST VIEW (e.g. "Requests")
────────────────────────────────────────
- Page header: entity title, a "Filters" toggle button (outline grey, funnel icon) on the left side
  of the action group, and a green "+ New Record" button on the right.
- A collapsible filter panel (hidden by default): a row of small filter fields — text inputs,
  date-range pickers, and dropdowns depending on column type — with a blue "Apply" and a grey
  "Clear" button at the end of the row.
- A small quick-search input (magnifying glass icon, placeholder "Search this page...") sitting just
  above the table, left-aligned, narrower than the filter panel — this is a fast client-side search
  distinct from the Filters panel, which queries the full dataset.
- Data table: every row has a small teal/cyan circular "eye" icon button pinned to the far LEFT
  (Details), and on the far right a blue pencil icon (Edit) and a red trash icon (Delete), each as
  small outlined icon buttons. Column headers are clickable for sort (small caret indicator).
  Striped rows, hover highlight, comfortable row height.
- Pagination footer: page-size selector on the left, page numbers + prev/next on the right, showing
  total record count.
- Empty state: a calm centered illustration + "No records found" message when the table is empty,
  and a slightly different lighter message ("No results match your search") when the quick-search
  narrows to zero rows.

────────────────────────────────────────
SCREEN 2 — DETAILS VIEW WITH TABBED COLLECTIONS (e.g. "Position Details")
────────────────────────────────────────
- Header card: entity title, then a right-aligned action group with three buttons in this exact
  order and color: grey "← Back" (outline), blue "✎ Edit" (outline), red "🗑 Delete" (outline).
- Below the header, a clean key-value summary of the main properties: label in muted grey on the
  left, value in normal text on the right, two-column layout (label + value pairs stacked in rows).
  FK values that link elsewhere render as clickable blue links.
- Below the summary, a Bootstrap-style tab strip when the entity has more than one related
  collection (e.g. "Employees in this Position (4)" / "Responsible Categories (2)" — note the small
  rounded count badge next to each tab label). Only ONE tab's content is visible at a time; clicking
  a tab switches the panel below it, active tab underlined/highlighted in the accent color.
- Each tab's content is a compact nested table (smaller row height than the main list view) showing
  the related records, with the same far-left teal "eye" Details icon per row, and a small green
  "+ Add" button in that panel's own header to create a new related record pre-linked to this
  parent.
- If an entity has only ONE related collection, skip the tab strip entirely and show that one
  collection directly below the summary card (no need for a single, pointless tab).

────────────────────────────────────────
SCREEN 3 — CREATE / EDIT FORM (shared layout, two color states)
────────────────────────────────────────
- Header: form title ("New Request" / "Edit Request"), grey "← Back" button top-right.
- Two-column responsive field grid (labels above inputs, full-width for text areas), matching input
  styles to field type: text, dropdown (native-style select), date picker, checkbox, textarea.
  Foreign-key fields show a small "browse" icon next to the dropdown that opens the Lookup Modal
  (see below) as an alternative to the inline dropdown, for FK lists that are long.
  Show a subtle "prefilled" badge or muted-lock styling next to any field that arrived pre-filled
  from a parent screen (e.g. creating a related record from inside a Details tab) and is disabled.
- Footer action bar, right-aligned: grey "Cancel" button, then the Save button —
  render TWO visual variants of this same screen so both states are visible: one where the Save
  button is GREEN with a "+" icon and reads "Create" (this is the Create-mode screen), and one where
  it's BLUE with a checkmark icon and reads "Save" (this is the Edit-mode screen).
- Inline validation: a red banner at the top of the form body when a save attempt fails, plus a
  small spinner replacing the button icon while saving.

────────────────────────────────────────
SCREEN 4 — MODAL: CONFIRM DIALOG (generic, reusable)
────────────────────────────────────────
- Small centered modal, dimmed backdrop. Header: short title (e.g. "Delete Record"), close (x) icon
  top-right. Body: one or two lines of plain description text (e.g. "Are you sure you want to delete
  this record? This action cannot be undone."). Footer: grey "Cancel" button + a second button whose
  color matches the action it confirms — show the RED variant (for a delete confirmation) as the
  primary example, with a small caption noting "the confirm button's color changes based on the
  action — blue for a save-and-leave confirmation, red for delete, etc."

────────────────────────────────────────
SCREEN 5 — MODAL: LOOKUP / RECORD PICKER (generic, reusable)
────────────────────────────────────────
- Larger centered modal than the confirm dialog. Header: title (e.g. "Select Employee"), close icon.
- Body: a search input pinned at the top (magnifying glass icon, placeholder "Search..."), below it
  a scrollable list of selectable rows (simple list, not a full data table — just the record's
  display name per row), each row highlighting on hover, the currently-selected row shown with a
  checkmark and a soft accent-colored background. Clicking a row selects it immediately and closes
  the modal (no separate "confirm selection" step needed — but do include a "Cancel" button at the
  bottom in case the user wants to back out without picking anything).
- Empty state inside the list: "No matching records" when the search yields nothing.

Use the same accent color, type scale, spacing, corner radius, and shadow style established in the
Landing/Login/Dashboard screens so this feels like the same product's app pages, not a different app.
```

---

## Notes for you

- **The color convention above is not just a design suggestion — it's already implemented in the real Blazor components** (`GenericListView`, `GenericDetailsView`, `GenericFormView`, `HydraGrid`): Create is `btn-success` (green), Edit is `btn-primary`/`btn-outline-primary` (blue), Delete is `btn-danger`/`btn-outline-danger` (red), and the row-level "Details" eye icon is `btn-outline-info` (teal/cyan) — the fourth color, chosen specifically so it doesn't collide with Edit's blue. Whatever hex values Stitch gives you for these four roles, we can map straight into the app's CSS.
- **The tabbed Master-Detail layout is real, not aspirational** — `GenericDetailsView` now auto-detects how many `CollectionViewSection` children a Details page has and renders Bootstrap `nav-tabs` when there's more than one (with a live record-count badge per tab), or a single plain panel when there's only one — exactly as described in Screen 2. `Position/Details.razor` and `RequestCategory/Details.razor` are the two real pages that already get two tabs each for free, no page-level changes needed.
- **The quick-search box in Screen 1 is real too** — `HydraGrid` now has a small client-side search input above every table (filters the currently-loaded page instantly), separate from the existing server-side `FilterBarComponent` filter panel. If Stitch's mockup suggests different placement or copy for it, that's an easy CSS/text tweak on our end.
- **Both modals in Screens 4 and 5 already exist as components** — `ConfirmDialogComponent` (Title/Message/Cancel/Confirm with a configurable button color) and the new `LookupModalComponent` (search box + selectable list, backed by the existing `LookupService` that already powers FK dropdowns). Whatever Stitch produces for these two, we're translating it onto working components, not building from scratch.
- Once you have HTML/CSS back from Stitch for these five screens, send it over and I'll fold the visual details (spacing, exact shadows, icon choices, empty-state copy) into the real `.razor` files the same way we can do for the Landing/Dashboard/Login set.
