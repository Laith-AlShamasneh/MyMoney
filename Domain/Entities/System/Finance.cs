using Domain.Shared;

namespace Domain.Entities.System;

// --- Categories ---
public class CategoryVM
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public TransactionType Type { get; set; }
    public string? Icon { get; set; }
}

// --- Transactions ---
public class TransactionVM
{
    public long TransactionId { get; set; }
    public int? CategoryId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? Notes { get; set; }
}

public class TransactionListVM
{
    public long TransactionId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType TransactionType { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? Notes { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? CategoryIcon { get; set; }
}

// --- Dashboard Statistics ---
public class DashboardStatsVM
{
    public decimal CurrentBalance { get; set; }

    public decimal TotalIncome { get; set; }
    public int IncomeTransactionsCount { get; set; }

    public decimal TotalExpense { get; set; }
    public int ExpenseTransactionsCount { get; set; }
}
