using MediatR;
using BusinessCentral.Application.DTOs.Auth;
using BusinessCentral.Application.Feature.Common.Results;

namespace BusinessCentral.Application.Feature.Auth.Commands.PasswordReset
{
    public record RequestPasswordResetCommand(PasswordResetRequestDTO Request) : IRequest<Result<bool>>;
}