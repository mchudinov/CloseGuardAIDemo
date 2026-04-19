using CloseGuardAIDemo.Web.Models;

namespace CloseGuardAIDemo.Tests.Models;

public class AccountSnapshotTests
{
    [Fact]
    public void AccountSnapshot_DefaultValues_AreCorrect()
    {
        var snapshot = new AccountSnapshot();

        Assert.Equal(string.Empty, snapshot.AccountId);
        Assert.Equal(string.Empty, snapshot.AccountName);
        Assert.Equal(string.Empty, snapshot.AccountType);
        Assert.Equal(0m, snapshot.PreviousBalance);
        Assert.Equal(0m, snapshot.CurrentBalance);
        Assert.Equal(0m, snapshot.VarianceAmount);
        Assert.Equal(0m, snapshot.VariancePercent);
        Assert.Equal(0, snapshot.ManualJournalCount);
        Assert.Equal(0m, snapshot.LargestManualJournalAmount);
        Assert.Equal(0, snapshot.UnmatchedItemCount);
        Assert.Equal(0, snapshot.DaysToCompleteReconciliation);
        Assert.False(snapshot.HasSupportDocument);
        Assert.Equal(0m, snapshot.MaterialityThreshold);
    }

    [Fact]
    public void AccountSnapshot_CanSetAllProperties()
    {
        var snapshot = new AccountSnapshot
        {
            AccountId = "ACC-001",
            AccountName = "Cash",
            AccountType = "Asset",
            PreviousBalance = 10_000m,
            CurrentBalance = 12_500m,
            VarianceAmount = 2_500m,
            VariancePercent = 25m,
            ManualJournalCount = 3,
            LargestManualJournalAmount = 1_500m,
            UnmatchedItemCount = 2,
            DaysToCompleteReconciliation = 5,
            HasSupportDocument = true,
            MaterialityThreshold = 5_000m
        };

        Assert.Equal("ACC-001", snapshot.AccountId);
        Assert.Equal("Cash", snapshot.AccountName);
        Assert.Equal("Asset", snapshot.AccountType);
        Assert.Equal(10_000m, snapshot.PreviousBalance);
        Assert.Equal(12_500m, snapshot.CurrentBalance);
        Assert.Equal(2_500m, snapshot.VarianceAmount);
        Assert.Equal(25m, snapshot.VariancePercent);
        Assert.Equal(3, snapshot.ManualJournalCount);
        Assert.Equal(1_500m, snapshot.LargestManualJournalAmount);
        Assert.Equal(2, snapshot.UnmatchedItemCount);
        Assert.Equal(5, snapshot.DaysToCompleteReconciliation);
        Assert.True(snapshot.HasSupportDocument);
        Assert.Equal(5_000m, snapshot.MaterialityThreshold);
    }
}
