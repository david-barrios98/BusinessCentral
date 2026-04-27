using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Puc.Commands;

public sealed class CreateJournalEntryHandler : IRequestHandler<CreateJournalEntryCommand, Result<long>>
{
    private readonly IPucAccountingRepository _repo;

    public CreateJournalEntryHandler(IPucAccountingRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<long>> Handle(CreateJournalEntryCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<long>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        var id = await _repo.CreateJournalEntryAsync(
            request.CompanyId,
            request.EntryDate,
            request.EntryType,
            request.ReferenceType,
            request.ReferenceId,
            request.Description,
            request.CreatedByUserId
        );

        return Result<long>.Success(id);
    }
}

