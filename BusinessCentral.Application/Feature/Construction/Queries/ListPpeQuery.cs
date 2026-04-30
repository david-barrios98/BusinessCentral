using BusinessCentral.Application.DTOs.Construction;
using BusinessCentral.Application.Feature.Common.Results;
using BusinessCentral.Application.Feature.Construction.Handler;
using BusinessCentral.Application.Ports.Outbound;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace BusinessCentral.Application.Feature.Construction.Handler
{
    public record ListPpeQuery(int ProjectId) : IRequest<Result<List<PpeDto>>>;
}
