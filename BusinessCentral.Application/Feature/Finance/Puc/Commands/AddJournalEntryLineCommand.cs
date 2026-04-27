using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Puc.Commands;

public sealed record AddJournalEntryLineCommand(
    int CompanyId,
    long JournalEntryId,
    long AccountId,
    decimal Debit,
    decimal Credit,
    string? ThirdPartyDocument,
    string? ThirdPartyName,
    string? Notes
) : IRequest<Result<long>>;

