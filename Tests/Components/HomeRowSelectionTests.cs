using Bunit;
using CloseGuardAIDemo.Web.Services;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Web.Components.Pages;

namespace CloseGuardAIDemo.Tests.Components;

public class HomeRowSelectionTests : IAsyncLifetime
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

    private static IRenderedComponent<Home> RenderWithAllData(BunitContext ctx)
    {
        var dataService = new DataService();
        dataService.SetDataset(DataSet.Clean);
        dataService.SetDataset(DataSet.Deviated);
        dataService.Analyse();
        ctx.Services.AddSingleton(dataService);
        return ctx.Render<Home>();
    }

    [Fact]
    public void AnalysisPanel_ClickRow_HighlightsSelectedRowInAnalysisPanel()
    {
        var cut = RenderWithAllData(_ctx);

        var firstRow = cut.Find("[data-testid='analysis-panel'] tbody tr.mud-table-row");
        firstRow.Click();

        cut.WaitForState(() =>
            cut.Find("[data-testid='analysis-panel'] tbody tr.mud-table-row")
               .ClassList.Contains("selected-row"));

        Assert.Contains("selected-row",
            cut.Find("[data-testid='analysis-panel'] tbody tr.mud-table-row").ClassList);
    }

    [Fact]
    public void AnalysisPanel_ClickRow_HighlightsMatchingRowInCleanPanel()
    {
        var cut = RenderWithAllData(_ctx);

        var firstAnalysisRow = cut.Find("[data-testid='analysis-panel'] tbody tr.mud-table-row");
        firstAnalysisRow.Click();

        cut.WaitForState(() =>
            cut.FindAll("[data-testid='clean-panel'] tbody tr.mud-table-row")
               .Any(r => r.ClassList.Contains("selected-row")));

        var highlighted = cut.FindAll("[data-testid='clean-panel'] tbody tr.mud-table-row")
                             .Where(r => r.ClassList.Contains("selected-row"))
                             .ToList();
        Assert.Single(highlighted);
    }

    [Fact]
    public void AnalysisPanel_ClickRow_HighlightsMatchingRowInDeviatedPanel()
    {
        var cut = RenderWithAllData(_ctx);

        var firstAnalysisRow = cut.Find("[data-testid='analysis-panel'] tbody tr.mud-table-row");
        firstAnalysisRow.Click();

        cut.WaitForState(() =>
            cut.FindAll("[data-testid='deviated-panel'] tbody tr.mud-table-row")
               .Any(r => r.ClassList.Contains("selected-row")));

        var highlighted = cut.FindAll("[data-testid='deviated-panel'] tbody tr.mud-table-row")
                             .Where(r => r.ClassList.Contains("selected-row"))
                             .ToList();
        Assert.Single(highlighted);
    }

    [Fact]
    public void AnalysisPanel_ClickSameRowTwice_ClearsSelectionInAllPanels()
    {
        var cut = RenderWithAllData(_ctx);

        var firstRow = cut.Find("[data-testid='analysis-panel'] tbody tr.mud-table-row");
        firstRow.Click();
        cut.WaitForState(() =>
            cut.Find("[data-testid='analysis-panel'] tbody tr.mud-table-row")
               .ClassList.Contains("selected-row"));

        cut.Find("[data-testid='analysis-panel'] tbody tr.mud-table-row").Click();
        cut.WaitForState(() =>
            !cut.Find("[data-testid='analysis-panel'] tbody tr.mud-table-row")
                .ClassList.Contains("selected-row"));

        Assert.DoesNotContain("selected-row",
            cut.Find("[data-testid='analysis-panel'] tbody tr.mud-table-row").ClassList);
        Assert.Empty(cut.FindAll("[data-testid='clean-panel'] tbody tr.mud-table-row")
                        .Where(r => r.ClassList.Contains("selected-row")));
        Assert.Empty(cut.FindAll("[data-testid='deviated-panel'] tbody tr.mud-table-row")
                        .Where(r => r.ClassList.Contains("selected-row")));
    }
}
