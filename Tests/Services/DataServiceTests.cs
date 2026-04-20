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

    [Fact]
    public void CleanData_InitiallyNull()
    {
        var svc = new DataService();
        Assert.Null(svc.CleanData);
    }

    [Fact]
    public void DeviatedData_InitiallyNull()
    {
        var svc = new DataService();
        Assert.Null(svc.DeviatedData);
    }

    [Fact]
    public void SetDataset_Clean_PopulatesCleanData()
    {
        var svc = new DataService();
        svc.SetDataset(DataSet.Clean);
        Assert.NotNull(svc.CleanData);
        Assert.Equal(20, svc.CleanData!.Count);
    }

    [Fact]
    public void SetDataset_Deviated_PopulatesDeviatedData()
    {
        var svc = new DataService();
        svc.SetDataset(DataSet.Deviated);
        Assert.NotNull(svc.DeviatedData);
        Assert.Equal(20, svc.DeviatedData!.Count);
    }

    [Fact]
    public void SetDataset_Clean_DoesNotClearDeviatedData()
    {
        var svc = new DataService();
        svc.SetDataset(DataSet.Deviated);
        svc.SetDataset(DataSet.Clean);
        Assert.NotNull(svc.DeviatedData);
    }

    [Fact]
    public void SetDataset_Deviated_DoesNotClearCleanData()
    {
        var svc = new DataService();
        svc.SetDataset(DataSet.Clean);
        svc.SetDataset(DataSet.Deviated);
        Assert.NotNull(svc.CleanData);
    }

    [Fact]
    public void AnalysisResults_InitiallyNull()
    {
        var svc = new DataService();
        Assert.Null(svc.AnalysisResults);
    }

    [Fact]
    public void Analyse_PopulatesAnalysisResults_WithTwentyItems()
    {
        var svc = new DataService();
        svc.Analyse();
        Assert.NotNull(svc.AnalysisResults);
        Assert.Equal(20, svc.AnalysisResults!.Count);
    }

    [Fact]
    public void Analyse_ResultsAreSortedByRiskScoreDescending()
    {
        var svc = new DataService();
        svc.Analyse();
        var scores = svc.AnalysisResults!.Select(r => r.RiskScore).ToList();
        Assert.Equal(scores.OrderByDescending(s => s).ToList(), scores);
    }

    [Fact]
    public void Analyse_RaisesStateChangedEvent()
    {
        var svc = new DataService();
        var raised = false;
        svc.StateChanged += () => raised = true;
        svc.Analyse();
        Assert.True(raised);
    }

    [Fact]
    public void SetDataset_Clean_RaisesStateChangedEvent()
    {
        var svc = new DataService();
        var raised = false;
        svc.StateChanged += () => raised = true;
        svc.SetDataset(DataSet.Clean);
        Assert.True(raised);
    }

    [Fact]
    public void SetDataset_Deviated_RaisesStateChangedEvent()
    {
        var svc = new DataService();
        var raised = false;
        svc.StateChanged += () => raised = true;
        svc.SetDataset(DataSet.Deviated);
        Assert.True(raised);
    }

    [Fact]
    public void SetDataset_Clean_ClearsAnalysisResults()
    {
        var svc = new DataService();
        svc.Analyse();
        Assert.NotNull(svc.AnalysisResults);

        svc.SetDataset(DataSet.Clean);

        Assert.Null(svc.AnalysisResults);
    }

    [Fact]
    public void SetDataset_Deviated_ClearsAnalysisResults()
    {
        var svc = new DataService();
        svc.Analyse();
        Assert.NotNull(svc.AnalysisResults);

        svc.SetDataset(DataSet.Deviated);

        Assert.Null(svc.AnalysisResults);
    }

    [Fact]
    public void Reset_ClearsCleanData()
    {
        var svc = new DataService();
        svc.SetDataset(DataSet.Clean);
        svc.Reset();
        Assert.Null(svc.CleanData);
    }

    [Fact]
    public void Reset_ClearsDeviatedData()
    {
        var svc = new DataService();
        svc.SetDataset(DataSet.Deviated);
        svc.Reset();
        Assert.Null(svc.DeviatedData);
    }

    [Fact]
    public void Reset_ClearsAnalysisResults()
    {
        var svc = new DataService();
        svc.Analyse();
        svc.Reset();
        Assert.Null(svc.AnalysisResults);
    }

    [Fact]
    public void Reset_ResetsActiveDatasetToClean()
    {
        var svc = new DataService();
        svc.SetDataset(DataSet.Deviated);
        svc.Reset();
        Assert.Equal(DataSet.Clean, svc.ActiveDataset);
    }

    [Fact]
    public void Reset_RaisesStateChangedEvent()
    {
        var svc = new DataService();
        var raised = false;
        svc.StateChanged += () => raised = true;
        svc.Reset();
        Assert.True(raised);
    }
}
