using CloseGuardAIDemo.Web.Models;

namespace CloseGuardAIDemo.Web.Services;

public static class DataGenerator
{
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
        var rng = new Random();
        var snapshots = new List<AccountSnapshot>();

        foreach (var (id, name, type) in AccountDefinitions)
        {
            var previous = Math.Round((decimal)(rng.NextDouble() * 90_000 + 10_000), 2);
            var variancePct = Math.Round((decimal)(rng.NextDouble() * 10 - 5), 2);
            var current = Math.Round(previous * (1 + variancePct / 100), 2);

            snapshots.Add(new AccountSnapshot
            {
                AccountId = id,
                AccountName = name,
                AccountType = type,
                PreviousBalance = previous,
                CurrentBalance = current,
                VarianceAmount = current - previous,
                VariancePercent = variancePct,
                ManualJournalCount = rng.Next(0, 4),
                LargestManualJournalAmount = Math.Round((decimal)(rng.NextDouble() * 2_000), 2),
                UnmatchedItemCount = rng.Next(0, 3),
                DaysToCompleteReconciliation = rng.Next(1, 11),
                HasSupportDocument = true,
                MaterialityThreshold = Math.Round(previous * 0.05m, 2),
            });
        }

        return snapshots;
    }

    public static List<AccountSnapshot> GenerateDeviated()
    {
        var rng = new Random();
        var snapshots = new List<AccountSnapshot>();

        // Anomaly types assigned to fixed indices so counts are deterministic
        var anomalies = new Dictionary<int, string>
        {
            [2]  = "LargeJump",
            [5]  = "MissingSupport",
            [8]  = "ManualJournal",
            [11] = "Overdue",
            [14] = "UnmatchedItems",
            [17] = "LargeJump",
            [19] = "MissingSupport",
        };

        for (int i = 0; i < AccountDefinitions.Length; i++)
        {
            var (id, name, type) = AccountDefinitions[i];
            var previous = Math.Round((decimal)(rng.NextDouble() * 90_000 + 10_000), 2);
            var variancePct = Math.Round((decimal)(rng.NextDouble() * 10 - 5), 2);
            var current = Math.Round(previous * (1 + variancePct / 100), 2);
            int manualCount = rng.Next(0, 4);
            int unmatched = rng.Next(0, 3);
            int days = rng.Next(1, 11);
            bool hasSupport = true;

            if (anomalies.TryGetValue(i, out var anomaly))
            {
                switch (anomaly)
                {
                    case "LargeJump":
                        variancePct = Math.Round((decimal)(rng.NextDouble() * 30 + 25), 2); // 25–55%
                        current = Math.Round(previous * (1 + variancePct / 100), 2);
                        break;
                    case "MissingSupport":
                        hasSupport = false;
                        break;
                    case "ManualJournal":
                        manualCount = rng.Next(8, 15);
                        break;
                    case "Overdue":
                        days = rng.Next(20, 45);
                        break;
                    case "UnmatchedItems":
                        unmatched = rng.Next(6, 12);
                        break;
                }
            }

            snapshots.Add(new AccountSnapshot
            {
                AccountId = id,
                AccountName = name,
                AccountType = type,
                PreviousBalance = previous,
                CurrentBalance = current,
                VarianceAmount = current - previous,
                VariancePercent = variancePct,
                ManualJournalCount = manualCount,
                LargestManualJournalAmount = Math.Round((decimal)(rng.NextDouble() * 5_000), 2),
                UnmatchedItemCount = unmatched,
                DaysToCompleteReconciliation = days,
                HasSupportDocument = hasSupport,
                MaterialityThreshold = Math.Round(previous * 0.05m, 2),
            });
        }

        return snapshots;
    }
}
