using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.Feature.Finance.Puc.Commands;
using BusinessCentral.Application.Feature.Finance.Puc.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Finance;

[Authorize]
[RequiresModule("FIN")]
[Route("api/v1/secure/finance/puc/journal-entries")]
public sealed class JournalEntriesController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public JournalEntriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public sealed class CreateJournalEntryRequest
    {
        public DateTime EntryDate { get; set; }
        public string? EntryType { get; set; } = "MANUAL";
        public string? ReferenceType { get; set; }
        public string? ReferenceId { get; set; }
        public string? Description { get; set; }
        public int? CreatedByUserId { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateJournalEntryRequest req)
    {
        var result = await _mediator.Send(new CreateJournalEntryCommand(
            CompanyId,
            req.EntryDate,
            req.EntryType,
            req.ReferenceType,
            req.ReferenceId,
            req.Description,
            req.CreatedByUserId
        ));
        return ProcessResult(result);
    }

    public sealed class AddLineRequest
    {
        public long AccountId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? ThirdPartyDocument { get; set; }
        public string? ThirdPartyName { get; set; }
        public string? Notes { get; set; }
    }

    [HttpPost("{journalEntryId:long}/lines")]
    public async Task<IActionResult> AddLine([FromRoute] long journalEntryId, [FromBody] AddLineRequest req)
    {
        var result = await _mediator.Send(new AddJournalEntryLineCommand(
            CompanyId,
            journalEntryId,
            req.AccountId,
            req.Debit,
            req.Credit,
            req.ThirdPartyDocument,
            req.ThirdPartyName,
            req.Notes
        ));
        return ProcessResult(result);
    }

    [HttpGet("{journalEntryId:long}")]
    public async Task<IActionResult> Get([FromRoute] long journalEntryId)
    {
        var result = await _mediator.Send(new GetJournalEntryQuery(CompanyId, journalEntryId));
        return ProcessResult(result);
    }

    [HttpPost("{journalEntryId:long}/post")]
    public async Task<IActionResult> Post([FromRoute] long journalEntryId)
    {
        var result = await _mediator.Send(new PostJournalEntryCommand(CompanyId, journalEntryId));
        return ProcessResult(result);
    }
}

