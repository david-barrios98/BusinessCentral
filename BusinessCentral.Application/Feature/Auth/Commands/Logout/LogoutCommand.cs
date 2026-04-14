using MediatR;
using BusinessCentral.Application.Common.Results;

namespace BusinessCentral.Application.Features.Auth.Commands.Logout
{
    public record LogoutCommand(
        int? UserId = null,
        int? CompanyId = null,
        string? RefreshToken = null,
        long? SessionId = null)
        : IRequest<Result<bool>>;
}