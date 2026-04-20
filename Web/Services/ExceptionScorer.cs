using CloseGuardAIDemo.Web.Models;

namespace CloseGuardAIDemo.Web.Services;

public static class ExceptionScorer
{
    private const decimal VarianceWeight      = 0.40m;
    private const decimal ManualJournalWeight = 0.25m;
    private const decimal MissingSupportWeight = 0.15m;
    private const decimal UnmatchedWeight     = 0.10m;
    private const decimal LateCompletionWeight = 0.10m;

    private const int NormalManualJournalMax = 3;
    private const int NormalUnmatchedMax     = 2;
    private const int NormalDaysMax          = 10;

    public static ExceptionResult Score(AccountSnapshot account)
    {
        var materiality = account.MaterialityThreshold > 0 ? account.MaterialityThreshold : 1m;

        var varianceScore      = Clamp(Math.Abs(account.VarianceAmount) / materiality);
        var missingSupport     = account.HasSupportDocument ? 0m : 1m;
        var manualJournalScore = Clamp(account.LargestManualJournalAmount / materiality)
                                 * Clamp((decimal)account.ManualJournalCount / NormalManualJournalMax);
        var unmatchedScore     = Clamp((decimal)account.UnmatchedItemCount / NormalUnmatchedMax);
        var lateScore          = Clamp((decimal)(account.DaysToCompleteReconciliation - NormalDaysMax)
                                       / NormalDaysMax);

        var risk = Clamp(
            VarianceWeight       * varianceScore
            + ManualJournalWeight * manualJournalScore
            + MissingSupportWeight * missingSupport
            + UnmatchedWeight     * unmatchedScore
            + LateCompletionWeight * lateScore);

        return new ExceptionResult
        {
            AccountId                    = account.AccountId,
            AccountName                  = account.AccountName,
            AccountType                  = account.AccountType,
            PreviousBalance              = account.PreviousBalance,
            CurrentBalance               = account.CurrentBalance,
            VarianceAmount               = account.VarianceAmount,
            VariancePercent              = account.VariancePercent,
            ManualJournalCount           = account.ManualJournalCount,
            LargestManualJournalAmount   = account.LargestManualJournalAmount,
            UnmatchedItemCount           = account.UnmatchedItemCount,
            DaysToCompleteReconciliation = account.DaysToCompleteReconciliation,
            HasSupportDocument           = account.HasSupportDocument,
            MaterialityThreshold         = account.MaterialityThreshold,
            RiskScore                    = risk,
            Severity                     = ToSeverity(risk),
            LikelyCause                  = BuildCause(account, varianceScore, missingSupport, manualJournalScore, unmatchedScore, lateScore),
        };
    }

    public static Severity ToSeverity(decimal risk) => risk switch
    {
        >= 0.75m => Severity.High,
        >= 0.50m => Severity.Medium,
        _        => Severity.Low,
    };

    private static string BuildCause(
        AccountSnapshot a,
        decimal varianceScore, decimal missingSupport,
        decimal manualJournalScore, decimal unmatchedScore, decimal lateScore)
    {
        var causes = new List<string>();

        if (varianceScore > 0.5m)
            causes.Add("large balance variance");
        if (missingSupport > 0)
            causes.Add("missing support document");
        if (manualJournalScore > 0.5m)
            causes.Add("high manual journal activity");
        if (unmatchedScore > 0.5m)
            causes.Add("many unmatched items");
        if (lateScore > 0.5m)
            causes.Add("overdue reconciliation");

        return causes.Count == 0 ? string.Empty : string.Join(", ", causes);
    }

    private static decimal Clamp(decimal value) => Math.Max(0m, Math.Min(1m, value));
}
