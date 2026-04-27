using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Puc.Commands;

public sealed class AddJournalEntryLineHandler : IRequestHandler<AddJournalEntryLineCommand, Result<long>>
{
    private readonly IPucAccountingRepository _repo;

    public AddJournalEntryLineHandler(IPucAccountingRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<long>> Handle(AddJournalEntryLineCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<long>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        if (request.JournalEntryId <= 0)
            return Result<long>.Failure("JournalEntryId inválido.", "VALIDATION", "Validation");

        if (request.AccountId <= 0)
            return Result<long>.Failure("AccountId inválido.", "VALIDATION", "Validation");

        if (request.Debit < 0 || request.Credit < 0)
            return Result<long>.Failure("Débito/Crédito inválido.", "VALIDATION", "Validation");

        if (request.Debit > 0 && request.Credit > 0)
            return Result<long>.Failure("Una línea no puede tener débito y crédito simultáneamente.", "VALIDATION", "Validation");

        var id = await _repo.AddJournalEntryLineAsync(
            request.CompanyId,
            request.JournalEntryId,
            request.AccountId,
            request.Debit,
            request.Credit,
            request.ThirdPartyDocument,
            request.ThirdPartyName,
            request.Notes
        );

        return Result<long>.Success(id);
    }
}

