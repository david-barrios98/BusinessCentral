using MediatR;
using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.DTOs.Auth;

namespace BusinessCentral.Application.Feature.Auth.Commands.Login
{
    public record LoginCommand(
        LoginRequestDTO userLogin,
        string? IpAddress,
        string? UserAgent,
        string? Platform)
        : IRequest<Result<LoginResponseDTO>>;
}