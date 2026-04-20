using CloseGuardAIDemo.Web.Services;

namespace CloseGuardAIDemo.Tests.Services;

public class DataGeneratorTests
{
    [Fact]
    public void GenerateClean_CalledTwice_ReturnsDifferentBalances()
    {
        var first = DataGenerator.GenerateClean();
        var second = DataGenerator.GenerateClean();
        Assert.NotEqual(first[0].PreviousBalance, second[0].PreviousBalance);
    }

    [Fact]
    public void GenerateDeviated_CalledTwice_ReturnsDifferentBalances()
    {
        var first = DataGenerator.GenerateDeviated();
        var second = DataGenerator.GenerateDeviated();
        Assert.NotEqual(first[0].PreviousBalance, second[0].PreviousBalance);
    }
}
