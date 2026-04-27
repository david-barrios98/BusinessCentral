using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Puc.Queries;

public sealed record GetJournalEntryQuery(int CompanyId, long JournalEntryId)
    : IRequest<Result<JournalEntryDTO>>;

