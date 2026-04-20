using MediatR;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Auth.Commands.Refresh
{
    public record RefreshTokenCommand(string RefreshToken, string Token)
        : IRequest<Result<BusinessCentral.Application.DTOs.Auth.LoginResponseDTO>>;
}