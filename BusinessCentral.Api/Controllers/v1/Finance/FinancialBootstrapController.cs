using BusinessCentral.Api.Controllers.v1;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.DTOs.Finance;
using BusinessCentral.Application.Feature.Finance.Bootstrap.Commands;
using BusinessCentral.Application.Feature.Finance.Bootstrap.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Finance;

/// <summary>
/// Arranque financiero por escenario: constitución (desde cero), saneamiento (sin software), migración (otro sistema).
/// </summary>
[Authorize]
[RequiresModule("FIN")]
[Route("api/v1/secure/finance/bootstrap")]
public sealed class FinancialBootstrapController : SecureCompanyControllerBase
{
    private readonly IMediator _mediator;

    public FinancialBootstrapController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _mediator.Send(new GetFinancialBootstrapProfileQuery(CompanyId));
        return ProcessResult(result);
    }

    public sealed class SetProfileRequest
    {
        /// <summary>CONSTITUTION | SANITATION | MIGRATION</summary>
        public string? StartupMode { get; set; }
        public DateTime? OperatingStartDateUtc { get; set; }
        /// <summary>NOT_STARTED | IN_PROGRESS | COMPLETED</summary>
        public string BootstrapStatus { get; set; } = "NOT_STARTED";
        public string? Notes { get; set; }
    }

    [HttpPut("profile")]
    public async Task<IActionResult> SetProfile([FromBody] SetProfileRequest req)
    {
        var result = await _mediator.Send(new SetFinancialBootstrapProfileCommand(
            CompanyId,
            req.StartupMode,
            req.OperatingStartDateUtc,
            req.BootstrapStatus,
            req.Notes));
        return ProcessResult(result);
    }

    public sealed class ConstitutionRequest
    {
        public DateTime EntryDateUtc { get; set; }
        public decimal CashAmount { get; set; }
        public decimal BankAmount { get; set; }
        public decimal EquipmentAmount { get; set; }
        public decimal InventoryAmount { get; set; }
        public decimal OtherAssetsAmount { get; set; }
        public string? OtherAssetsAccountCode { get; set; }
        public string? CapitalAccountCode { get; set; }
        public string? CashAccountCode { get; set; }
        public string? BankAccountCode { get; set; }
        public string? EquipmentAccountCode { get; set; }
        public string? InventoryAccountCode { get; set; }
    }

    /// <summary>Asiento de constitución: activos/caja vs capital (PUC configurable).</summary>
    [HttpPost("opening/constitution")]
    public async Task<IActionResult> PostConstitution([FromBody] ConstitutionRequest req)
    {
        var result = await _mediator.Send(new PostConstitutionCapitalCommand(
            CompanyId,
            req.EntryDateUtc,
            req.CashAmount,
            req.BankAmount,
            req.EquipmentAmount,
            req.InventoryAmount,
            req.OtherAssetsAmount,
            req.OtherAssetsAccountCode,
            req.CapitalAccountCode,
            req.CashAccountCode,
            req.BankAccountCode,
            req.EquipmentAccountCode,
            req.InventoryAccountCode,
            UserId == 0 ? null : UserId));
        return ProcessResult(result);
    }

    public sealed class OpeningBalancesRequest
    {
        public DateTime EntryDateUtc { get; set; }
        /// <summary>SANITATION o MIGRATION (u otro etiqueta corta).</summary>
        public string OpeningKind { get; set; } = "SANITATION";
        public string? Description { get; set; }
        public List<OpeningJournalLineInputDTO> Lines { get; set; } = new();
    }

    /// <summary>Asiento de apertura libre para saneamiento o migración (debe cuadrar).</summary>
    [HttpPost("opening/balances")]
    public async Task<IActionResult> PostOpeningBalances([FromBody] OpeningBalancesRequest req)
    {
        var result = await _mediator.Send(new PostOpeningBalancesJournalCommand(
            CompanyId,
            req.EntryDateUtc,
            req.OpeningKind,
            req.Lines,
            req.Description,
            UserId == 0 ? null : UserId));
        return ProcessResult(result);
    }

    [HttpGet("validate/balance-equation")]
    public async Task<IActionResult> ValidateBalanceEquation([FromQuery] DateTime asOfUtc)
    {
        var result = await _mediator.Send(new ValidateBalanceEquationQuery(CompanyId, asOfUtc));
        return ProcessResult(result);
    }
}
