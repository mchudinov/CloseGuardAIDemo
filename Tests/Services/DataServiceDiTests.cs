using CloseGuardAIDemo.Web.Extensions;
using CloseGuardAIDemo.Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CloseGuardAIDemo.Tests.Services;

public class DataServiceDiTests
{
    [Fact]
    public void AddApplicationServices_DataServiceDescriptorIsSingleton()
    {
        var services = new ServiceCollection();
        services.AddApplicationServices();

        var descriptor = services.Single(d => d.ServiceType == typeof(DataService));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AddApplicationServices_ResolvingDataServiceTwiceReturnsSameInstance()
    {
        var services = new ServiceCollection();
        services.AddApplicationServices();
        var provider = services.BuildServiceProvider();

        var svc1 = provider.GetRequiredService<DataService>();
        var svc2 = provider.GetRequiredService<DataService>();

        Assert.Same(svc1, svc2);
    }
}
