using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Puc.Commands;

public sealed record PostJournalEntryCommand(int CompanyId, long JournalEntryId)
    : IRequest<Result<bool>>;

