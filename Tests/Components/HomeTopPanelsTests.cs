using Bunit;
using CloseGuardAIDemo.Web.Services;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Web.Components.Pages;

namespace CloseGuardAIDemo.Tests.Components;

public class HomeTopPanelsTests : IAsyncLifetime
{
    private BunitContext _ctx = null!;

    public Task InitializeAsync()
    {
        _ctx = new BunitContext();
        _ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        _ctx.Services.AddMudServices();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync() => await _ctx.DisposeAsync();

    [Fact]
    public void LeftPanel_WhenNoCleanData_ShowsNoDataYet()
    {
        var dataService = new DataService();
        _ctx.Services.AddSingleton(dataService);

        var cut = _ctx.Render<Home>();

        Assert.Contains("No data yet", cut.Find("[data-testid='clean-panel']").TextContent);
    }

    [Fact]
    public void RightPanel_WhenNoDeviatedData_ShowsNoDataYet()
    {
        var dataService = new DataService();
        _ctx.Services.AddSingleton(dataService);

        var cut = _ctx.Render<Home>();

        Assert.Contains("No data yet", cut.Find("[data-testid='deviated-panel']").TextContent);
    }

    [Fact]
    public void LeftPanel_AfterCleanDataGenerated_HidesNoDataYet()
    {
        var dataService = new DataService();
        dataService.SetDataset(DataSet.Clean);
        _ctx.Services.AddSingleton(dataService);

        var cut = _ctx.Render<Home>();

        Assert.DoesNotContain("No data yet", cut.Find("[data-testid='clean-panel']").TextContent);
    }

    [Fact]
    public void RightPanel_AfterDeviatedDataGenerated_HidesNoDataYet()
    {
        var dataService = new DataService();
        dataService.SetDataset(DataSet.Deviated);
        _ctx.Services.AddSingleton(dataService);

        var cut = _ctx.Render<Home>();

        Assert.DoesNotContain("No data yet", cut.Find("[data-testid='deviated-panel']").TextContent);
    }

    [Fact]
    public void LeftPanel_AfterCleanDataGenerated_ShowsTwentyRows()
    {
        var dataService = new DataService();
        dataService.SetDataset(DataSet.Clean);
        _ctx.Services.AddSingleton(dataService);

        var cut = _ctx.Render<Home>();

        var rows = cut.Find("[data-testid='clean-panel']").QuerySelectorAll("tbody tr.mud-table-row");
        Assert.Equal(20, rows.Length);
    }

    [Fact]
    public void RightPanel_AfterDeviatedDataGenerated_ShowsTwentyRows()
    {
        var dataService = new DataService();
        dataService.SetDataset(DataSet.Deviated);
        _ctx.Services.AddSingleton(dataService);

        var cut = _ctx.Render<Home>();

        var rows = cut.Find("[data-testid='deviated-panel']").QuerySelectorAll("tbody tr.mud-table-row");
        Assert.Equal(20, rows.Length);
    }

    [Fact]
    public void LeftPanel_AfterSetDatasetClean_ReactsToStateChanged_ShowsData()
    {
        var dataService = new DataService();
        _ctx.Services.AddSingleton(dataService);

        var cut = _ctx.Render<Home>();
        Assert.Contains("No data yet", cut.Find("[data-testid='clean-panel']").TextContent);

        dataService.SetDataset(DataSet.Clean);
        cut.WaitForState(() =>
            !cut.Find("[data-testid='clean-panel']").TextContent.Contains("No data yet"));

        Assert.DoesNotContain("No data yet", cut.Find("[data-testid='clean-panel']").TextContent);
    }

    [Fact]
    public void RightPanel_AfterSetDatasetDeviated_ReactsToStateChanged_ShowsData()
    {
        var dataService = new DataService();
        _ctx.Services.AddSingleton(dataService);

        var cut = _ctx.Render<Home>();
        Assert.Contains("No data yet", cut.Find("[data-testid='deviated-panel']").TextContent);

        dataService.SetDataset(DataSet.Deviated);
        cut.WaitForState(() =>
            !cut.Find("[data-testid='deviated-panel']").TextContent.Contains("No data yet"));

        Assert.DoesNotContain("No data yet", cut.Find("[data-testid='deviated-panel']").TextContent);
    }

    [Fact]
    public void CleanPanel_TableHeaders_AreBold()
    {
        var dataService = new DataService();
        dataService.SetDataset(DataSet.Clean);
        _ctx.Services.AddSingleton(dataService);

        var cut = _ctx.Render<Home>();

        var headers = cut.Find("[data-testid='clean-panel']").QuerySelectorAll("th b");
        Assert.True(headers.Length > 0, "Expected bold <b> elements inside table headers");
    }

    [Fact]
    public void DeviatedPanel_TableHeaders_AreBold()
    {
        var dataService = new DataService();
        dataService.SetDataset(DataSet.Deviated);
        _ctx.Services.AddSingleton(dataService);

        var cut = _ctx.Render<Home>();

        var headers = cut.Find("[data-testid='deviated-panel']").QuerySelectorAll("th b");
        Assert.True(headers.Length > 0, "Expected bold <b> elements inside table headers");
    }

    [Fact]
    public void AnalysisPanel_TableHeaders_AreBold()
    {
        var dataService = new DataService();
        dataService.Analyse();
        _ctx.Services.AddSingleton(dataService);

        var cut = _ctx.Render<Home>();

        var headers = cut.Find("[data-testid='analysis-panel']").QuerySelectorAll("th b");
        Assert.True(headers.Length > 0, "Expected bold <b> elements inside table headers");
    }
}
