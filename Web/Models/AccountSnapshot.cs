namespace CloseGuardAIDemo.Web.Models;

public class AccountSnapshot
{
    public string AccountId { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public decimal PreviousBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal VarianceAmount { get; set; }
    public decimal VariancePercent { get; set; }
    public int ManualJournalCount { get; set; }
    public decimal LargestManualJournalAmount { get; set; }
    public int UnmatchedItemCount { get; set; }
    public int DaysToCompleteReconciliation { get; set; }
    public bool HasSupportDocument { get; set; }
    public decimal MaterialityThreshold { get; set; }
}
