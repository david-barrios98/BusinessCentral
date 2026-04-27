using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;

namespace BusinessCentral.Application.Feature.Finance.Puc.Commands;

public sealed class PostJournalEntryHandler : IRequestHandler<PostJournalEntryCommand, Result<bool>>
{
    private readonly IPucAccountingRepository _repo;

    public PostJournalEntryHandler(IPucAccountingRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<bool>> Handle(PostJournalEntryCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId <= 0)
            return Result<bool>.Failure("CompanyId inválido.", "VALIDATION", "Validation");

        if (request.JournalEntryId <= 0)
            return Result<bool>.Failure("JournalEntryId inválido.", "VALIDATION", "Validation");

        var ok = await _repo.PostJournalEntryAsync(request.CompanyId, request.JournalEntryId);
        return Result<bool>.Success(ok);
    }
}

