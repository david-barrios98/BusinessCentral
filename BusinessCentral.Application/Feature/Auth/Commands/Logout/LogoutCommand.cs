using MediatR;
using BusinessCentral.Application.Common.Results;

namespace BusinessCentral.Application.Features.Auth.Commands.Logout
{
    public record LogoutCommand(string RefreshToken, long? SessionId = null)
        : IRequest<Result<bool>>;
}