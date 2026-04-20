using Bunit;
using CloseGuardAIDemo.Web.Services;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Web.Components.Layout;

namespace CloseGuardAIDemo.Tests.Components;

public class MainLayoutTests : IAsyncLifetime
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
    public void GenerateRandomDataButton_Click_SetsDatasetToClean()
    {
        var dataService = new DataService();
        _ctx.Services.AddSingleton(dataService);

        var cut = _ctx.Render<MainLayout>();
        cut.FindAll("button").Single(b => b.TextContent.Contains("Random")).Click();

        Assert.Equal(DataSet.Clean, dataService.ActiveDataset);
    }

    [Fact]
    public void GenerateDeviatedDataButton_Click_SetsDatasetToDeviated()
    {
        var dataService = new DataService();
        _ctx.Services.AddSingleton(dataService);

        var cut = _ctx.Render<MainLayout>();
        cut.FindAll("button").Single(b => b.TextContent.Contains("Deviated")).Click();

        Assert.Equal(DataSet.Deviated, dataService.ActiveDataset);
    }
}
