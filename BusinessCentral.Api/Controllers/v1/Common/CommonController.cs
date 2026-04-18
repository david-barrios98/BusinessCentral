using BusinessCentral.Api.Common;
using BusinessCentral.Application.Feature.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCentral.Api.Controllers.v1.Common
{
    [Route("api/v1/public/common")]
    public class CommonController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public CommonController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region Geografía

        [HttpGet("countries")]
        public async Task<IActionResult> GetCountries()
        {
            var result = await _mediator.Send(new GetCountriesQuery());
            return ProcessResult(result);
        }

        [HttpGet("countries/{countryId:int}/departments")]
        public async Task<IActionResult> GetDepartments(int countryId)
        {
            var result = await _mediator.Send(new GetDepartmentsByCountryQuery(countryId));
            return ProcessResult(result);
        }

        [HttpGet("departments/{departmentId:int}/cities")]
        public async Task<IActionResult> GetCities(int departmentId)
        {
            var result = await _mediator.Send(new GetCitiesByDepartmentQuery(departmentId));
            return ProcessResult(result);
        }

        #endregion

        #region Documentos y Configuración

        [HttpGet("document-types")]
        public async Task<IActionResult> GetDocumentTypes()
        {
            var result = await _mediator.Send(new GetDocumentTypesQuery());
            return ProcessResult(result);
        }

        [HttpGet("membership-plans")]
        public async Task<IActionResult> GetMembershipPlans()
        {
            var result = await _mediator.Send(new GetMembershipPlansQuery());
            return ProcessResult(result);
        }

        [HttpGet("membership-plans/{id:int}")]
        public async Task<IActionResult> GetMembershipPlanById(int id)
        {
            var result = await _mediator.Send(new GetMembershipPlanByIdQuery(id));
            return ProcessResult(result);
        }

        #endregion
    }
}
