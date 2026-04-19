# Project CLAUDE.md — Root Agent Context

## Project Overview
Full-stack application with a Vue 3 + TypeScript frontend and a .NET Entity Framework API backend.

## Repository Structure
```
project-root/
├── CLAUDE.md                  ← You are here
├── frontend/                  ← Vue 3 + TypeScript + PrimeVue
│   ├── CLAUDE.md
│   └── src/
├── api/                       ← .NET 8 + Entity Framework Core
│   ├── CLAUDE.md
│   └── ...
└── contracts/                 ← Shared OpenAPI spec + generated types
    ├── CLAUDE.md
    └── openapi.yaml
```

## Tech Stack Summary
| Layer      | Technology                                      |
|------------|-------------------------------------------------|
| Frontend   | Vue 3, TypeScript, PrimeVue, Pinia, Vue Router  |
| Backend    | .NET 8, Entity Framework Core, SQL Server       |
| Auth       | JWT bearer tokens                               |
| DTO        | AutoMapper                                      |
| Contracts  | OpenAPI / Swagger → openapi-typescript          |

## Cross-Agent Workflow

### Adding a New Feature (e.g., a new resource/module)
1. **API agent** — scaffold entity, migration, repository, service, controller
2. **API agent** — regenerate OpenAPI spec: `dotnet swagger tofile --output ../contracts/openapi.yaml`
3. **Contract agent** — regenerate TypeScript types from spec
4. **Frontend agent** — build composable, store, and components using generated types

### The Contract is the Handoff Point
- The API owns `contracts/openapi.yaml` — it is always generated, never hand-edited
- The frontend consumes `frontend/src/api/generated/schema.d.ts` — always generated from the spec, never hand-edited
- If frontend and backend appear out of sync, regenerate both before debugging

## Naming Conventions (Enforced Across All Agents)
- API resources are **plural nouns**: `users`, `products`, `orders`
- DTOs match the API resource name: `UserDto`, `ProductDto`
- Frontend types mirror DTOs exactly (generated from OpenAPI)
- Endpoints follow REST conventions: `GET /api/users`, `POST /api/users`, `GET /api/users/{id}`

## Authentication Flow
- JWT issued by `POST /api/auth/login`
- Token stored in Pinia `useAuthStore` (in-memory, not localStorage)
- Axios interceptor attaches `Authorization: Bearer <token>` to every request
- 401 responses trigger automatic redirect to `/login`

## General Rules for All Agents
- Never expose raw EF entities to the frontend — always use DTOs
- Never put business logic in Vue components — use composables or Pinia stores
- Always use async/await (no raw Promises or callbacks)
- All API calls go through the central Axios instance at `frontend/src/api/client.ts`
- Prefer explicit types over `any` everywhere
