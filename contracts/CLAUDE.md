# Contracts Agent CLAUDE.md

## Purpose
This directory is the **handoff layer** between the .NET API and the Vue frontend.
It contains the OpenAPI spec (owned by the API) and the generated TypeScript types (consumed by the frontend).

## Contents
```
contracts/
├── CLAUDE.md
└── openapi.yaml     ← Generated from .NET API — never edited manually
```

## The Golden Rule
> `openapi.yaml` is always **generated**, never hand-written.
> `frontend/src/api/generated/schema.d.ts` is always **generated**, never hand-written.

If these files are out of date, regenerate them — don't patch them manually.

---

## Regenerating the OpenAPI Spec (API → Contracts)

Run from the `api/` directory after any endpoint, DTO, or controller change:

```bash
dotnet swagger tofile --output ../contracts/openapi.yaml ./bin/Debug/net8.0/YourApi.dll v1
```

Or if using `Swashbuckle.AspNetCore.Cli`:
```bash
dotnet tool run swagger tofile --output ../contracts/openapi.yaml your-api.dll v1
```

**When to regenerate:**
- After adding or removing any controller endpoint
- After changing any DTO (request or response shape)
- After changing route paths or HTTP methods
- After adding or removing query/route parameters

---

## Regenerating TypeScript Types (Contracts → Frontend)

Run from the `frontend/` directory after `openapi.yaml` changes:

```bash
npx openapi-typescript ../contracts/openapi.yaml -o src/api/generated/schema.d.ts
```

This produces typed interfaces for every request and response shape defined in the spec.

**When to regenerate:**
- Every time `openapi.yaml` is updated
- Always regenerate the TypeScript types immediately after the spec — don't let them drift

---

## Full Cross-Layer Sync Sequence

When a feature touches both layers, always run both steps in order:

```bash
# Step 1 — from api/
dotnet swagger tofile --output ../contracts/openapi.yaml ./bin/Debug/net8.0/YourApi.dll v1

# Step 2 — from frontend/
npx openapi-typescript ../contracts/openapi.yaml -o src/api/generated/schema.d.ts
```

Commit both `openapi.yaml` and `schema.d.ts` together in the same commit as the API change.

---

## Using Generated Types in the Frontend

After regeneration, types are available at `src/api/generated/schema.d.ts`.

```typescript
// Import the generated types
import type { components } from '@/api/generated/schema'

// Use them for API response shapes
type UserDto = components['schemas']['UserDto']
type CreateUserRequest = components['schemas']['CreateUserRequest']

// Use in composables
const user = ref<UserDto | null>(null)
```

---

## Validation Checklist

Before committing a feature that touches both layers, verify:

- [ ] All new/changed endpoints are reflected in `openapi.yaml`
- [ ] `schema.d.ts` has been regenerated and committed
- [ ] Frontend composables use types from `schema.d.ts`, not hand-written interfaces
- [ ] No `any` types introduced to work around missing generated types (regenerate instead)
- [ ] API and frontend agree on field names and casing (check generated types match usage)
