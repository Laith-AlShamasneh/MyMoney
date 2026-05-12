using Application.DTOs.Shared;
using Domain.Entities.System;
using Domain.Shared;

namespace Application.Interfaces.System;

public interface IFinanceService
{
    Task<ServiceResponse<DashboardStatsVM>> GetDashboardStatisticsAsync(CancellationToken ct = default);

    Task<ServiceResponse<IReadOnlyList<TransactionListVM>>> GetTransactionsAsync(DateTime? startDate, DateTime? endDate, int? categoryId, TransactionType? type, CancellationToken ct = default);
    Task<ServiceResponse<TransactionVM>> GetTransactionByIdAsync(long transactionId, CancellationToken ct = default);
    Task<ServiceResponse<long>> CreateTransactionAsync(TransactionVM request, CancellationToken ct = default);
    Task<ServiceResponse<bool>> UpdateTransactionAsync(TransactionVM request, CancellationToken ct = default);
    Task<ServiceResponse<bool>> DeleteTransactionAsync(long transactionId, CancellationToken ct = default);

    Task<ServiceResponse<IReadOnlyList<CategoryVM>>> GetCategoriesAsync(TransactionType? type, CancellationToken ct = default);
    Task<ServiceResponse<int>> CreateCategoryAsync(CategoryVM request, CancellationToken ct = default);
    Task<ServiceResponse<bool>> UpdateCategoryAsync(CategoryVM request, CancellationToken ct = default);
    Task<ServiceResponse<bool>> DeleteCategoryAsync(int categoryId, CancellationToken ct = default);
}