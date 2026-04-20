using CloseGuardAIDemo.Web.Models;

namespace CloseGuardAIDemo.Web.Services;

public enum DataSet { Clean, Deviated }

public class DataService
{
    public DataSet ActiveDataset { get; private set; } = DataSet.Clean;
    public IReadOnlyList<AccountSnapshot>? CleanData { get; private set; }
    public IReadOnlyList<AccountSnapshot>? DeviatedData { get; private set; }
    public IReadOnlyList<ExceptionResult>? AnalysisResults { get; private set; }

    public event Action? StateChanged;

    public void SetDataset(DataSet dataset)
    {
        ActiveDataset = dataset;
        if (dataset == DataSet.Clean)
            CleanData = DataGenerator.GenerateClean();
        else
            DeviatedData = DataGenerator.GenerateDeviated();
        StateChanged?.Invoke();
    }

    public void Analyse()
    {
        AnalysisResults = GetExceptions();
        StateChanged?.Invoke();
    }

    public IReadOnlyList<ExceptionResult> GetExceptions()
    {
        var snapshots = ActiveDataset == DataSet.Deviated
            ? DataGenerator.GenerateDeviated()
            : DataGenerator.GenerateClean();

        return snapshots
            .Select(ExceptionScorer.Score)
            .OrderByDescending(r => r.RiskScore)
            .ToList();
    }
}
