using MediatR;
using BusinessCentral.Application.Common.Results;
using BusinessCentral.Application.DTOs.Auth;

namespace BusinessCentral.Application.Features.Auth.Commands.PasswordReset
{
    public record RequestPasswordResetCommand(PasswordResetRequestDTO Request) : IRequest<Result<bool>>;
}