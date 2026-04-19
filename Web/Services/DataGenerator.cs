using CloseGuardAIDemo.Web.Models;

namespace CloseGuardAIDemo.Web.Services;

public static class DataGenerator
{
    private static readonly Random _rng = new(42);

    private static readonly (string Id, string Name, string Type)[] AccountDefinitions =
    [
        ("ACC-001", "Cash and Cash Equivalents", "Asset"),
        ("ACC-002", "Accounts Receivable", "Asset"),
        ("ACC-003", "Inventory", "Asset"),
        ("ACC-004", "Prepaid Expenses", "Asset"),
        ("ACC-005", "Property, Plant & Equipment", "Asset"),
        ("ACC-006", "Accounts Payable", "Liability"),
        ("ACC-007", "Accrued Liabilities", "Liability"),
        ("ACC-008", "Short-Term Debt", "Liability"),
        ("ACC-009", "Long-Term Debt", "Liability"),
        ("ACC-010", "Deferred Revenue", "Liability"),
        ("ACC-011", "Common Stock", "Equity"),
        ("ACC-012", "Retained Earnings", "Equity"),
        ("ACC-013", "Revenue", "Income"),
        ("ACC-014", "Cost of Goods Sold", "Expense"),
        ("ACC-015", "Salaries Expense", "Expense"),
        ("ACC-016", "Rent Expense", "Expense"),
        ("ACC-017", "Utilities Expense", "Expense"),
        ("ACC-018", "Marketing Expense", "Expense"),
        ("ACC-019", "Depreciation Expense", "Expense"),
        ("ACC-020", "Interest Expense", "Expense"),
    ];

    public static List<AccountSnapshot> GenerateClean()
    {
        var snapshots = new List<AccountSnapshot>();

        foreach (var (id, name, type) in AccountDefinitions)
        {
            var previous = Math.Round((decimal)(_rng.NextDouble() * 90_000 + 10_000), 2);
            var variancePct = Math.Round((decimal)(_rng.NextDouble() * 10 - 5), 2); // ±5%
            var current = Math.Round(previous * (1 + variancePct / 100), 2);
            var variance = current - previous;

            snapshots.Add(new AccountSnapshot
            {
                AccountId = id,
                AccountName = name,
                AccountType = type,
                PreviousBalance = previous,
                CurrentBalance = current,
                VarianceAmount = variance,
                VariancePercent = variancePct,
                ManualJournalCount = _rng.Next(0, 4),
                LargestManualJournalAmount = Math.Round((decimal)(_rng.NextDouble() * 2_000), 2),
                UnmatchedItemCount = _rng.Next(0, 3),
                DaysToCompleteReconciliation = _rng.Next(1, 11),
                HasSupportDocument = true,
                MaterialityThreshold = Math.Round(previous * 0.05m, 2),
            });
        }

        return snapshots;
    }
}
