using MediatR;
using BusinessCentral.Application.Common.Results;

namespace BusinessCentral.Application.Features.Auth.Commands.Refresh
{
    public record RefreshTokenCommand(string RefreshToken)
        : IRequest<Result<BusinessCentral.Application.DTOs.Auth.LoginResponseDTO>>;
}