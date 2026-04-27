using Domain.Entities.System;
using Domain.Interfaces.Shared;
using Domain.Interfaces.System;
using Domain.Shared;

namespace Infrastructure.Repositories.System;

internal class FinanceRepository(IUnitOfWork uow) : IFinanceRepository
{
    private readonly IUnitOfWork _uow = uow;

    // ==========================================
    // Dashboard & Statistics
    // ==========================================

    public async Task<DashboardStatsVM> GetDashboardStatistics(long userId)
    {
        var result = await _uow.GlobalActions.ExecuteSingleAsync<DashboardStatsVM>(
            "usp_Finance_GetDashboardStats",
            new { UserId = userId });

        return result ?? new DashboardStatsVM();
    }

    // ==========================================
    // Transactions
    // ==========================================

    public async Task<IReadOnlyList<TransactionListVM>> GetTransactions(
        long userId,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? categoryId = null,
        TransactionType? type = null)
    {
        var parameters = new
        {
            UserId = userId,
            StartDate = startDate,
            EndDate = endDate,
            CategoryId = categoryId,
            TransactionType = (int?)type
        };

        return await _uow.GlobalActions.ExecuteListAsync<TransactionListVM>(
            "usp_Finance_GetTransactions",
            parameters);
    }

    public async Task<TransactionVM?> GetTransactionById(long transactionId, long userId)
    {
        var parameters = new { TransactionId = transactionId, UserId = userId };

        return await _uow.GlobalActions.ExecuteSingleAsync<TransactionVM>(
            "usp_Finance_GetTransactionById",
            parameters);
    }

    public async Task<long> CreateTransaction(TransactionVM transaction, long userId)
    {
        var parameters = new
        {
            UserId = userId,
            transaction.CategoryId,
            transaction.Amount,
            TransactionType = (int)transaction.TransactionType,
            transaction.TransactionDate,
            transaction.Notes,
            CreatedBy = userId
        };

        // Use ExecuteScalar to return the newly generated TransactionId
        return await _uow.GlobalActions.ExecuteScalarAsync<long>(
            "usp_Finance_CreateTransaction",
            parameters);
    }

    public async Task UpdateTransaction(TransactionVM transaction, long userId)
    {
        var parameters = new
        {
            transaction.TransactionId,
            UserId = userId,
            transaction.CategoryId,
            transaction.Amount,
            TransactionType = (int)transaction.TransactionType,
            transaction.TransactionDate,
            transaction.Notes,
            ModifiedBy = userId
        };

        await _uow.GlobalActions.ExecuteAsync("usp_Finance_UpdateTransaction", parameters);
    }

    public async Task DeleteTransaction(long transactionId, long userId)
    {
        var parameters = new { TransactionId = transactionId, UserId = userId, ModifiedBy = userId };

        await _uow.GlobalActions.ExecuteAsync("usp_Finance_DeleteTransaction", parameters);
    }

    // ==========================================
    // Categories
    // ==========================================

    public async Task<IReadOnlyList<CategoryVM>> GetCategories(long userId, TransactionType? type = null)
    {
        var parameters = new { UserId = userId, Type = (int?)type };

        return await _uow.GlobalActions.ExecuteListAsync<CategoryVM>(
            "usp_Finance_GetCategories",
            parameters);
    }

    public async Task<int> CreateCategory(CategoryVM category, long userId)
    {
        var parameters = new
        {
            UserId = userId,
            category.Name,
            Type = (int)category.Type,
            category.Icon,
            CreatedBy = userId
        };

        return await _uow.GlobalActions.ExecuteScalarAsync<int>(
            "usp_Finance_CreateCategory",
            parameters);
    }

    public async Task UpdateCategory(CategoryVM category, long userId)
    {
        var parameters = new
        {
            category.CategoryId,
            UserId = userId,
            category.Name,
            Type = (int)category.Type,
            category.Icon,
            ModifiedBy = userId
        };

        await _uow.GlobalActions.ExecuteAsync("usp_Finance_UpdateCategory", parameters);
    }

    public async Task DeleteCategory(int categoryId, long userId)
    {
        var parameters = new { CategoryId = categoryId, UserId = userId, ModifiedBy = userId };

        await _uow.GlobalActions.ExecuteAsync("usp_Finance_DeleteCategory", parameters);
    }
}
