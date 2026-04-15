# CloseGuard AI Demo — MVP Concept

A good MVP is **not** a full reconciliation platform. It should be a **small web application that proves one idea clearly**:

> Given account balances and a few supporting signals for two or more periods, the system can automatically detect unusual financial situations, rank them by risk, and generate a useful AI explanation with recommended follow-up actions.

That is enough to demonstrate the concept.

## MVP concept

### Working title
**CloseGuard AI Demo**

### Purpose
A lightweight web app for finance users that:

- imports a small financial dataset
- compares current period vs previous period
- detects exceptions and anomalies
- shows a ranked exception list
- lets the user open one exception and see:
  - why it was flagged
  - likely root cause
  - AI-generated explanation
  - recommended next steps

This MVP is for **demonstration**, not full production close management.

---

## 1. What problem the MVP demonstrates

Finance teams often review too many accounts manually. The MVP demonstrates that AI can help answer:

- Which accounts should I look at first?
- What is unusual here?
- Is this probably a timing difference, missing support, or a suspicious manual journal?
- Can the system draft a controller-style explanation instead of making the user write from scratch?

That is the core value.

---

## 2. What the MVP should include

Keep only the minimum features needed to show the concept.

### Feature 1: Import demo data
User uploads one CSV or two CSV files containing:

- company
- account
- previous period balance
- current period balance
- number of manual journals
- amount of largest journal
- number of unmatched items
- days since reconciliation completed
- support document present or not

You can also preload a sample dataset so the demo works instantly.

### Feature 2: Exception scoring
The backend calculates:

- variance amount
- variance percent
- anomaly signals
- overall risk score

Example signals:

- large variance vs previous period
- unusually high manual journal amount
- missing support document
- too many unmatched items
- overdue reconciliation

### Feature 3: Ranked exception list
Show all account-period rows in a grid with:

- account number
- account name
- previous balance
- current balance
- variance
- risk score
- likely cause
- status

Sort by highest risk first.

### Feature 4: Exception details page
When user clicks an exception, show:

- current vs previous balance
- the signals that caused the alert
- a likely root cause
- AI-generated explanation
- recommended follow-up actions

This is the main “wow” screen.

### Feature 5: User feedback
Allow user to click:

- valid issue
- expected variance
- false positive

This proves the concept of learning/tuning later.

---

## 3. What the MVP should NOT include

To keep it small, do **not** build:

- full ERP integration
- full document management
- complex role matrix
- approval workflow
- multi-entity consolidation
- real audit portal
- transaction matching engine
- full multilingual support
- advanced machine learning training pipeline

Those belong in later phases.

---

## 4. MVP user flow

A very simple demo flow:

1. Open app
2. Choose sample company dataset
3. Select current period and previous period
4. Click **Analyze Exceptions**
5. See ranked list of flagged accounts
6. Click one account
7. See:
   - anomaly score
   - drivers
   - AI explanation
   - recommended actions
8. Mark as:
   - investigate
   - expected
   - dismissed

That is enough for a demo in a few minutes.

---

## 5. MVP data model

Use a simplified data model.

### Company

```text
Company
-------
CompanyId
Name
Currency
```

### Period

```text
Period
------
PeriodId
Year
Month
Name
StartDate
EndDate
PreviousPeriodId
```

### Account

```text
Account
-------
AccountId
CompanyId
AccountNumber
AccountName
AccountType
MaterialityThreshold
```

### AccountPeriodSnapshot
This is the core table for the MVP.

```text
AccountPeriodSnapshot
---------------------
SnapshotId
CompanyId
AccountId
PeriodId
PreviousBalance
CurrentBalance
VarianceAmount
VariancePercent
ManualJournalCount
LargestManualJournalAmount
UnmatchedItemCount
DaysToCompleteReconciliation
HasSupportDocument
```

### ExceptionCase

```text
ExceptionCase
-------------
ExceptionCaseId
CompanyId
AccountId
PeriodId
Severity
LikelyCause
OverallRiskScore
Status
CreatedAt
```

### ExceptionSignal

```text
ExceptionSignal
---------------
ExceptionSignalId
ExceptionCaseId
SignalType
ActualValue
ExpectedValue
SignalScore
```

Examples of `SignalType`:

- VarianceMagnitude
- ManualJournalSpike
- MissingSupport
- UnmatchedItemsSpike
- LateCompletion

### AIExplanation

```text
AIExplanation
-------------
AIExplanationId
ExceptionCaseId
SummaryText
DetailedExplanationText
RecommendedActionsText
ModelName
GeneratedAt
IsUserEdited
```

### UserFeedback

```text
UserFeedback
------------
UserFeedbackId
ExceptionCaseId
FeedbackType
Comment
CreatedAt
```

---

## 6. Example demo dataset

One row might look like this:

```text
Company: Nordic Foods AS
Account: 1610
AccountName: Accrued Revenue
PreviousBalance: 4,800,000
CurrentBalance: 8,900,000
ManualJournalCount: 4
LargestManualJournalAmount: 3,100,000
UnmatchedItemCount: 1
DaysToCompleteReconciliation: 9
HasSupportDocument: false
```

The MVP would score this highly because:

- variance is very large
- one manual journal is dominant
- support is missing
- reconciliation is late

Likely cause:

- large one-off journal

AI explanation:

- draft a short controller-style explanation and follow-up note

---

## 7. How anomaly detection works in the MVP

Keep the first version simple and explainable.

### Simple scoring rules

#### Variance score
```text
variance_score = abs(VarianceAmount) / MaterialityThreshold
```

#### Manual journal score
```text
manual_journal_score = LargestManualJournalAmount / MaterialityThreshold
```

#### Missing support score
```text
missing_support_score = 1 if HasSupportDocument = false else 0
```

#### Unmatched items score
```text
unmatched_score = UnmatchedItemCount / 10
```

#### Late completion score
```text
late_score = DaysToCompleteReconciliation / 10
```

#### Combined score
```text
overall_risk =
0.40 * variance_score +
0.25 * manual_journal_score +
0.15 * missing_support_score +
0.10 * unmatched_score +
0.10 * late_score
```

Then map to:

- Low
- Medium
- High

This is enough for a demo and easy to explain to stakeholders.

---

## 8. How AI is used in the MVP

The AI should **not** detect anomalies from raw data by itself.

Instead:

1. Backend calculates the signals and risk score
2. Backend builds a small structured prompt
3. LLM generates:
   - summary
   - detailed explanation
   - follow-up actions

Example prompt payload:

- account name
- previous balance
- current balance
- variance amount
- variance percent
- detected signals
- likely root cause
- output language

Example output:

- Summary: “Accrued Revenue increased materially in the current period.”
- Explanation: “The account increased by 4.1M compared with the previous period, significantly above its materiality threshold. The movement appears primarily driven by one large manual journal of 3.1M. Because supporting documentation is missing, the entry should be reviewed before approval.”
- Recommended actions:
  - verify journal approval
  - request support
  - confirm account classification

That is the demo value.

---

## 9. Recommended MVP architecture

### Frontend
A simple web UI with:

- upload screen
- exception list page
- exception details page

React, Blazor, or plain Razor Pages all work.

### Backend
ASP.NET Core Web API:

- import data
- calculate scores
- create exception cases
- call Azure OpenAI for explanation
- store feedback

### Database
Azure SQL or even SQLite locally for demo purposes.

For Azure-hosted MVP:

- Azure SQL Database
- App Service
- Azure OpenAI

That is enough.

---

## 10. Suggested API endpoints

```text
POST   /api/import
POST   /api/analyze
GET    /api/exceptions
GET    /api/exceptions/{id}
POST   /api/exceptions/{id}/generate-explanation
POST   /api/exceptions/{id}/feedback
```

---

## 11. Best demo scenario

For a short demo, preload 20–30 accounts and make sure 5–7 of them are intentionally interesting:

- one huge balance jump
- one missing support case
- one late reconciliation
- one unusual manual journal case
- one false positive that looks big but is expected seasonality

Then show how the system:

- ranks them
- explains them
- lets the user dismiss expected cases

That makes the concept believable.

---

## 12. MVP success criteria

The MVP is successful if a user can see in under 3 minutes that the system can:

- identify risky exceptions automatically
- explain why they matter
- reduce manual review effort
- provide a useful draft narrative

You do **not** need perfect anomaly science in the MVP. You need a clear proof that the workflow is valuable.

---

## 13. Smallest possible MVP scope

If you want the absolute smallest version:

- no login
- one sample company
- one CSV upload
- one page with exception list
- one details panel with AI explanation

Data model then reduces to just:

- Account
- Period
- AccountPeriodSnapshot
- ExceptionCase
- AIExplanation

That version is enough to demonstrate the concept in 10–15 minutes.
