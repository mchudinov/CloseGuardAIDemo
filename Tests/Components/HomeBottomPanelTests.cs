using Bunit;
using CloseGuardAIDemo.Web.Services;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Web.Components.Pages;

namespace CloseGuardAIDemo.Tests.Components;

public class HomeBottomPanelTests : IAsyncLifetime
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
    public void BottomPanel_Initially_ShowsRunAnalysisMessage()
    {
        var dataService = new DataService();
        _ctx.Services.AddSingleton(dataService);

        var cut = _ctx.Render<Home>();

        Assert.Contains("Run analysis to see results",
            cut.Find("[data-testid='analysis-panel']").TextContent);
    }

    [Fact]
    public void BottomPanel_AfterAnalyse_HidesRunAnalysisMessage()
    {
        var dataService = new DataService();
        dataService.Analyse();
        _ctx.Services.AddSingleton(dataService);

        var cut = _ctx.Render<Home>();

        Assert.DoesNotContain("Run analysis to see results",
            cut.Find("[data-testid='analysis-panel']").TextContent);
    }

    [Fact]
    public void BottomPanel_AfterAnalyse_ShowsTwentyRows()
    {
        var dataService = new DataService();
        dataService.Analyse();
        _ctx.Services.AddSingleton(dataService);

        var cut = _ctx.Render<Home>();

        var rows = cut.Find("[data-testid='analysis-panel']")
            .QuerySelectorAll("tbody tr.mud-table-row");
        Assert.Equal(20, rows.Length);
    }

    [Fact]
    public void BottomPanel_AfterSetDatasetClean_RevertsToRunAnalysisMessage()
    {
        var dataService = new DataService();
        dataService.Analyse();
        _ctx.Services.AddSingleton(dataService);

        var cut = _ctx.Render<Home>();
        Assert.DoesNotContain("Run analysis to see results",
            cut.Find("[data-testid='analysis-panel']").TextContent);

        dataService.SetDataset(DataSet.Clean);
        cut.WaitForState(() =>
            cut.Find("[data-testid='analysis-panel']").TextContent
                .Contains("Run analysis to see results"));

        Assert.Contains("Run analysis to see results",
            cut.Find("[data-testid='analysis-panel']").TextContent);
    }

    [Fact]
    public void BottomPanel_AfterStateChangedFires_RendersUpdatedResults()
    {
        var dataService = new DataService();
        _ctx.Services.AddSingleton(dataService);

        var cut = _ctx.Render<Home>();

        Assert.Contains("Run analysis to see results",
            cut.Find("[data-testid='analysis-panel']").TextContent);

        dataService.Analyse();
        cut.WaitForState(() =>
            !cut.Find("[data-testid='analysis-panel']").TextContent
                .Contains("Run analysis to see results"));

        Assert.DoesNotContain("Run analysis to see results",
            cut.Find("[data-testid='analysis-panel']").TextContent);
    }
}
