# CloseGuard AI Demo — Main Screen Design

**Date:** 2026-04-18

## Goal

Implement the main screen with three panels: clean data (left), deviated data (right), and anomaly analysis (bottom). All data lives in memory; the app is stateless.

---

## Architecture

### DataService (singleton)

Registered as a singleton in DI. Holds:

```csharp
List<AccountSnapshot>? CleanData
List<AccountSnapshot>? DeviatedData
List<ExceptionResult>? AnalysisResult
LastGenerated LastGenerated  // enum: None | Clean | Deviated
bool HasData  // true if CleanData or DeviatedData is set

event Action? StateChanged
```

Methods:
- `GenerateClean()` — populates `CleanData`, sets `LastGenerated = Clean`, raises `StateChanged`
- `GenerateDeviated()` — populates `DeviatedData`, sets `LastGenerated = Deviated`, raises `StateChanged`
- `Analyse()` — scores whichever dataset matches `LastGenerated`, populates `AnalysisResult`, raises `StateChanged`

### DataGenerator (static class)

- `GenerateClean()` — 20 accounts, 2 periods, realistic balances with ±5–10% natural variance
- `GenerateDeviated()` — same structure, 5–7 accounts injected with anomalies:
  - large balance jump
  - missing support document
  - dominant manual journal
  - overdue reconciliation
  - too many unmatched items

### ExceptionScorer (static class)

Applies weighted formula to a `List<AccountSnapshot>` and returns `List<ExceptionResult>`:

```
overall_risk = 0.40 * variance_score
             + 0.25 * manual_journal_score
             + 0.15 * missing_support_score
             + 0.10 * unmatched_score
             + 0.10 * late_completion_score
```

Maps score to severity: Low / Medium / High.

---

## Data Types

```csharp
class AccountSnapshot
{
    string AccountId, AccountName, AccountType
    decimal PreviousBalance, CurrentBalance
    decimal VarianceAmount, VariancePercent
    int ManualJournalCount
    decimal LargestManualJournalAmount
    int UnmatchedItemCount
    int DaysToCompleteReconciliation
    bool HasSupportDocument
    decimal MaterialityThreshold
}

class ExceptionResult : AccountSnapshot
{
    decimal RiskScore
    Severity Severity  // enum: Low | Medium | High
    string LikelyCause
}
```

---

## UI Layout

```
┌──────────────────────────────────────────┐
│  MudAppBar (nav buttons)                 │
├───────────────────┬──────────────────────┤
│  Clean Data       │  Deviated Data       │
│  MudDataGrid      │  MudDataGrid         │
│  (left half)      │  (right half)        │
├───────────────────┴──────────────────────┤
│  Anomaly Analysis                        │
│  MudDataGrid — scored exceptions         │
│  sorted by RiskScore desc                │
│  Severity shown as colored MudChip       │
└──────────────────────────────────────────┘
```

### Top panels (left & right)

Columns: Account Name, Account Type, Previous Balance, Current Balance, Variance Amount, Variance %

Empty state: MudText "No data yet" centered in panel.

### Bottom panel

Columns: Account Name, Risk Score, Severity (MudChip: red=High, orange=Medium, green=Low), Likely Cause, Variance Amount

Empty state: MudText "Run analysis to see results".

---

## Navigation Buttons

Buttons live in `MainLayout.razor`. `DataService` is injected into the layout.

| Button | Action | Disabled when |
|--------|--------|---------------|
| Generate Random Data | `DataService.GenerateClean()` | never |
| Generate Deviated Data | `DataService.GenerateDeviated()` | never |
| Analyse Data | `DataService.Analyse()` | `!DataService.HasData` |

Each button shows a `MudProgressCircular` spinner while its operation runs via a local `bool _loading` flag.

---

## State Flow

1. User clicks a nav button → `DataService` method runs → `StateChanged` event fires
2. `Home.razor` subscribes to `DataService.StateChanged` in `OnInitialized`, calls `StateHasChanged()`
3. Panels re-render with updated data
4. `Home.razor` unsubscribes in `Dispose()`

---

## Files to Create / Modify

| File | Change |
|------|--------|
| `Web/Services/DataService.cs` | New — singleton service |
| `Web/Models/AccountSnapshot.cs` | New — data model |
| `Web/Models/ExceptionResult.cs` | New — analysis result model |
| `Web/Services/DataGenerator.cs` | New — random + deviated generation |
| `Web/Services/ExceptionScorer.cs` | New — scoring engine |
| `Web/Components/Pages/Home.razor` | Replace placeholder with three-panel layout |
| `Web/Components/Layout/MainLayout.razor` | Wire buttons to DataService |
| `Web/Program.cs` | Register DataService as singleton |
