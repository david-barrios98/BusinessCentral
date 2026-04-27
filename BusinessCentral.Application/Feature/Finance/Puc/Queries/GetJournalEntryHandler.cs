using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Puc.Queries;

public sealed class GetJournalEntryHandler : IRequestHandler<GetJournalEntryQuery, Result<JournalEntryDTO>>
{
    private readonly IPucAccountingRepository _repo;

    public GetJournalEntryHandler(IPucAccountingRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<JournalEntryDTO>> Handle(GetJournalEntryQuery request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<JournalEntryDTO>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        if (request.JournalEntryId <= 0)
            return Result<JournalEntryDTO>.Failure("JournalEntryId inválido.", "VALIDATION", "Validation");

        var data = await _repo.GetJournalEntryAsync(request.CompanyId, request.JournalEntryId);
        if (data is null)
            return Result<JournalEntryDTO>.Failure("Comprobante no encontrado.", "NOT_FOUND", "NotFound");

        return Result<JournalEntryDTO>.Success(data);
    }
}

