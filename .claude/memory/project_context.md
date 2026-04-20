---
name: Project Context
description: CloseGuardAIDemo architecture, tech stack, and current state
type: project
originSessionId: 031220a4-d066-4180-8228-30a3b7761532
---
**CloseGuardAIDemo** — Blazor Server MVP for AI-driven financial exception management.

- Solution: `CloseGuardAIDemo.slnx` (two projects: `Web/`, `Library/`)
- Stack: ASP.NET Core 10, Blazor Server (interactive SSR), MudBlazor 9.3, Serilog, xUnit
- Test project: `Tests/CloseGuardAIDemo.Tests.csproj` — references `Web.csproj`
- Runs on http://localhost:8085 (`cd Web && dotnet run`)

**Risk scoring formula:**
```
overall_risk = 0.40 * variance_score
             + 0.25 * manual_journal_score
             + 0.15 * missing_support_score
             + 0.10 * unmatched_score
             + 0.10 * late_completion_score
```
Each component normalized against `MaterialityThreshold`.

**Steps completed (on main):** 1–4 (AccountSnapshot, ExceptionResult, DataGenerator.GenerateClean, DataGenerator.GenerateDeviated)

**Steps remaining:** 5 (ExceptionScorer), 6 (DataService singleton), 7 (DI registration), 8 (MainLayout buttons), 9 (Home top panels), 10 (Home bottom panel)

**Why:** Demo for AI-driven close process; LLM generates narratives, backend does all scoring.
**How to apply:** Suggest changes aligned with the scoring formula and Blazor Server patterns; always check step order before implementing.
