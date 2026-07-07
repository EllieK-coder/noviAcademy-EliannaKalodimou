# Novibet · .NET Academy — NoviCode Capstone

This repository contains the **NoviCode** capstone assignment for the Novibet .NET Academy training programme.

## Programme Overview

15 days total: 10 days of structured teaching followed by a 5-day self-directed sprint.

| Week | Days | Theme |
|------|------|-------|
| Week 1 | 1–5 | C# Foundations — OOP, LINQ, Clean Architecture, EF Core |
| Week 2 | 6–10 | Service Patterns — REST, CQRS/MediatR, HTTP Clients, Quartz, Testing |
| Sprint | 11–15 | NoviCode Capstone — self-directed build |

## Solution Structure

```
NoviCode.slnx
├── src/
│   ├── NoviCode.Domain/          # Entities, value objects, domain interfaces
│   ├── NoviCode.Application/     # CQRS handlers, use-case logic, strategies
│   ├── NoviCode.Infrastructure/  # EF Core, Dapper, HTTP clients, Quartz jobs
│   └── NoviCode.Api/             # ASP.NET Core controllers, middleware, DI wiring
└── tests/
    └── NoviCode.Tests/           # xUnit test suite
```

## Sprint Deliverables

| # | Task | Key Technologies |
|---|------|-----------------|
| Task 1 | External API Gateway | `IHttpClientFactory`, Polly, XML/JSON parsing |
| Task 2 | Periodic Job + MERGE SQL | Quartz.NET, SQL MERGE upsert |
| Task 3 | Wallet API + 3 Strategies | ASP.NET Core, `IFundsStrategy`, MediatR |
| Bonus 1 | Redis Caching | `IDistributedCache`, cache-aside pattern |
| Bonus 2 | Rate Limiting | ASP.NET Core `RateLimiter` |
| Required | xUnit Test Suite | Strategies, handlers, gateway |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 / JetBrains Rider
- SQL Server (local or Docker)
- Redis (optional, for Bonus #1)

## Getting Started

```bash
git clone <repo-url>
cd NoviAcademy
dotnet restore
dotnet build
```

## Branching Strategy

- `main` — stable, reviewed code only
- `develop` — integration branch
- `feature/<name>` — individual feature branches, PR into `develop`

## Architecture

This project follows **Clean Architecture** with the dependency rule strictly enforced:
`Api → Application → Domain` (Infrastructure implements interfaces defined in Application/Domain).
