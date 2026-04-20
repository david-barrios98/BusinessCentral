using MediatR;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Auth.Commands.Login
{
    public record LoginCommand(
        LoginRequestDTO userLogin,
        string? IpAddress,
        string? UserAgent,
        string? Platform)
        : IRequest<Result<LoginResponseDTO>>;
}