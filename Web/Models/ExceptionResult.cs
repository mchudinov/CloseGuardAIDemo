namespace CloseGuardAIDemo.Web.Models;

public enum Severity { Low, Medium, High }

public class ExceptionResult : AccountSnapshot
{
    public decimal RiskScore { get; set; }
    public Severity Severity { get; set; } = Severity.Low;
    public string LikelyCause { get; set; } = string.Empty;
}
