# Prompt for Google Stitch — Tentacle UI (Landing, Dashboard, Login)

> Paste the block below into Stitch as-is. It's written as one long prompt on purpose — Stitch reads it as a single design brief. If Stitch asks you to generate one screen at a time, generate them in this order: **Landing Page → Login → Dashboard**, and paste the same brief each time so the style stays consistent.

---

## The prompt

```
Design a modern SaaS web app called "Tentacle" — a multi-request work and task management
system built around one simple idea: every unit of work is a Request, and every Request gets
a Response. Teams use it to raise requests, route them automatically to the right person or
department, track their status (Open, In Progress, Done), and never lose track of anything.

BRAND CONCEPT
The name plays on the octopus: many arms (requests) moving independently and intelligently,
all connected to one body (the system), each arm sensing and reporting back on its own. Lean
into this visually but keep it subtle and premium — this is a serious productivity tool for
teams, not a kids' app. Think "deep-sea tech" rather than "cartoon octopus."

VISUAL STYLE
- Modern, confident B2B SaaS aesthetic — similar in spirit to Linear, Notion, or Vercel's
  marketing sites: clean, generous whitespace, sharp typography, subtle depth.
- Color palette: deep ocean tones as the base — near-black navy / deep teal background options
  for dark sections, paired with a bright accent color (electric teal, cyan, or violet) used
  sparingly for CTAs, active states, and data highlights. Light mode should use off-white /
  pale blue-grey backgrounds with the same accent color.
- Organic, curved shapes in the background (soft blob/tentacle-like gradients or line art
  suggesting flowing arms/tendrils) used as subtle decorative elements behind hero sections or
  card edges — never literal cartoon tentacles, more like abstract flowing curves and gradient
  blobs, closer to how "neural network" or "fluid" illustrations look in modern SaaS branding.
- Rounded corners, soft shadows, generous padding. Sans-serif type, confident large headlines,
  clean data tables and cards for the app screens.
- Fully responsive layouts (desktop-first, but should hold up on tablet/mobile).

Generate the following three screens as one connected design system, sharing the same
typography, color palette, spacing scale, and component style:

────────────────────────────────────────
SCREEN 1 — LANDING PAGE
────────────────────────────────────────
- Sticky top navbar: logo mark "Tentacle" (an abstract mark suggesting radiating arms/curves
  from a central point — not a literal octopus cartoon), nav links (Product, How it works,
  Pricing, About), a "Log in" link, and a primary "Get Started" button.
- Hero section: large confident headline ("One Body. Eight Arms. Zero Dropped Requests." or
  similar), a supporting subheadline about requests and responses, two CTAs (primary "Start a
  Request", secondary "See it in action"), and a large abstract hero illustration/graphic on
  the right — flowing gradient curves suggesting tentacle arms reaching toward floating cards
  that represent tasks/requests being picked up.
- "How it works" section: a 4-step horizontal flow (Raise it → Route it → Work it → Respond it)
  with a small icon and short description per step, connected by a subtle curved line.
- Features section: a 3–4 column grid of feature cards (Smart Routing, One Pane for Every Team,
  a Dashboard That Tells You Something, Built to Extend, Full Audit Trail) — each with an icon,
  short title, and one-line description.
- A dark "About / Why Tentacle" band: short paragraph connecting the octopus metaphor to the
  product (many arms working independently, one shared brain/dashboard), paired with a subtle
  abstract graphic of branching curved lines.
- Social proof strip (logo placeholders / stat counters — e.g. "10,000+ requests routed daily").
- Closing CTA band with a bold headline and a single prominent button.
- Footer: logo, short tagline, link columns (Product, Company, Resources), social icons.

────────────────────────────────────────
SCREEN 2 — LOGIN SCREEN
────────────────────────────────────────
- Split-screen layout: left half is a centered login card on a clean background; right half is
  a full-bleed branded panel using the deep ocean gradient with the same abstract flowing-arms
  graphic from the hero, plus a short reassuring line of copy (e.g. "Every request finds its
  way home.").
- Login card contents: Tentacle logo mark at top, "Welcome back" heading, email field, password
  field (with show/hide toggle), "Forgot password?" link, primary "Log In" button (full width,
  accent color), a divider ("or continue with"), and SSO buttons (Google, Microsoft).
- Small footer line under the card: "Don't have an account? Request access."
- Keep the form minimal, generous field spacing, soft rounded inputs, subtle focus states in
  the accent color.

────────────────────────────────────────
SCREEN 3 — DASHBOARD (post-login app screen)
────────────────────────────────────────
- Left sidebar navigation (collapsible), dark or deep-navy background: logo mark at top, then
  nav items grouped by section — "Work Tracking" (Requests, Categories), "HR" (Employees,
  Positions, Organization Units), "Administration" (Users, Roles, Permissions) — with simple
  line icons, and a user avatar + name pinned at the bottom of the sidebar.
- Top bar: page title "Dashboard", a global search field, a "+ New Request" primary button, and
  a notification bell + user avatar on the right.
- KPI row: 4 stat cards across the top — Total Requests, Open, In Progress, Completed — each
  with a large number, a short label, and a small trend indicator (up/down arrow with percent).
- A status distribution section: a horizontal progress-bar-style breakdown by status (Open /
  In Progress / Completed) with counts and percentages, using the accent color family in
  different shades.
- Two side-by-side panels below: a "Requests by Category" bar or donut chart, and a "Team
  Workload" panel showing a short ranked list of people/positions with their open request
  counts.
- A "Recent Requests" data table at the bottom: columns for Request title, Category, Requested
  by, Assigned position, Status (as a colored pill/badge), Priority, and Date — with a small
  "View" icon button on the far left of each row that would open the request's detail view.
  Include realistic-sounding sample data (5–8 rows) with a mix of statuses and priorities.
- Keep the dashboard information-dense but calm — plenty of breathing room between cards,
  consistent corner radius, soft shadows, and the same accent color used consistently for
  primary actions and active states across all three screens.

Deliver all three screens using the same design tokens (colors, typography, spacing, button
and card styles) so they clearly belong to one product.
```

---

## Notes for you

- If Stitch lets you set a color palette or upload brand colors first, feed it: a deep navy/teal base (`#0B1F2B`-ish), an off-white light surface, and a bright teal or violet accent — that keeps every screen visually consistent even if you regenerate one at a time.
- The "View" icon on the far left of the Dashboard's request table matches what we just built into the real `HydraGrid`/`GenericListView` components (a details button pinned to the left of each row) — worth keeping that detail in the mockup so the design and the actual app line up.
- Once you have HTML/CSS you like from Stitch, send it back here and I can help translate the layout into the actual Blazor components (`Home.razor`, `Dashboard.razor`, and a new `Login.razor`) so the real app matches the mockup.
