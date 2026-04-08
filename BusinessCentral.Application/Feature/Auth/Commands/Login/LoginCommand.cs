using MediatR;
using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.DTOs.Auth;

namespace BusinessCentral.Application.Features.Auth.Commands.Login
{
    public record LoginCommand(LoginRequestDTO userLogin)
        : IRequest<Result<LoginResponseDTO>>;
}