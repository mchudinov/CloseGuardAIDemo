using CloseGuardAIDemo.Web.Models;
using CloseGuardAIDemo.Web.Services;

namespace CloseGuardAIDemo.Tests.Services;

public class ExceptionScorerTests
{
    private static AccountSnapshot BaseAccount(decimal materiality = 1000m) => new()
    {
        AccountId = "ACC-001",
        AccountName = "Test Account",
        AccountType = "Asset",
        PreviousBalance = 10_000m,
        CurrentBalance = 10_000m,
        VarianceAmount = 0m,
        VariancePercent = 0m,
        ManualJournalCount = 0,
        LargestManualJournalAmount = 0m,
        UnmatchedItemCount = 0,
        DaysToCompleteReconciliation = 1,
        HasSupportDocument = true,
        MaterialityThreshold = materiality,
    };

    [Fact]
    public void Score_CleanAccount_ReturnsNearZeroRisk()
    {
        var account = BaseAccount();
        var result = ExceptionScorer.Score(account);

        Assert.True(result.RiskScore < 0.05m, $"Expected near-zero risk, got {result.RiskScore}");
    }

    [Fact]
    public void Score_ReturnsExceptionResultWithAccountData()
    {
        var account = BaseAccount();
        var result = ExceptionScorer.Score(account);

        Assert.Equal("ACC-001", result.AccountId);
        Assert.Equal("Test Account", result.AccountName);
    }

    [Fact]
    public void Score_LargeVariance_DominatesRiskScore()
    {
        var account = BaseAccount(materiality: 1000m);
        account.VarianceAmount = 2000m; // 2x materiality
        var result = ExceptionScorer.Score(account);

        Assert.True(result.RiskScore > 0.35m, $"Large variance should drive high risk, got {result.RiskScore}");
    }

    [Fact]
    public void Score_MissingSupport_ContributesToRisk()
    {
        var account = BaseAccount();
        account.HasSupportDocument = false;
        var result = ExceptionScorer.Score(account);

        Assert.True(result.RiskScore >= 0.15m, $"Missing support should contribute 0.15, got {result.RiskScore}");
    }

    [Fact]
    public void Score_HighManualJournals_ContributesToRisk()
    {
        var account = BaseAccount(materiality: 1000m);
        account.ManualJournalCount = 10;
        account.LargestManualJournalAmount = 5000m; // 5x materiality
        var result = ExceptionScorer.Score(account);

        Assert.True(result.RiskScore > 0.20m, $"High manual journals should contribute, got {result.RiskScore}");
    }

    [Fact]
    public void Score_ManyUnmatchedItems_ContributesToRisk()
    {
        var account = BaseAccount(materiality: 1000m);
        account.UnmatchedItemCount = 10;
        var result = ExceptionScorer.Score(account);

        Assert.True(result.RiskScore > 0.08m, $"Unmatched items should contribute, got {result.RiskScore}");
    }

    [Fact]
    public void Score_OverdueReconciliation_ContributesToRisk()
    {
        var account = BaseAccount();
        account.DaysToCompleteReconciliation = 30;
        var result = ExceptionScorer.Score(account);

        Assert.True(result.RiskScore > 0.08m, $"Overdue reconciliation should contribute, got {result.RiskScore}");
    }

    [Fact]
    public void Score_RiskScoreIsCappedAtOne()
    {
        var account = BaseAccount(materiality: 100m);
        account.VarianceAmount = 1_000_000m;
        account.HasSupportDocument = false;
        account.ManualJournalCount = 100;
        account.LargestManualJournalAmount = 1_000_000m;
        account.UnmatchedItemCount = 100;
        account.DaysToCompleteReconciliation = 999;
        var result = ExceptionScorer.Score(account);

        Assert.True(result.RiskScore <= 1.0m, $"Risk score should never exceed 1.0, got {result.RiskScore}");
    }

    [Fact]
    public void Score_RiskScoreIsNeverNegative()
    {
        var account = BaseAccount();
        var result = ExceptionScorer.Score(account);

        Assert.True(result.RiskScore >= 0m, $"Risk score should never be negative, got {result.RiskScore}");
    }

    [Theory]
    [InlineData(0.0, Severity.Low)]
    [InlineData(0.4, Severity.Low)]
    [InlineData(0.499, Severity.Low)]
    [InlineData(0.5, Severity.Medium)]
    [InlineData(0.74, Severity.Medium)]
    [InlineData(0.75, Severity.High)]
    [InlineData(1.0, Severity.High)]
    public void ToSeverity_MapsRiskBandsCorrectly(double riskDouble, Severity expectedSeverity)
    {
        var severity = ExceptionScorer.ToSeverity((decimal)riskDouble);
        Assert.Equal(expectedSeverity, severity);
    }

    [Fact]
    public void Score_LikelyCauseIsNonEmptyForAnomalousAccount()
    {
        var account = BaseAccount();
        account.HasSupportDocument = false;
        var result = ExceptionScorer.Score(account);

        Assert.NotEmpty(result.LikelyCause);
    }

    [Fact]
    public void Score_LikelyCauseIsEmptyForCleanAccount()
    {
        var account = BaseAccount();
        var result = ExceptionScorer.Score(account);

        Assert.Empty(result.LikelyCause);
    }
}
