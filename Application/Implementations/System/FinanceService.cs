using Application.Common.Helpers;
using Application.DTOs.Shared;
using Application.Helpers;
using Application.Interfaces.System;
using Domain.Entities.System;
using Domain.Interfaces.Shared;
using Domain.Interfaces.System;
using Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Application.Implementations.System;

public class FinanceService(
    IFinanceRepository financeRepository,
    IUserContext userContext,
    ILogger<FinanceService> logger) : IFinanceService
{
    private readonly IFinanceRepository _financeRepository = financeRepository;
    private readonly IUserContext _userContext = userContext;
    private readonly ILogger<FinanceService> _logger = logger;

    public async Task<ServiceResponse<DashboardStatsVM>> GetDashboardStatisticsAsync(CancellationToken ct = default)
    {
        return await ExecutionHelper.ExecuteAsync(async () =>
        {
            var data = await _financeRepository.GetDashboardStatistics(_userContext.UserId);
            return ServiceResponse<DashboardStatsVM>.Success(data, MessagesHelper.GetMessage(MessageType.RetrieveSuccessfully, _userContext.Language));
        }, _logger, "Get Dashboard Stats", null, _userContext.Language, ct);
    }

    public async Task<ServiceResponse<IReadOnlyList<TransactionListVM>>> GetTransactionsAsync(DateTime? startDate, DateTime? endDate, int? categoryId, TransactionType? type, CancellationToken ct = default)
    {
        return await ExecutionHelper.ExecuteAsync(async () =>
        {
            var data = await _financeRepository.GetTransactions(_userContext.UserId, startDate, endDate, categoryId, type);
            return ServiceResponse<IReadOnlyList<TransactionListVM>>.Success(data, MessagesHelper.GetMessage(MessageType.RetrieveSuccessfully, _userContext.Language));
        }, _logger, "Get Transactions", new { startDate, endDate, categoryId, type }, _userContext.Language, ct);
    }

    public async Task<ServiceResponse<TransactionVM>> GetTransactionByIdAsync(long transactionId, CancellationToken ct = default)
    {
        return await ExecutionHelper.ExecuteAsync(async () =>
        {
            var data = await _financeRepository.GetTransactionById(transactionId, _userContext.UserId);

            if (data is null)
                return ServiceResponse<TransactionVM>.Failure(ErrorCodes.Common.NOT_FOUND, MessagesHelper.GetMessage(MessageType.InvalidInput, _userContext.Language), HttpResponseStatus.NotFound);

            return ServiceResponse<TransactionVM>.Success(data, MessagesHelper.GetMessage(MessageType.RetrieveSuccessfully, _userContext.Language));
        }, _logger, "Get Transaction By Id", transactionId, _userContext.Language, ct);
    }

    public async Task<ServiceResponse<long>> CreateTransactionAsync(TransactionVM request, CancellationToken ct = default)
    {
        return await ExecutionHelper.ExecuteAsync(async () =>
        {
            var newId = await _financeRepository.CreateTransaction(request, _userContext.UserId);
            return ServiceResponse<long>.Success(newId, MessagesHelper.GetMessage(MessageType.Success, _userContext.Language), HttpResponseStatus.Created);
        }, _logger, "Create Transaction", request, _userContext.Language, ct);
    }

    public async Task<ServiceResponse<bool>> UpdateTransactionAsync(TransactionVM request, CancellationToken ct = default)
    {
        return await ExecutionHelper.ExecuteAsync(async () =>
        {
            await _financeRepository.UpdateTransaction(request, _userContext.UserId);
            return ServiceResponse<bool>.Success(true, MessagesHelper.GetMessage(MessageType.Success, _userContext.Language));
        }, _logger, "Update Transaction", request, _userContext.Language, ct);
    }

    public async Task<ServiceResponse<bool>> DeleteTransactionAsync(long transactionId, CancellationToken ct = default)
    {
        return await ExecutionHelper.ExecuteAsync(async () =>
        {
            await _financeRepository.DeleteTransaction(transactionId, _userContext.UserId);
            return ServiceResponse<bool>.Success(true, MessagesHelper.GetMessage(MessageType.Success, _userContext.Language));
        }, _logger, "Delete Transaction", transactionId, _userContext.Language, ct);
    }

    public async Task<ServiceResponse<IReadOnlyList<CategoryVM>>> GetCategoriesAsync(TransactionType? type, CancellationToken ct = default)
    {
        return await ExecutionHelper.ExecuteAsync(async () =>
        {
            var data = await _financeRepository.GetCategories(_userContext.UserId, type);
            return ServiceResponse<IReadOnlyList<CategoryVM>>.Success(data, MessagesHelper.GetMessage(MessageType.RetrieveSuccessfully, _userContext.Language));
        }, _logger, "Get Categories", type, _userContext.Language, ct);
    }

    public async Task<ServiceResponse<int>> CreateCategoryAsync(CategoryVM request, CancellationToken ct = default)
    {
        return await ExecutionHelper.ExecuteAsync(async () =>
        {
            var newId = await _financeRepository.CreateCategory(request, _userContext.UserId);
            return ServiceResponse<int>.Success(newId, MessagesHelper.GetMessage(MessageType.Success, _userContext.Language), HttpResponseStatus.Created);
        }, _logger, "Create Category", request, _userContext.Language, ct);
    }

    public async Task<ServiceResponse<bool>> UpdateCategoryAsync(CategoryVM request, CancellationToken ct = default)
    {
        return await ExecutionHelper.ExecuteAsync(async () =>
        {
            await _financeRepository.UpdateCategory(request, _userContext.UserId);
            return ServiceResponse<bool>.Success(true, MessagesHelper.GetMessage(MessageType.Success, _userContext.Language));
        }, _logger, "Update Category", request, _userContext.Language, ct);
    }

    public async Task<ServiceResponse<bool>> DeleteCategoryAsync(int categoryId, CancellationToken ct = default)
    {
        return await ExecutionHelper.ExecuteAsync(async () =>
        {
            await _financeRepository.DeleteCategory(categoryId, _userContext.UserId);
            return ServiceResponse<bool>.Success(true, MessagesHelper.GetMessage(MessageType.Success, _userContext.Language));
        }, _logger, "Delete Category", categoryId, _userContext.Language, ct);
    }
}