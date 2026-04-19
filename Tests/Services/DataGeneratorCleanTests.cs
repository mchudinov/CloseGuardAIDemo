using CloseGuardAIDemo.Web.Models;
using CloseGuardAIDemo.Web.Services;

namespace CloseGuardAIDemo.Tests.Services;

public class DataGeneratorCleanTests
{
    private readonly List<AccountSnapshot> _data = DataGenerator.GenerateClean();

    [Fact]
    public void GenerateClean_Returns20Accounts()
    {
        Assert.Equal(20, _data.Count);
    }

    [Fact]
    public void GenerateClean_AllAccountsHaveUniqueIds()
    {
        var ids = _data.Select(a => a.AccountId).ToList();
        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    [Fact]
    public void GenerateClean_AllAccountsHaveNonEmptyNames()
    {
        Assert.All(_data, a =>
        {
            Assert.NotEmpty(a.AccountId);
            Assert.NotEmpty(a.AccountName);
            Assert.NotEmpty(a.AccountType);
        });
    }

    [Fact]
    public void GenerateClean_AllBalancesArePositive()
    {
        Assert.All(_data, a =>
        {
            Assert.True(a.PreviousBalance > 0);
            Assert.True(a.CurrentBalance > 0);
        });
    }

    [Fact]
    public void GenerateClean_VarianceIsWithinNaturalRange()
    {
        Assert.All(_data, a =>
        {
            Assert.True(Math.Abs(a.VariancePercent) <= 10m,
                $"{a.AccountName} variance {a.VariancePercent}% exceeds ±10%");
        });
    }

    [Fact]
    public void GenerateClean_VarianceAmountMatchesBalanceDifference()
    {
        Assert.All(_data, a =>
        {
            var expected = a.CurrentBalance - a.PreviousBalance;
            Assert.Equal(expected, a.VarianceAmount);
        });
    }

    [Fact]
    public void GenerateClean_AllSignalsWithinNormalRange()
    {
        Assert.All(_data, a =>
        {
            Assert.True(a.ManualJournalCount <= 3, $"{a.AccountName} has too many manual journals");
            Assert.True(a.UnmatchedItemCount <= 2, $"{a.AccountName} has too many unmatched items");
            Assert.True(a.DaysToCompleteReconciliation <= 10, $"{a.AccountName} reconciliation overdue");
            Assert.True(a.HasSupportDocument, $"{a.AccountName} is missing support document");
        });
    }

    [Fact]
    public void GenerateClean_AllMaterialityThresholdsArePositive()
    {
        Assert.All(_data, a => Assert.True(a.MaterialityThreshold > 0));
    }
}
