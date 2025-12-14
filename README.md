# HydraTentacle

HydraTentacle is a robust **Work Tracking System** designed to manage requests, units, and positions within an organization. It is built as a multi-project solution leveraging the **Hydra** framework.

## Project Overview

HydraTentacle simplifies workflow management by organizing tasks into a structured hierarchy:

- **Requests**: The core units of work or tasks that need to be completed.
- **Categories**: Taxonomies for classifying requests (e.g., IT Support, HR, Maintenance).
- **Units**: Functional groups or departments responsible for handling specific categories of requests.
- **Positions**: Roles within units that are assigned to specific request categories, establishing a clear chain of responsibility.

Effectively, Tentacle maps *incoming requests* to the *consuming units* through *assigned positions*, ensuring every request is routed to the right team.

## Solution Structure

- **HydraTentacle.Core**: Core library containing domain models (e.g., Request, RequestDTO) and business logic.
- **HydraTentacle.WebApi**: Backend API exposing endpoints for the Blazor frontend and external consumers.
- **HydraTentacle.Blazor**: Frontend UI built with Blazor, utilizing a clean architecture with **Base Views** and **Generic Components**.

## Dependencies

This solution references the **Hydra** framework which is expected to be located at `../Hydra` relative to this repository root.
If you are cloning this repository, ensure that the `Hydra` repository is checked out in the sibling directory:
```
/ParentFolder
  /Hydra
  /Tentacle (this repo)
```

## Getting Started

1. Ensure **Hydra** is set up.
2. Open `HydraTentacle.sln`.
3. Build and Run.
