using CloseGuardAIDemo.Web.Models;

namespace CloseGuardAIDemo.Tests.Models;

public class ExceptionResultTests
{
    [Fact]
    public void ExceptionResult_InheritsFromAccountSnapshot()
    {
        var result = new ExceptionResult();
        Assert.IsAssignableFrom<AccountSnapshot>(result);
    }

    [Fact]
    public void ExceptionResult_DefaultValues_AreCorrect()
    {
        var result = new ExceptionResult();

        Assert.Equal(0m, result.RiskScore);
        Assert.Equal(Severity.Low, result.Severity);
        Assert.Equal(string.Empty, result.LikelyCause);
    }

    [Fact]
    public void ExceptionResult_CanSetAllProperties()
    {
        var result = new ExceptionResult
        {
            AccountId = "ACC-007",
            AccountName = "Accounts Payable",
            RiskScore = 0.85m,
            Severity = Severity.High,
            LikelyCause = "Large balance jump exceeding materiality threshold"
        };

        Assert.Equal("ACC-007", result.AccountId);
        Assert.Equal("Accounts Payable", result.AccountName);
        Assert.Equal(0.85m, result.RiskScore);
        Assert.Equal(Severity.High, result.Severity);
        Assert.Equal("Large balance jump exceeding materiality threshold", result.LikelyCause);
    }

    [Theory]
    [InlineData(Severity.Low)]
    [InlineData(Severity.Medium)]
    [InlineData(Severity.High)]
    public void Severity_AllValuesAreValid(Severity severity)
    {
        var result = new ExceptionResult { Severity = severity };
        Assert.Equal(severity, result.Severity);
    }
}
