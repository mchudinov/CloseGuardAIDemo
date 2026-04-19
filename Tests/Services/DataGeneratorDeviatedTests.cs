using CloseGuardAIDemo.Web.Models;
using CloseGuardAIDemo.Web.Services;

namespace CloseGuardAIDemo.Tests.Services;

public class DataGeneratorDeviatedTests
{
    private readonly List<AccountSnapshot> _data = DataGenerator.GenerateDeviated();

    [Fact]
    public void GenerateDeviated_Returns20Accounts()
    {
        Assert.Equal(20, _data.Count);
    }

    [Fact]
    public void GenerateDeviated_AllAccountsHaveUniqueIds()
    {
        var ids = _data.Select(a => a.AccountId).ToList();
        Assert.Equal(ids.Count, ids.Distinct().Count());
    }

    [Fact]
    public void GenerateDeviated_HasBetween5And7AnomalousAccounts()
    {
        var anomalous = _data.Count(a =>
            Math.Abs(a.VariancePercent) > 10m ||
            !a.HasSupportDocument ||
            a.ManualJournalCount > 3 ||
            a.UnmatchedItemCount > 2 ||
            a.DaysToCompleteReconciliation > 10);

        Assert.InRange(anomalous, 5, 7);
    }

    [Fact]
    public void GenerateDeviated_HasAtLeastOneLargeBalanceJump()
    {
        Assert.Contains(_data, a => Math.Abs(a.VariancePercent) > 20m);
    }

    [Fact]
    public void GenerateDeviated_HasAtLeastOneMissingSupportDocument()
    {
        Assert.Contains(_data, a => !a.HasSupportDocument);
    }

    [Fact]
    public void GenerateDeviated_HasAtLeastOneDominantManualJournal()
    {
        Assert.Contains(_data, a => a.ManualJournalCount > 3);
    }

    [Fact]
    public void GenerateDeviated_HasAtLeastOneOverdueReconciliation()
    {
        Assert.Contains(_data, a => a.DaysToCompleteReconciliation > 10);
    }

    [Fact]
    public void GenerateDeviated_HasAtLeastOneTooManyUnmatchedItems()
    {
        Assert.Contains(_data, a => a.UnmatchedItemCount > 2);
    }

    [Fact]
    public void GenerateDeviated_VarianceAmountMatchesBalanceDifference()
    {
        Assert.All(_data, a =>
        {
            var expected = a.CurrentBalance - a.PreviousBalance;
            Assert.Equal(expected, a.VarianceAmount);
        });
    }

    [Fact]
    public void GenerateDeviated_AllBalancesArePositive()
    {
        Assert.All(_data, a =>
        {
            Assert.True(a.PreviousBalance > 0);
            Assert.True(a.CurrentBalance > 0);
        });
    }

    [Fact]
    public void GenerateDeviated_AllMaterialityThresholdsArePositive()
    {
        Assert.All(_data, a => Assert.True(a.MaterialityThreshold > 0));
    }
}
