# ItemService modernization audit

**Baseline:** `develop`  
**Target framework:** .NET 10  
**Target Franz.Common version:** 2.3.3  
**Scope:** ItemService API/domain/persistence/client and its integration with HeroService.Admin.  
**Status:** Initial repository-backed audit; implementation decisions that depend on runtime behavior are marked for validation.

## Executive summary

ItemService already has a layered solution and an HTTP client project, but it is not aligned with the target Franz.Common baseline. The current central package file pins most explicit Franz packages to 2.2.16, while the wildcard fallback is 2.2.6. HeroService's current branch pins Franz.Common packages to 2.3.3.

The existing seed pipeline is not suitable as the new catalogue importer: `ItemSeeder` embeds a small, hand-written set of item definitions, skips the entire operation if any item exists, and commits within the per-item helper as well as at the end. The supplied catalogue contains 56 richer definitions, so it needs a deliberate mapping and safe reconciliation strategy rather than a blind extension of the existing list.

HeroService.Admin already registers typed HeroService HTTP clients through `AddHeroServiceClients`; its project currently references only HeroService.Client.Http. The item-management UI should consume a separate ItemService client and ItemService-owned request/response DTOs, never ItemService domain entities or database access.

## Verified repository observations

### 1. Framework and package baseline

- `Directory.Packages.props` uses a wildcard Franz version of 2.2.6 and explicit versions of 2.2.16 for the packages referenced by the service.
- HeroService's `Develop_Unity_V1` branch centralizes Franz.Common packages at 2.3.3.
- ItemService targets .NET 10 through `common.targets`.
- The solution already separates Domain, Contracts, Application, Persistence, API, Consumer, and Client.Http projects.
- The solution also includes testing and architectural-report projects; preserve and extend these rather than introducing a parallel test structure.
- The current ItemService client project references ItemService.Contracts and Franz HTTP client abstractions. The exact public methods and registration surface still need a full contract-level review.

**Required action:** migrate the package graph as a coherent set to 2.3.3, verify the APIs used by ItemService against the released NuGet packages, and validate restore/build/tests using the repository's intended package sources. Do not assume wildcard versions override explicit package versions.

### 2. Current domain and persistence model

- `ApplicationDbContext` persists `ItemEntity` and `ItemBaseStats` in PostgreSQL.
- The current entity mapping stores category, mythology, and class affinity as integer enum values.
- Base stats are modeled as owned hero and skill modifier value objects.
- Behavioural conditions are persisted through a JSON conversion.
- Item names have a unique database index.
- The current model includes a signature hero ID, but the supplied catalogue includes a signature hero name; the migration must resolve that name to a stable relationship or preserve it as an explicitly defined external/domain reference.
- The supplied catalogue also contains price, bilingual passive content, cooldown labels, synergy heroes and notes, skill-modifier flags, and a design status. Those fields are not all represented by the currently inspected persistence mapping.

**Required action:** map every source field to an explicit domain/API representation or record a deliberate exclusion with a rationale. Do not discard gameplay semantics by flattening structured fields into text. Stable identity must not depend on localized display names.

### 3. Seeder behavior

- `ItemSeeder` currently embeds item definitions directly in C#.
- It exits early if the item table contains any record, so it cannot reconcile a partially seeded database or update missing catalogue entries.
- The current hand-written definitions include legacy item names and model values that do not directly represent all 56 source records.
- The helper commits the unit of work for each item, and the seeder commits again after the list. This creates unnecessary transaction boundaries and complicates all-or-nothing initialization.
- `Program.cs` migrates and seeds the database only in Development; higher environments rely on the deployment/orchestration plane.

**Required action:** separate reference data, canonical catalogue data, and any one-time legacy-data reconciliation. Use deterministic stable keys and explicit migration semantics. Seed operations must be repeatable, transactional where appropriate, and must not silently overwrite admin-managed changes. Keep production schema/data initialization under the existing deployment boundary unless a separate deployment decision changes it.

### 4. API lifecycle and security review points

- API composition registers application services, domain business services, relational persistence, auditing, mediator pipelines, and the Franz HTTP architecture.
- The development-only endpoint mapping contains an explicit anonymous-access path; the authorization middleware/endpoint policy behavior must be verified against the actual controller attributes and Franz HTTP architecture before changing it.
- API configuration currently contains a database credential in a tracked settings file. Treat it as exposed: remove reliance on committed credentials, move configuration to environment/secret providers, and rotate the credential if it is or has been used outside a disposable local environment.
- The current startup behavior intentionally avoids applying migrations/seeds in higher environments. Preserve that separation and document the deployment responsibility.

**Required action:** verify authorization at the API boundary for all write endpoints, error response contracts, validation propagation, auditing, and the actual runtime middleware order. UI visibility is not authorization.

### 5. HeroService.Admin integration

- `HeroService.Admin` is a .NET 10 Blazor application using MudBlazor.
- Its startup registers HeroService typed clients via `AddHeroServiceClients`.
- The current HeroService client registration configures a shared base URL and a 30-second timeout from the Admin composition root.
- The admin project references `HeroService.Client.Http`; it does not currently reference ItemService.Client.Http in its project file.
- Search of the current HeroService branch did not find an existing `ItemService.ClientHttp` integration.

**Required action:** add an ItemService client dependency through a separate project reference and DI extension/configuration key. Keep the ItemService base URL separate from the HeroService base URL. The UI calls ItemService only through the typed client, with DTOs defined by the ItemService HTTP contract. Do not add ItemService entities, EF Core, or persistence dependencies to HeroService.Admin.

### 6. Documentation and CI

- ItemService's current README describes UserService rather than ItemService and needs replacement with accurate ItemService architecture, responsibilities, API/client usage, configuration, seeding, and development instructions.
- The repository contains a solution-level test project and a separate architectural-report project. Their full coverage and workflow triggers are to be reviewed during implementation planning.
- The exact GitHub Actions workflow paths have not yet been established by this file audit; CI discovery is a required task before implementation changes.

## Catalogue source contract

The supplied `codex-items-app-v1.zip` is a content source, not the backend domain model. It contains 56 item records and includes, across the records:

- stable source IDs and English/French content;
- category, civilization, role/classification, and design status;
- price and activation state;
- structured stat entries, including whether a stat modifies a skill;
- passive name and bilingual description;
- trigger expression and cooldown information;
- signature/synergy hero references and explanatory notes;
- optional external-game equivalents.

Before seeding, produce a field-by-field mapping and validation report. Confirm the runtime semantics of trigger expressions such as `ForConsecutive(0, 3)`; do not treat arbitrary strings as executable behavior or invent trigger semantics. Keep design-review state separate from gameplay activation state.

## Implementation sequence

1. **PR 1 — Audit and plan (this PR):** record the repository findings, dependencies, service boundary, migration risks, and test gates.
2. **PR 2 — Franz.Common 2.3.3 alignment:** update central package versions coherently, fix compile/runtime incompatibilities, and validate the full solution before changing the domain schema.
3. **PR 3 — Canonical item domain and seed pipeline:** implement the validated domain/data mapping, reference data, transactional deterministic seeding, and explicit legacy reconciliation/migrations.
4. **PR 4 — ItemService API and Client.Http contracts:** add list/detail/create/update and lifecycle operations required by the confirmed domain; validate requests and authorize writes server-side.
5. **PR 5 — HeroService.Admin item management:** register the separate typed client and add catalogue, detail, create, and edit experiences with loading, validation, and error states.
6. **PR 6 — Integration and release readiness:** test clean/repeated seeding, PostgreSQL migrations, API contracts, authorization, UI error handling, and GitHub Actions workflows.

## Acceptance gates

- ItemService builds and tests against the released Franz.Common 2.3.3 package graph.
- Every one of the 56 source records is accounted for by the mapping report.
- Seed initialization is deterministic, repeatable, and does not silently overwrite admin-owned data.
- Item mutations are validated and authorized by ItemService.
- HeroService.Admin uses ItemService.Client.Http DTOs over HTTP only.
- No direct cross-service database access or duplicated ItemService business rules are introduced.
- PostgreSQL integration tests and architecture tests cover the new boundaries.
- Documentation and GitHub Actions accurately describe and validate the implemented solution.

## Explicitly out of scope for the audit PR

This PR does not change domain behavior, package versions, database schema, production authorization, or seed data. Those changes require the evidence and test gates above and will be delivered in focused implementation PRs.
