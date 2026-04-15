# AI-driven Exception Management & Anomaly Detection

AI-driven Exception Management & Anomaly Detection in finance works best as a **pipeline**, not a single model call. The system first computes what is normal for each account or reconciliation, then flags deviations, then classifies likely causes, and only after that uses an LLM to draft a human-readable explanation. In Azure terms, that usually means your .NET backend and SQL layer do the deterministic work, a statistical or ML layer does anomaly scoring, and Azure OpenAI turns the result into controller-facing text or actions.

Also, one Azure anomaly service you might find in older material is being retired on **October 1, 2026**, so for new builds you should avoid designing around Azure AI Anomaly Detector as the long-term core; Microsoft points customers toward Microsoft Fabric or the open-source anomaly-detector project instead. Structured outputs in Azure OpenAI are useful here because they let you force the LLM to return a strict JSON schema for triage results and follow-up actions.

## What the system is actually trying to detect

In a close/reconciliation product, an “exception” is not always the same thing as an “anomaly.”

- An **exception** is usually a known rule failure, such as:
  - balance mismatch above threshold
  - missing support
  - stale reconciliation
  - unmatched items
  - late approval

- An **anomaly** is broader:
  - a data point or pattern that deviates from learned normal behavior, even if no fixed rule was violated

A good system uses both.

- Rule logic catches deterministic breaches.
- Anomaly logic catches unfamiliar or suspicious behavior that static thresholds miss.

## The pipeline, step by step

### 1. Build the “normal baseline”

For each tenant, company, account, and period grain you care about, the system builds a baseline from historical data in Azure SQL.

That baseline should not just be the previous period balance. It should include:

- seasonality
- volatility
- normal posting volume
- normal reversal patterns
- currency effects
- known close-cycle behavior

For example:

- payroll accrual may spike at quarter-end in a very repeatable way
- cash clearing may swing heavily at month-end but settle within a few days

The baseline can be simple at first, such as:

- rolling mean
- rolling standard deviation
- median absolute deviation
- period-of-year comparisons

More advanced versions use time-series models that estimate expected values and confidence bands from historical sequences.

Concretely, for every account-period row you can precompute fields such as:

- `expected_balance`
- `expected_activity`
- `expected_unmatched_item_count`
- `expected_days_open`
- `volatility_score`
- `seasonality_index`
- `materiality_threshold`
- `peer_group_percentile`

These are stored back in SQL or a feature table so your API can reuse them quickly.

### 2. Compute anomaly signals

When the current period arrives, the system compares observed values to the baseline and emits one or more anomaly signals.

In practice you want several:

- **Magnitude anomaly**: balance or activity is much larger or smaller than expected.
- **Rate-of-change anomaly**: the jump from prior period is abnormal even if the absolute balance is not.
- **Composition anomaly**: the account total looks normal, but the makeup changed sharply, such as one vendor now driving 60% of AP accruals.
- **Process anomaly**: unusually high manual overrides, attachments missing, same user preparing and approving, unusually late completion.
- **Behavioral anomaly**: posting pattern, adjustment timing, or override pattern differs from historical norms.

Each signal gets a numeric score, often normalized to `0–1` or `0–100`.

A first implementation does not need deep ML. A robust z-score, percentile-based outlier score, and rules around materiality already get you far.

Example for balance anomaly:

```text
score_balance =
abs(current_balance - expected_balance) / max(volatility_floor, rolling_stddev)
```

Example for process anomaly:

```text
score_process =
weighted_sum(
  late_close_flag,
  missing_support_flag,
  manual_override_count_vs_baseline,
  approval_segregation_flag
)
```

### 3. Combine signals into a risk score

You then combine these signals into a single **exception risk score**. This is the score shown in the UI and used for routing.

It should not be only statistical. In finance, materiality matters more than pure mathematical rarity.

A common formula is:

```text
overall_risk =
0.35 * magnitude_score +
0.20 * change_score +
0.15 * composition_score +
0.20 * process_score +
0.10 * historical_issue_score
```

Then modulate it by materiality:

```text
if abs(variance_amount) < account_materiality_threshold:
    overall_risk *= 0.6
```

And by known expected events:

```text
if period_is_year_end and account in known_year_end_spike_accounts:
    overall_risk *= 0.8
```

This is important because raw anomaly detection often produces false positives. The finance system has to be opinionated about business context.

### 4. Classify likely root causes

Once something is flagged, the system should not stop at “anomaly detected.” It should try to classify **why**.

A practical classifier can start as deterministic logic, then move to ML later.

Typical root-cause categories:

- timing difference
- missing or delayed posting
- reclassification / mapping issue
- FX movement
- large one-off journal
- recurring seasonal movement
- duplicate or split transaction behavior
- missing supporting documentation
- unusual manual override / control issue

This classifier uses the anomaly signals plus transactional summaries.

Examples:

- If there is a large balance increase and 80% is driven by one journal entry posted on the last day of the period, classify as `large_one_off_journal`.
- If AP accrual increased sharply but reverses in the first three days of the next month, classify as `timing_difference`.
- If account total is stable but transaction source mix changed drastically, classify as `mapping_or_source_change`.

You can implement this as a rules engine first, because controllers often prefer explainable logic.

### 5. Group and deduplicate exceptions

A useful system does not send users 50 separate alerts when one upstream issue caused them all. It clusters related exceptions.

Examples:

- same company, same source system, same posting date
- same journal batch
- same vendor/customer
- same account family
- same likely root cause

The outcome is not “23 anomalies.” It becomes:

> 1 incident affecting 23 anomalies, likely caused by delayed subledger feed.

That is what users actually want.

### 6. Generate controller-facing explanation and next actions

Only after the deterministic and statistical work is done should the LLM be called.

Its job is not to detect the anomaly from raw data. Its job is to explain what the detection layer found, in the user’s language, with a stable schema.

This is where Azure OpenAI structured outputs fit well. You pass in:

- scored signals
- likely root cause
- key transaction drivers
- materiality
- account metadata

…and require JSON like this:

```json
{
  "summary": "AR allowance increased unusually in March.",
  "riskLevel": "HIGH",
  "likelyCause": "large_one_off_journal",
  "confidence": 0.82,
  "driverHighlights": [
    "Journal JRN-4481 posted on 2026-03-31 explains 74% of the movement",
    "Manual override count exceeded the six-month average by 3.2x"
  ],
  "recommendedActions": [
    "Verify approval and support for JRN-4481",
    "Confirm whether the posting should remain in account 1290"
  ]
}
```

## What data the model should and should not see

The LLM should receive a **curated anomaly packet**, not arbitrary SQL rows.

Your .NET backend should assemble something like:

- tenant-safe company and account identifiers
- current and prior balance
- expected balance and anomaly band
- variance amount and percent
- top transaction drivers
- process/control flags
- likely root cause candidates
- role-specific visibility scope
- target language

It should usually **not** receive:

- raw PII
- full bank details
- unrestricted free-text comments
- data from outside the user’s tenant or role scope

The important point is that the detection engine runs inside your application boundary; the LLM only explains the result.

## A concrete finance example

Suppose account `1610 Accrued Revenue` for Company A in March 2026 shows:

- February ending balance: `4.8M`
- March expected balance based on history: `5.0M`
- March actual ending balance: `8.9M`
- anomaly band upper limit: `5.9M`
- top driver: one manual journal of `3.1M` on March 31
- support document missing
- preparer override count: `4`, normal average: `1`

Your pipeline might do this:

1. Magnitude score becomes very high because `8.9M` is far outside expected band.
2. Change score is high because month-over-month jump is abnormal.
3. Process score is high because support is missing and overrides are elevated.
4. Classifier labels likely cause as `large_one_off_journal`.
5. Grouping layer links this anomaly with two related revenue recognition accounts touched by the same batch.
6. LLM outputs:

> Accrued Revenue increased by 4.1M from February to March, materially above the expected month-end range. The movement is primarily driven by a 3.1M manual journal posted on March 31, which accounts for most of the variance. Because the entry lacks supporting documentation and override activity was above normal, this item should be reviewed before sign-off.

That explanation is useful because it is based on structured detection results, not model guesswork.

## What models and services to use on Azure

For new builds on Azure, separate the stack into three layers.

### Detection layer

Implement in SQL + .NET first, or Azure ML / Fabric if you want more sophistication later.

If you were considering Azure AI Anomaly Detector, note that Microsoft says the service is being retired on **October 1, 2026** and recommends Fabric or the open-source anomaly-detector path instead.

### Explanation layer

Use Azure OpenAI with structured outputs for strict JSON results.

- JSON mode guarantees valid JSON.
- Structured outputs go further and enforce a supplied schema.

That is better for production workflows.

### Visualization / investigation layer

Use your application UI, Power BI, or similar tools to show:

- actual vs expected
- anomaly boundaries
- driver breakdown
- recommended actions
- incident grouping

## What “good” looks like in the UI

A useful exception panel usually shows one line per incident with:

- risk level
- account and period
- variance vs expected
- likely cause
- confidence
- affected related accounts
- owner
- recommended next action

Then a drill-down page shows:

- actual vs expected trend chart
- top journals or transaction groups
- control/process flags
- AI narrative
- approve / reclassify / request support / dismiss as expected

The dismissal action is important. It teaches the system over time which anomalies are genuine and which are known seasonal behavior.

Even if you do not retrain a formal model immediately, you can feed those decisions back into thresholds and suppression rules.

## Recommended implementation order

Start with a narrow slice:

1. Pick 10–20 high-value account types.
2. Implement baseline + anomaly scoring in SQL/.NET.
3. Add a simple root-cause rules engine.
4. Use Azure OpenAI structured output only for explanation and next steps.
5. Track user actions: accepted, dismissed, reclassified, escalated.
6. Use that feedback to tune weights and suppression logic.

That order matters.

If you start by asking an LLM to “find anomalies” from raw finance data, you will get unstable and hard-to-audit behavior.

If you start with deterministic scoring and let the LLM explain, you get a system that controllers and auditors are much more likely to trust.
