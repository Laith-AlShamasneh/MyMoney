using Domain.Entities.System;
using Domain.Shared;

namespace Domain.Interfaces.System;

public interface IFinanceRepository
{
    // ==========================================
    // Dashboard & Statistics
    // ==========================================

    /// <summary>
    /// Gets the user's current balance, total deposits, total withdrawals, and transaction counts.
    /// </summary>
    Task<DashboardStatsVM> GetDashboardStatistics(long userId);


    // ==========================================
    // Transactions
    // ==========================================

    /// <summary>
    /// Gets a filtered list of transactions for the table page.
    /// Parameters like startDate and categoryId are optional to allow dynamic filtering.
    /// </summary>
    Task<IReadOnlyList<TransactionListVM>> GetTransactions(
        long userId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? categoryId = null,
        TransactionType? type = null);

    /// <summary>
    /// Gets a single transaction for the Edit page.
    /// </summary>
    Task<TransactionVM?> GetTransactionById(long transactionId, long userId);

    /// <summary>
    /// Adds a new deposit or withdrawal.
    /// </summary>
    Task<long> CreateTransaction(TransactionVM transaction, long userId);

    /// <summary>
    /// Updates an existing transaction.
    /// </summary>
    Task UpdateTransaction(TransactionVM transaction, long userId);

    /// <summary>
    /// Deletes a transaction.
    /// </summary>
    Task DeleteTransaction(long transactionId, long userId);


    // ==========================================
    // Categories
    // ==========================================

    /// <summary>
    /// Gets all categories belonging to the user. Useful for populating dropdowns on the Add/Edit page.
    /// </summary>
    Task<IReadOnlyList<CategoryVM>> GetCategories(long userId, TransactionType? type = null);

    /// <summary>
    /// Creates a custom category for the user.
    /// </summary>
    Task<int> CreateCategory(CategoryVM category, long userId);

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    Task UpdateCategory(CategoryVM category, long userId);

    /// <summary>
    /// Deletes a category (should ideally be a soft-delete to preserve transaction history).
    /// </summary>
    Task DeleteCategory(int categoryId, long userId);
}
