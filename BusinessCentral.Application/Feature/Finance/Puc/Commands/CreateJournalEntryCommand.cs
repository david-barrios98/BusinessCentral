using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Puc.Commands;

public sealed record CreateJournalEntryCommand(
    int CompanyId,
    DateTime EntryDate,
    string? EntryType,
    string? ReferenceType,
    string? ReferenceId,
    string? Description,
    int? CreatedByUserId
) : IRequest<Result<long>>;

