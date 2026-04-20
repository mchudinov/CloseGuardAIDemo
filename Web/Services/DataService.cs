using CloseGuardAIDemo.Web.Models;

namespace CloseGuardAIDemo.Web.Services;

public enum DataSet { Clean, Deviated }

public class DataService
{
    public DataSet ActiveDataset { get; private set; } = DataSet.Clean;

    public void SetDataset(DataSet dataset) => ActiveDataset = dataset;

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
