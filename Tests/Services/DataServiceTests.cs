using CloseGuardAIDemo.Web.Models;
using CloseGuardAIDemo.Web.Services;

namespace CloseGuardAIDemo.Tests.Services;

public class DataServiceTests
{
    [Fact]
    public void GetExceptions_DefaultsToCleanDataset()
    {
        var svc = new DataService();
        Assert.Equal(DataSet.Clean, svc.ActiveDataset);
    }

    [Fact]
    public void GetExceptions_CleanDataset_ReturnsTwentyAccounts()
    {
        var svc = new DataService();
        var results = svc.GetExceptions();
        Assert.Equal(20, results.Count);
    }

    [Fact]
    public void SetDataset_SwitchesActiveDataset()
    {
        var svc = new DataService();
        svc.SetDataset(DataSet.Deviated);
        Assert.Equal(DataSet.Deviated, svc.ActiveDataset);
    }

    [Fact]
    public void GetExceptions_DeviatedDataset_ReturnsTwentyAccounts()
    {
        var svc = new DataService();
        svc.SetDataset(DataSet.Deviated);
        var results = svc.GetExceptions();
        Assert.Equal(20, results.Count);
    }

    [Fact]
    public void GetExceptions_DeviatedDataset_HasHigherAverageRiskThanClean()
    {
        var svc = new DataService();

        svc.SetDataset(DataSet.Clean);
        var cleanAvgRisk = svc.GetExceptions().Average(r => r.RiskScore);

        svc.SetDataset(DataSet.Deviated);
        var deviatedAvgRisk = svc.GetExceptions().Average(r => r.RiskScore);

        Assert.True(deviatedAvgRisk > cleanAvgRisk,
            $"Deviated avg risk {deviatedAvgRisk:F3} should exceed clean avg {cleanAvgRisk:F3}");
    }

    [Fact]
    public void GetExceptions_ResultsAreSortedByRiskDescending()
    {
        var svc = new DataService();
        var results = svc.GetExceptions();
        var scores = results.Select(r => r.RiskScore).ToList();
        var sorted = scores.OrderByDescending(s => s).ToList();
        Assert.Equal(sorted, scores);
    }

    [Fact]
    public void GetExceptions_AllResultsHaveValidAccountIds()
    {
        var svc = new DataService();
        var results = svc.GetExceptions();
        Assert.All(results, r => Assert.False(string.IsNullOrEmpty(r.AccountId)));
    }
}
