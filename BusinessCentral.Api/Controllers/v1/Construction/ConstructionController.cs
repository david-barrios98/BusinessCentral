using BusinessCentral.Api.Common;
using BusinessCentral.Api.Middleware;
using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Application.Feature.Construction.Command;
using BusinessCentral.Application.Feature.Construction.Commands;
using BusinessCentral.Application.Feature.Construction.Handler;
using BusinessCentral.Application.Feature.Construction.Queries;
using BusinessCentral.Application.Feature.Construction.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.Construction
{
    [Authorize]
    [RequiresModule("CONST")]
    [Route("api/v1/construction/projects")]
    public class ConstructionController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        public ConstructionController(IMediator mediator) => _mediator = mediator;

        // ---- Projects (CRUD) ----
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDTO dto)
        {
            var cmd = new CreateProjectCommand(dto);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        [HttpGet("{projectId:int}")]
        public async Task<IActionResult> GetProjectById(int projectId)
        {
            var q = new GetProjectQuery(projectId);
            var result = await _mediator.Send(q);
            return ProcessResult(result);
        }

        [HttpGet("company/{companyId:int}")]
        public async Task<IActionResult> GetProjectsByCompany([FromRoute] int companyId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var q = new ListProjectsQuery(companyId, page, pageSize);
            var result = await _mediator.Send(q);
            return ProcessResult(result);
        }

        [HttpPut("{projectId:int}")]
        public async Task<IActionResult> UpdateProject(int projectId, [FromBody] UpdateProjectDTO dto)
        {
            dto.ProjectId = projectId;
            var cmd = new UpdateProjectCommand(dto);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        [HttpDelete("{projectId:int}")]
        public async Task<IActionResult> DeleteProject(int projectId)
        {
            var cmd = new DeleteProjectCommand(projectId);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        // ---- WorkLog / Bitácora ----
        [HttpPost("{projectId:int}/worklog")]
        public async Task<IActionResult> AddWorkLog(int projectId, [FromForm] string notes, [FromForm] List<IFormFile>? photos)
        {
            var cmd = new CreateWorkLogCommand(projectId, notes, photos);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        // ---- APU (items) ----
        [HttpPost("{projectId:int}/apu")]
        public async Task<IActionResult> AddApuItem(int projectId, [FromBody] ApuItemDto dto)
        {
            var cmd = new CreateApuItemCommand(projectId, dto);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        [HttpGet("{projectId:int}/apu")]
        public async Task<IActionResult> ListApuItems(int projectId)
        {
            var q = new ListApuItemsQuery(projectId);
            var result = await _mediator.Send(q);
            return ProcessResult(result);
        }

        // ---- Change orders ----
        [HttpPost("{projectId:int}/change-orders")]
        public async Task<IActionResult> CreateChangeOrder(int projectId, [FromBody] ChangeOrderDto dto)
        {
            var cmd = new CreateChangeOrderCommand(projectId, dto);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        [HttpGet("{projectId:int}/change-orders")]
        public async Task<IActionResult> ListChangeOrders(int projectId)
        {
            var q = new ListChangeOrdersQuery(projectId);
            var result = await _mediator.Send(q);
            return ProcessResult(result);
        }

        // ---- Tools ----
        [HttpPost("company/{companyId:int}/tools")]
        public async Task<IActionResult> CreateTool(int companyId, [FromBody] ToolDto dto)
        {
            var cmd = new CreateToolCommand(companyId, dto);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        [HttpPost("tools/{toolId:int}/loan")]
        public async Task<IActionResult> LoanTool(int toolId, [FromBody] ToolLoanDto dto)
        {
            var cmd = new LoanToolCommand(toolId, dto);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        [HttpGet("company/{companyId:int}/tools")]
        public async Task<IActionResult> ListTools(int companyId)
        {
            var q = new ListToolsQuery(companyId);
            var result = await _mediator.Send(q);
            return ProcessResult(result);
        }

        [HttpGet("tools/{toolId:int}/loans")]
        public async Task<IActionResult> ListToolLoans(int toolId)
        {
            var q = new ListToolLoansQuery(toolId);
            var result = await _mediator.Send(q);
            return ProcessResult(result);
        }

        // ---- Project expenses ----
        [HttpPost("{projectId:int}/expenses")]
        public async Task<IActionResult> AddExpense(int projectId, [FromBody] ProjectExpenseDto dto)
        {
            var cmd = new CreateProjectExpenseCommand(projectId, dto);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        [HttpGet("{projectId:int}/expenses")]
        public async Task<IActionResult> ListExpenses(int projectId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var q = new ListProjectExpensesQuery(projectId, page, pageSize);
            var result = await _mediator.Send(q);
            return ProcessResult(result);
        }

        // ---- Attendance ----
        [HttpPost("{projectId:int}/attendance")]
        public async Task<IActionResult> AddAttendance(int projectId, [FromBody] AttendanceDto dto)
        {
            var cmd = new AddAttendanceCommand(projectId, dto);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        [HttpGet("{projectId:int}/attendance")]
        public async Task<IActionResult> ListAttendance(int projectId, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var q = new ListAttendanceQuery(projectId, from, to);
            var result = await _mediator.Send(q);
            return ProcessResult(result);
        }

        // ---- PPE ----
        [HttpPost("{projectId:int}/ppe")]
        public async Task<IActionResult> AddPpe(int projectId, [FromBody] PpeDto dto)
        {
            var cmd = new AddPpeCommand(projectId, dto);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        [HttpGet("{projectId:int}/ppe")]
        public async Task<IActionResult> ListPpe(int projectId)
        {
            var q = new ListPpeQuery(projectId);
            var result = await _mediator.Send(q);
            return ProcessResult(result);
        }

        // ---- Project documents ----
        [HttpPost("{projectId:int}/documents")]
        public async Task<IActionResult> UploadDocument(int projectId, [FromForm] IFormFile file, [FromForm] string documentType)
        {
            var cmd = new UploadProjectDocumentCommand(projectId, file, documentType);
            var result = await _mediator.Send(cmd);
            return ProcessResult(result);
        }

        [HttpGet("{projectId:int}/documents")]
        public async Task<IActionResult> ListDocuments(int projectId)
        {
            var q = new ListProjectDocumentsQuery(projectId);
            var result = await _mediator.Send(q);
            return ProcessResult(result);
        }
    }
}