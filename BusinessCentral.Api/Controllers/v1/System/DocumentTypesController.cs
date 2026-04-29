using BusinessCentral.Api.Common;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Application.Ports.Outbound;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.System;

[Authorize(Policy = "SystemRole")]
[Route("api/v1/system/config/catalog/document-types")]
public sealed class DocumentTypesController : ApiControllerBase
{
    private readonly ICommonRepository _common;

    public DocumentTypesController(ICommonRepository common)
    {
        _common = common;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var data = await _common.ListDocumentTypesAsync();
        return Ok(ApiResponse<object>.Success(data, "OK", 200));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var data = await _common.GetDocumentTypeByIdAsync(id);
        if (data == null)
            return NotFound(ApiResponse<object>.Failure("Tipo de documento no encontrado.", 404));

        return Ok(ApiResponse<object>.Success(data, "OK", 200));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DocumentTypeRequest body)
    {
        var id = await _common.UpsertDocumentTypeAsync(null, body);
        return Ok(ApiResponse<object>.Success(new { id }, "OK", 200));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] DocumentTypeRequest body)
    {
        var newId = await _common.UpsertDocumentTypeAsync(id, body);
        return Ok(ApiResponse<object>.Success(new { id = newId }, "OK", 200));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await _common.DeleteDocumentTypeAsync(id);
        return Ok(ApiResponse<object>.Success(new { id, deleted = true }, "OK", 200));
    }
}

