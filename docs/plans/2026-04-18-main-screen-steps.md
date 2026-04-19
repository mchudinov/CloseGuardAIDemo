# CloseGuard AI Demo — Main Screen Implementation Steps

**Date:** 2026-04-19  
**Source plan:** [2026-04-18-main-screen-design.md](./2026-04-18-main-screen-design.md)

## Steps

| Step | Title | File(s) |
|------|-------|---------|
| Step-1 | Create AccountSnapshot data model | `Web/Models/AccountSnapshot.cs` |
| Step-2 | Create ExceptionResult model and Severity enum | `Web/Models/ExceptionResult.cs` |
| Step-3 | Implement DataGenerator.GenerateClean() | `Web/Services/DataGenerator.cs` |
| Step-4 | Implement DataGenerator.GenerateDeviated() with anomalies | `Web/Services/DataGenerator.cs` |
| Step-5 | Implement ExceptionScorer with weighted risk formula | `Web/Services/ExceptionScorer.cs` |
| Step-6 | Create DataService singleton with state and events | `Web/Services/DataService.cs` |
| Step-7 | Register DataService in Program.cs DI container | `Web/Program.cs` |
| Step-8 | Wire MainLayout.razor buttons to DataService | `Web/Components/Layout/MainLayout.razor` |
| Step-9 | Implement Home.razor top panels (clean and deviated data grids) | `Web/Components/Pages/Home.razor` |
| Step-10 | Implement Home.razor bottom analysis panel with state subscription | `Web/Components/Pages/Home.razor` |

## Step Details

### Step-1 — Create AccountSnapshot data model
`Web/Models/AccountSnapshot.cs`

Fields: `AccountId`, `AccountName`, `AccountType`, `PreviousBalance`, `CurrentBalance`, `VarianceAmount`, `VariancePercent`, `ManualJournalCount`, `LargestManualJournalAmount`, `UnmatchedItemCount`, `DaysToCompleteReconciliation`, `HasSupportDocument`, `MaterialityThreshold`.

---

### Step-2 — Create ExceptionResult model and Severity enum
`Web/Models/ExceptionResult.cs`

Extends `AccountSnapshot`. Adds: `RiskScore` (decimal), `Severity` (enum: Low / Medium / High), `LikelyCause` (string).

---

### Step-3 — Implement DataGenerator.GenerateClean()
`Web/Services/DataGenerator.cs`

Static class. `GenerateClean()` returns `List<AccountSnapshot>` with 20 accounts across 2 periods, realistic balances with ±5–10% natural variance, all signals within normal range.

---

### Step-4 — Implement DataGenerator.GenerateDeviated() with anomalies
`Web/Services/DataGenerator.cs`

`GenerateDeviated()` returns same structure as clean but with 5–7 accounts injected with anomalies: large balance jump, missing support document, dominant manual journal, overdue reconciliation, too many unmatched items.

---

### Step-5 — Implement ExceptionScorer with weighted risk formula
`Web/Services/ExceptionScorer.cs`

Static class. `Score(List<AccountSnapshot>)` returns `List<ExceptionResult>` using:
```
overall_risk = 0.40 * variance_score
             + 0.25 * manual_journal_score
             + 0.15 * missing_support_score
             + 0.10 * unmatched_score
             + 0.10 * late_completion_score
```
Each component normalized against `MaterialityThreshold`. Maps score to Severity.

---

### Step-6 — Create DataService singleton with state and events
`Web/Services/DataService.cs`

Singleton. State: `CleanData`, `DeviatedData`, `AnalysisResult`, `LastGenerated` (enum: None/Clean/Deviated), `HasData`. Event: `StateChanged`. Methods: `GenerateClean()`, `GenerateDeviated()`, `Analyse()`.

---

### Step-7 — Register DataService in Program.cs DI container
`Web/Program.cs`

Add `builder.Services.AddSingleton<DataService>()`.

---

### Step-8 — Wire MainLayout.razor buttons to DataService
`Web/Components/Layout/MainLayout.razor`

Inject `DataService`. Add three `MudButton`s in the `MudAppBar`: Generate Random Data, Generate Deviated Data, Analyse Data (disabled when `!HasData`). Each shows a `MudProgressCircular` spinner via a local `bool _loading` flag while its operation runs.

---

### Step-9 — Implement Home.razor top panels (clean and deviated data grids)
`Web/Components/Pages/Home.razor`

Two side-by-side `MudDataGrid` panels. Columns: Account Name, Account Type, Previous Balance, Current Balance, Variance Amount, Variance %. Empty state: centered `MudText "No data yet"`.

---

### Step-10 — Implement Home.razor bottom analysis panel with state subscription
`Web/Components/Pages/Home.razor`

Bottom `MudDataGrid` with columns: Account Name, Risk Score, Severity (colored `MudChip`), Likely Cause, Variance Amount. Sorted by `RiskScore` descending. Empty state: `"Run analysis to see results"`. Subscribe to `DataService.StateChanged` in `OnInitialized`, call `StateHasChanged()` on event, unsubscribe in `Dispose()`.
